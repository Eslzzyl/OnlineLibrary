<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">当前借阅</v-banner>
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
    title: '借阅用户',
    align: 'center',
    sortable: true,
    key: 'userName',
  },
  {
    title: '借阅时间',
    align: 'center',
    sortable: true,
    key: 'borrowDate',
  },
  {
    title: '借阅时长（天）',
    align: 'center',
    sortable: true,
    key: 'borrowDuration',
  },
])

async function request(page, itemsPerPage, sortBy, search) {
  let orderBy = '';
  let order = '';
  if (sortBy.length !== 0) {
    orderBy = sortBy[0].key
    order = sortBy[0].order
  }
  const url = `/book/currentborrow?pageIndex=${page - 1}&pageSize=${itemsPerPage}&sortColumn=${orderBy}&sortOrder=${order}&filterQuery=${search}`
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
    