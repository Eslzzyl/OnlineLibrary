<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">用户管理</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col cols="3">
            <v-text-field variant="outlined" density="compact" label="检索用户..." v-model="search" clearable></v-text-field>
          </v-col>
          <v-spacer></v-spacer>
          <v-col cols="2">
            共 {{ totalItems }} 名用户
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据..." fixed-header height="60vh">
              <template v-slot:item.avatar="{ item }">
                <v-avatar>
                  <v-img :src="item.avatar" />
                </v-avatar>
              </template>
              <template v-slot:item.moreInfo="{ item }">
                <v-btn :disabled="item.roles.includes('Admin')" variant="tonal" color="red-darken-1" @click="deleteUser(item)">删除</v-btn>
              </template>
              <template v-slot:bottom>
                <div class="text-center pt-2">
                  <v-pagination rounded="circle" v-model="page" :length="pageCount"></v-pagination>
                </div>
              </template>
            </v-data-table-server>
          </v-col>
        </v-row>
      </v-container>
      <v-dialog v-model="dialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">确认删除</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <span>你确定要删除这个用户吗？该操作不可撤销。删除后，与该用户关联的所有借阅记录也将被一并删除。</span>
              </v-row>
              <v-row>
                <span>{{ currItem.userName }}</span>
              </v-row>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="dialog = false;">取消</v-btn>
            <v-btn color="red-darken-1" variant="text" @click="dialog = false; deleteConfirmed()">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
    <v-fade-transition>
      <v-alert v-if="isErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
        加载似乎出了一点问题。尝试刷新页面？<br>{{ requestError }}
      </v-alert>
    </v-fade-transition>
    <v-snackbar v-model="snackbar" timeout="5000" rounded="pill" :color="skyColor">
      {{ prompt }}
    </v-snackbar>
  </v-container>
</template>
  
<script setup>
import axiosInstance from '@/plugins/util/axiosInstance'
import { ref, watch } from 'vue';
import { getSkyColor } from '@/plugins/util/color';

import '@/style.css';

const skyColor = getSkyColor();

const tableData = ref([])

const search = ref('')

const totalItems = ref(0)
const itemsPerPage = ref(10)
const pageCount = ref(0)
const page = ref(0)
const sortBy = ref([])

const requestError = ref(null)
const isErrorHappened = ref(false)
const loading = ref(true)
const dialog = ref(false)
const currItem = ref(null)

const snackbar = ref(false)
const prompt = ref('')

function deleteUser(item) {
  console.log(item)
  currItem.value = item
  dialog.value = true
}

function deleteConfirmed() {
  const url = `/admin/user?userId=${currItem.value.id}`;
  axiosInstance.delete(url)
  .then((response) => {
    if (response.data.code === 0) {
      console.log("删除成功");
      prompt.value = "删除成功";
      snackbar.value = true;
      loadItems({
        page: 1,
        itemsPerPage: itemsPerPage.value,
        sortBy: sortBy.value
      })
    } else {
      console.log("删除失败");
      prompt.value = "删除失败" + response.data.message;
      snackbar.value = true;
    }
  }).catch((error) => {
    console.log(error);
    prompt.value = "删除失败" + error.message;
    snackbar.value = true;
  })
}

watch(page, () => {
  loadItems({
    page: page.value,
    itemsPerPage: itemsPerPage.value,
    sortBy: sortBy.value
  })
})

const headers = ref([
  {
    title: '头像',
    align: 'center',
    sortable: true,
    key: 'avatar',
  },
  {
    title: '用户名',
    align: 'center',
    sortable: true,
    key: 'userName',
  },
  {
    title: '邮箱',
    align: 'center',
    sortable: true,
    key: 'email',
  },
  {
    title: '电话',
    align: 'center',
    sortable: true,
    key: 'phone',
  },
  {
    title: '用户组',
    align: 'center',
    sortable: true,
    key: 'rolesStr',
  },
  {
    title: '操作',
    align: 'center',
    sortable: false,
    key: 'moreInfo',
  },
])

async function request(page, itemsPerPage, sortBy, search) {
  let orderBy = '';
  let order = '';
  if (sortBy.length !== 0) {
    orderBy = sortBy[0].key
    order = sortBy[0].order
  }
  const url = `/admin/users?pageIndex=${page - 1}&pageSize=${itemsPerPage}&sortColumn=${orderBy}&sortOrder=${order}&filterQuery=${search}`
  try {
    const response = await axiosInstance.get(url)

    if (response.data.code === 0) {
      return response.data
    } else {
      isErrorHappened.value = true
      console.error('请求失败！')
    }
  } catch (error) {
    console.log(error)
    requestError.value = error.message
    isErrorHappened.value = true
  }
}

async function loadItems({ page, itemsPerPage, sortBy }) {
  loading.value = true
  const result = await request(page, itemsPerPage, sortBy, search.value)
  console.log("result: ", result)
  totalItems.value = result.recordCount
  pageCount.value = Math.floor(result.recordCount / result.pageSize) + 1
  const packedData = result.data
  loading.value = false
  if (packedData.length !== 0) {
    console.log(packedData)
    packedData.forEach(item => {
      if (item.roles.length !== 0) {
        item.rolesStr = item.roles.join(', ');
      } else {
        item.rolesStr = '未知'
      }
    });
    tableData.value = packedData
  }
}

</script>
  