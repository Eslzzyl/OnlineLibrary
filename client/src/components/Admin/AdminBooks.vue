<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">查看书籍</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col cols="3">
            <v-text-field variant="outlined" density="compact" label="检索书籍..." v-model="search" clearable></v-text-field>
          </v-col>
          <v-spacer></v-spacer>
          <v-col cols="2">
            <v-card rounded="xl" variant="tonal">
              <div class="ml-2 mr-2 mt-2 mb-2 d-flex justify-center">
                共 {{ totalItems }} 本书籍
              </div>
            </v-card>
          </v-col>
          <v-col cols="1">
            <v-btn variant="tonal" @click="add(item)">新增</v-btn>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据..." fixed-header height="60vh">
              <template v-slot:item.moreInfo="{ item }">
                <v-btn variant="tonal" @click="edit(item)">修改</v-btn>
                <v-btn variant="tonal" color="red-darken-1" @click="deleteBook(item)">删除</v-btn>
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
      <v-dialog v-model="addDialog" max-width="80vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">新增书籍</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              待补
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="addDialog = false;">取消</v-btn>
            <v-btn color="blue-darken-1" variant="text" @click="addDialog = false; addConfirmed()">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="editDialog" max-width="80vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">修改书籍</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              待补
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="editDialog = false;">取消</v-btn>
            <v-btn color="blue-darken-1" variant="text" @click="editDialog = false; editConfirmed()">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="deleteDialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">删除书籍</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              你确定要删除这本书吗？该操作不可撤销。删除后，与这本书相关联的所有借书记录也将被一并删除。
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="deleteDialog = false;">取消</v-btn>
            <v-btn color="red-darken-1" variant="text" @click="deleteDialog = false; deleteConfirmed()">确定</v-btn>
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
const addDialog = ref(false)
const editDialog = ref(false)
const deleteDialog = ref(false)
const currItem = ref(null)

const snackbar = ref(false)
const prompt = ref('')

function add(item) {
  console.log(item)
  addDialog.value = true
}

function edit(item) {
  console.log(item)
  currItem.value = item
  editDialog.value = true
}

function deleteBook(item) {
  console.log(item)
  currItem.value = item
  deleteDialog.value = true
}

function addConfirmed() {
  const url = '/book/';
  axiosInstance.post(url, {
    id: currItem.value.id
  })
}

function editConfirmed() {
  const url = '/book/';
  axiosInstance.put(url, {
    id: currItem.value.id
  })
}

function deleteConfirmed() {
  const url = '/book/';
  axiosInstance.delete(url, {
    id: currItem.value.id
  }).then((response) => {
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
    console.log(error)
    prompt.value = "删除失败：" + error;
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
    title: '书名',
    align: 'center',
    sortable: true,
    key: 'title'
  },
  {
    title: '作者',
    align: 'center',
    sortable: true,
    key: 'author',
  },
  {
    title: '出版社',
    align: 'center',
    sortable: true,
    key: 'publisher',
  },
  {
    title: '出版时间',
    align: 'center',
    sortable: true,
    key: 'publishedDate',
  },
  {
    title: '索书号',
    align: 'center',
    sortable: true,
    key: 'identifier',
  },
  {
    title: '在馆数量',
    align: 'center',
    sortable: true,
    key: 'inventory',
  },
  {
    title: '累计借阅',
    align: 'center',
    sortable: true,
    key: 'borrowed',
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
  const url = `/book?pageIndex=${page - 1}&pageSize=${itemsPerPage}&sortColumn=${orderBy}&sortOrder=${order}&filterQuery=${search}`
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
    tableData.value = packedData
  }
}

</script>
  