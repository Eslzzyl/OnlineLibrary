<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">查看荐购</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col cols="3">
            <v-text-field variant="outlined" density="compact" label="检索记录..." v-model="search" clearable></v-text-field>
          </v-col>
          <v-spacer></v-spacer>
          <v-col cols="2">
            <v-card rounded="xl" variant="tonal">
              <div class="ml-2 mr-2 mt-2 mb-2 d-flex justify-center">
                共 {{ totalItems }} 条荐购记录
              </div>
            </v-card>
          </v-col>
          <v-col cols="1">
            <v-btn variant="tonal" color="blue-darken-1" @click="newRecommendDialog = true">新增</v-btn>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据..." fixed-header height="60vh">
              <template v-slot:item.moreInfo="{ item }">
                <v-btn variant="tonal" @click="moreInfo(item)">详情</v-btn>
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
      <v-dialog v-model="moreInfoDialog" max-width="70vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">荐购详情</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col>
                  书名：{{ currItem.title }}
                </v-col>
                <v-col>
                  作者：{{ currItem.author }}
                </v-col>
                <v-col>
                  出版社：{{ currItem.publisher }}
                </v-col>
                <v-col>
                  ISBN：{{ currItem.isbn }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  用户备注：{{ currItem.userRemark }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  推荐用户：{{ currItem.userName }}
                </v-col>
                <v-col>
                  推荐时间：{{ currItem.createTime }}
                </v-col>
                <v-col>
                  是否已处理：{{ currItem.isProcessed }}
                </v-col>
                <v-col v-if="currItem.isProcessed">
                  处理人：{{ currItem.adminName }}
                </v-col>
                <v-col v-if="currItem.isProcessed">
                  处理时间：{{ currItem.updateTime }}
                </v-col>
              </v-row>
              <v-row v-if="currItem.isProcessed">
                <v-col>
                  处理备注：{{ currItem.adminRemark }}
                </v-col>
              </v-row>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="moreInfoDialog = false;">关闭</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="newRecommendDialog" max-width="70vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">新增荐购</span>
          </v-card-title>
          <v-card-subtitle>
            <span>请输入尽可能详细的信息，以便我们找到你想要的书籍</span>
          </v-card-subtitle>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" clearable :rules="[notNullRule]" v-model="title" label="书名" hint="必填"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="author" label="作者" hint="选填"
                    persistent-hint></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="publisher" label="出版社" hint="选填"
                    persistent-hint></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" clearable v-model="isbn" label="ISBN" hint="选填"
                    persistent-hint></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-textarea variant="outlined" clearable v-model="remark" label="备注" hint="选填"
                    persistent-hint></v-textarea>
                </v-col>
              </v-row>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="newRecommendDialog = false;">取消</v-btn>
            <v-btn color="blue-darken-1" variant="text"
              @click="newRecommendDialog = false; newRecommendConfirmed()">提交</v-btn>
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
import { notNullRule } from '@/plugins/util/rules';
import '@/style.css';

const skyColor = getSkyColor();

const tableData = ref([])

const search = ref('')

const currItem = ref(null)

const totalItems = ref(0)
const itemsPerPage = ref(10)
const pageCount = ref(0)
const page = ref(0)
const sortBy = ref([])

const requestError = ref(null)
const isErrorHappened = ref(false)
const loading = ref(true)

const newRecommendDialog = ref(false)
const moreInfoDialog = ref(false)

const submitButtonDisabled = ref(false)

const snackbar = ref(false)
const prompt = ref('')

const title = ref('')
const author = ref('')
const publisher = ref('')
const isbn = ref('')
const remark = ref('')

function moreInfo(item) {
  currItem.value = item
  moreInfoDialog.value = true
}

function newRecommendConfirmed() {
  const url = '/user/recommend';
  axiosInstance.post(url, {
    Title: title.value,
    Author: author.value,
    Publisher: publisher.value,
    ISBN: isbn.value,
    Remark: remark.value
  }).then((response) => {
    if (response.data.code === 0) {
      prompt.value = "新增荐购记录成功";
      snackbar.value = true;
      loadItems({
        page: 1,
        itemsPerPage: itemsPerPage.value,
        sortBy: sortBy.value
      })
    } else {
      console.error("新增荐购记录失败: " + response.data.message);
      prompt.value = "新增荐购记录失败：" + response.data.message;
      snackbar.value = true;
    }
  }).catch((error) => {
    console.error(error)
    prompt.value = "新增荐购记录失败：" + error;
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
    title: 'ISBN',
    align: 'center',
    sortable: true,
    key: 'isbn',
  },
  {
    title: '提交用户',
    align: 'center',
    sortable: true,
    key: 'userName',
  },
  {
    title: '是否已处理',
    align: 'center',
    sortable: true,
    key: 'isProcessed',
  },
  {
    title: '更多信息',
    align: 'center',
    sortable: true,
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
  const url = `/recommend?pageIndex=${page - 1}&pageSize=${itemsPerPage}&sortColumn=${orderBy}&sortOrder=${order}&filterQuery=${search}`
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
  