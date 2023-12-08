<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">表格</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col cols="3">
            <v-text-field variant="outlined" density="compact" label="检索用户..." v-model="search" clearable></v-text-field>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据...">
              <template v-slot:item.moreInfo="{ item }">
                <v-btn variant="tonal" @click="moreInfo(item)">查看</v-btn> 
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
      <v-dialog v-model="dialog" max-width="80vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">查看详情</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col>
                  姓名：{{ currItem.name }}
                </v-col>
                <v-col>
                  性别：{{ currItem.gender }}
                </v-col>
                <v-col>
                  年级：{{ currItem.grade }}
                </v-col>
                <v-col>
                  院系：{{ currItem.dept }}
                </v-col>
                <v-col>
                  专业：{{ currItem.major }}
                </v-col>
                <v-col>
                  班级：{{ currItem.class }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  是否已毕业：{{ currItem.graduated }}
                </v-col>
                <v-col>
                  去向类型：{{ currItem.goneType }}
                </v-col>
                <v-col>
                  去向单位：{{ currItem.gone }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  QQ：{{ currItem.qq }}
                </v-col>
                <v-col>
                  电话：{{ currItem.phone }}
                </v-col>
                <v-col>
                  微信：{{ currItem.wechat }}
                </v-col>
                <v-col>
                  邮箱：{{ currItem.mail }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  其他联系方式：{{ currItem.others }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  备注：{{ currItem.comments }}
                </v-col>
              </v-row>
            </v-container>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="dialog = false">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-card>
    <v-fade-transition>
      <v-alert v-if="isErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          加载似乎出了一点问题。尝试刷新页面？<br>{{ requestError }}
      </v-alert>
    </v-fade-transition>
  </v-container>
</template>

<script setup>
import axiosInstance from '@/plugins/util/axiosInstance'
import { ref, watch } from 'vue';

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

function moreInfo(item) {
  console.log(item)
  currItem.value = item
  dialog.value = true
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
    title: '索书号',
    align: 'center',
    sortable: true,
    key: 'identifier',
  },
  {
    title: '在馆数量',
    align: 'center',
    sortable: false,
    key: 'inventory',
  },
  {
    title: '操作',
    align: 'center',
    sortable: false,
    key: 'moreInfo',
  },
])

async function request(page, itemsPerPage, sortBy, search) {
  try {
    const response = await axiosInstance.post('/user/table', {
      page: page,
      itemsPerPage: itemsPerPage,
      sortBy: sortBy,
      search: search,
    })
    console.log(response)

    if (response.data.code === 1) {
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
  pageCount.value = result.pageCount
  const packedData = result.data
  loading.value = false
  if (packedData.length !== 0) {
    console.log(packedData)
    tableData.value = packedData
  }
}

</script>

<style scoped>
.card {
  margin: 0 auto;
  margin-bottom: 15px;
  margin-top: 15px;
}
</style>