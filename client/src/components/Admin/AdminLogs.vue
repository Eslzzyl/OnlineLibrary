<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">日志管理</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col cols="3">
            <v-text-field variant="outlined" density="compact" label="检索日志..." v-model="search" clearable></v-text-field>
          </v-col>
          <v-spacer></v-spacer>
          <v-col cols="2">
            <v-card rounded="xl" variant="tonal">
              <div class="ml-2 mr-2 mt-2 mb-2 d-flex justify-center">
                共 {{ totalItems }} 条日志
              </div>
            </v-card>
          </v-col>
          <v-col cols="1">
            <v-btn variant="tonal" @click="clear">清空</v-btn>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据..." fixed-header height="60vh">
              <template v-slot:bottom>
                <div class="text-center pt-2">
                  <v-pagination rounded="circle" v-model="page" :length="pageCount"></v-pagination>
                </div>
              </template>
            </v-data-table-server>
          </v-col>
        </v-row>
      </v-container>
      <v-dialog v-model="clearDialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">清空日志</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              你确定要清空所有的日志吗？该操作不可撤销。如果日志数量较多，这可能需要一段时间来执行。
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="clearDialog = false;">取消</v-btn>
            <v-btn color="red-darken-1" variant="text" @click="clearDialog = false; clearConfirmed()">确定</v-btn>
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

const snackbar = ref(false)
const prompt = ref('')

const clearDialog = ref(false)

watch(page, () => {
  loadItems({
    page: page.value,
    itemsPerPage: itemsPerPage.value,
    sortBy: sortBy.value
  })
})

const headers = ref([
  {
    title: 'ID',
    align: 'end',
    sortable: true,
    key: 'id'
  },
  {
    title: '时间戳',
    align: 'start',
    sortable: true,
    key: 'timestamp',
  },
  {
    title: '级别',
    align: 'center',
    sortable: true,
    key: 'level',
  },
  {
    title: '异常',
    align: 'start',
    sortable: false,
    key: 'exception',
  },
  {
    title: '消息',
    align: 'start',
    sortable: false,
    key: 'renderedMessage',
  },
  {
    title: 'Properties',
    align: 'start',
    sortable: false,
    key: 'properties',
  },
])

function clear() {
  clearDialog.value = true;
}

function clearConfirmed() {
  axiosInstance.delete('/Log')
    .then(() => {
      prompt.value = "日志已清空"
      snackbar.value = true
      loadItems({
        page: 1,
        itemsPerPage: itemsPerPage.value,
        sortBy: sortBy.value
      })
    })
    .catch((error) => {
      prompt.value = "清空失败：" + error
      snackbar.value = true
    })
}

async function request(page, itemsPerPage, sortBy, search) {
  let orderBy = '';
  let order = '';
  if (sortBy.length !== 0) {
    orderBy = sortBy[0].key
    order = sortBy[0].order
  }
  const url = `/Log?pageIndex=${page - 1}&pageSize=${itemsPerPage}&sortColumn=${orderBy}&sortOrder=${order}&filterQuery=${search}`
  try {
    const response = await axiosInstance.get(url)

    if (response.data.code === 0) {
      return response.data
    } else {
      isErrorHappened.value = true
      console.error('请求失败！')
    }
  } catch (error) {
    console.error(error)
    requestError.value = error.message
    isErrorHappened.value = true
  }
}

async function loadItems({ page, itemsPerPage, sortBy }) {
  loading.value = true
  const result = await request(page, itemsPerPage, sortBy, search.value)
  totalItems.value = result.recordCount
  pageCount.value = Math.floor(result.recordCount / result.pageSize) + 1
  const packedData = result.data
  loading.value = false
  if (packedData.length !== 0) {
    tableData.value = packedData
  }
}

</script>
    