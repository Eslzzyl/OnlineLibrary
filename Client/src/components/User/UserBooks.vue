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
        </v-row>
        <v-row>
          <v-col>
            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items-length="totalItems"
              :items="tableData" :loading="loading" :search="search" class="elevation-1" item-value="name"
              @update:options="loadItems" loading-text="正在加载数据..." fixed-header height="60vh">
              <template v-slot:item.moreInfo="{ item }">
                <v-btn variant="tonal" @click="borrow(item)">借阅</v-btn>
                <v-btn variant="tonal" @click="comment(item)">评论</v-btn>
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
      <v-dialog v-model="borrowConfirmDialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">确认借阅</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              你确定要借阅这本书吗？
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="borrowConfirmDialog = false;">取消</v-btn>
            <v-btn color="blue-darken-1" variant="text" @click="borrowConfirmDialog = false; borrowConfirmed()">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="borrowLimitDialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">已达到最大借阅数量</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              你已经达到了系统规定的最大借阅数量。如果你需要借阅更多的书籍，请先归还部分书籍。
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="borrowLimitDialog = false;">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="borrowDurationDialog" max-width="40vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">存在超期未还书籍</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              你有书籍超期未还，无法借阅更多书籍。请先归还超期书籍。
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="borrowDurationDialog = false;">确定</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-dialog v-model="commentDialog" max-width="75vw">
        <v-card rounded="xl">
          <v-card-title>
            <span class="text-h5">对《{{ currItem.title }}》的评论</span>
          </v-card-title>
          <v-card-text>
            <div v-if="comments.length === 0">
              <span>暂无评论</span>
            </div>
            <div v-if="comments.length !== 0" style="max-height: 70vh; overflow-y: scroll;">
              <v-row v-for="(item, _) in comments" style="width: 69vw;">
                <v-col cols="1" class="d-flex align-start">
                  <span>#{{ item.index }}</span>
                </v-col>
                <v-col cols="2" class="d-flex align-start">
                  <v-avatar :image="item.userAvatar" size="32" class="mb-4" />
                  <div class="ml-1">
                    <span v-if="item.refCommentIndex === 0">{{ item.userName }} 说：</span>
                    <span v-else>{{ item.userName }} 回复 #{{ item.refCommentIndex }} 说：</span>
                  </div>
                </v-col>
                <v-col cols="4" class="d-flex align-start">
                  <span style="max-width: 35vw;">{{ item.content }}</span>
                </v-col>
                <v-spacer></v-spacer>
                <v-col cols="3" class="d-flex align-start">
                  <v-btn color="blue-darken-1" variant="text" @click="reply(item)">
                    回复<span>#{{ item.index }}</span>
                  </v-btn>
                  <span class="text-body-2 mt-2">{{ item.createTime }}</span>
                </v-col>
              </v-row>
            </div>
            <v-btn variant="tonal" @click="newComment">新增评论</v-btn>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue-darken-1" variant="text" @click="commentDialog = false;">关闭</v-btn>
          </v-card-actions>
        </v-card>
        <v-dialog v-model="newCommentDialog" max-width="50vw">
          <v-card rounded="xl">
            <v-card-title>
              <span class="text-h5">新增评论</span>
            </v-card-title>
            <v-container>
              <span v-if="isReply">回复 {{ userNameReplyTo }}：</span>
              <span v-else>撰写新评论：</span>
              <v-textarea v-model="content" label="评论内容" required></v-textarea>
            </v-container>

            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="blue-darken-1" variant="text" @click="newCommentDialog = false;">取消</v-btn>
              <v-btn color="blue-darken-1" variant="text" @click="newCommentDialog = false; newCommentSubmit()">提交</v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
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

const borrowConfirmDialog = ref(false)
const borrowLimitDialog = ref(false)
const borrowDurationDialog = ref(false)

const currItem = ref(null)

const snackbar = ref(false)
const prompt = ref('')

const commentDialog = ref(false)
const newCommentDialog = ref(false);
const isReply = ref(false);
const userNameReplyTo = ref('');
const refId = ref(0);
const comments = ref([]);
const content = ref('');

function borrow(item) {
  currItem.value = item
  borrowConfirmDialog.value = true
}

async function comment(item) {
  currItem.value = item
  await getComments()
  console.log(comments.value);
  commentDialog.value = true
}

function newComment() {
  isReply.value = false;
  userNameReplyTo.value = '';
  refId.value = 0;
  newCommentDialog.value = true;
}

function reply(item) {
  console.log(item);
  isReply.value = true;
  userNameReplyTo.value = item.userName;
  refId.value = item.id;
  newCommentDialog.value = true;
}

function borrowConfirmed() {
  const url = `/user/borrow?bookId=${currItem.value.id}`;
  axiosInstance.post(url).then((response) => {
    if (response.data.code === 0) {
      prompt.value = "借阅成功";
      snackbar.value = true;
      loadItems({
        page: 1,
        itemsPerPage: itemsPerPage.value,
        sortBy: sortBy.value
      })
    } else if (response.data.code === 2) {
      console.error("借阅失败：已达到最大借阅数量")
      borrowLimitDialog.value = true
    } else if (response.data.code === 3) {
      console.error("借阅失败：存在超期未还书籍")
      borrowDurationDialog.value = true
    } else {
      console.error("借阅失败: " + response);
      prompt.value = "借阅失败：" + response.data.message;
      snackbar.value = true;
    }
  }).catch((error) => {
    console.error(error)
    prompt.value = "借阅失败：" + error;
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

async function getComments() {
  const url = `/bookcomment?bookId=${currItem.value.id}`;
  try {
    const result = await axiosInstance.get(url);
    if (result.data.code === 0) {
      comments.value = result.data.data.comments;
    } else {
      prompt.value = '获取评论失败';
      snackbar.value = true;
    }
  } catch (error) {
    console.error(error)
    prompt.value = '获取评论失败: ' + error.message;
    snackbar.value = true;
  }
}

function newCommentSubmit() {
  const url = '/user/comment';
  axiosInstance.post(url, {
    bookId: currItem.value.id,
    refCommentId: refId.value,
    content: content.value
  }).then((response) => {
    if (response.data.code === 0) {
      prompt.value = '评论成功';
      snackbar.value = true;
      getComments()
    } else {
      prompt.value = '评论失败';
      snackbar.value = true;
    }
  }).catch((error) => {
    console.error(error)
    prompt.value = '评论失败: ' + error.message;
    snackbar.value = true;
  })
}

</script>
