<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">主页</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                共有<br>
                <span class="big-font ml-5 mr-2">{{ totalHistoryBorrowedBooks }}</span>条借阅历史记录
              </div>
            </v-card>
          </v-col>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                共有<br>
                <span class="big-font ml-5 mr-2">{{ totalCurrentBorrowedBooks }}</span>条当前借阅记录
              </div>
            </v-card>
          </v-col>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                共有<br>
                <span class="big-font ml-5 mr-2">{{ totalUsers }}</span>位用户
              </div>
            </v-card>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <span class="chart-title">借阅书籍分类统计</span>
            <div class="chart">
              <Pie :data="borrowByClassfication" :options="pieOptions" />
            </div>
          </v-col>
          <v-col>
            <span class="chart-title">过去12个月借阅书籍数量统计</span>
            <div class="chart">
              <Bar :data="monthlyBorrow" :options="barOptions" />
            </div>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                用户的平均借阅时长为<br>
                <span class="big-font ml-5 mr-2">{{ averageBorrowDuration }}</span>天
              </div>
            </v-card>
          </v-col>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                共有<br>
                <span class="big-font ml-5 mr-2">{{ unhandledRecommends }}</span>条未处理的推荐
              </div>
            </v-card>
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
  
<script setup lang="ts">
import axiosInstance from '@/plugins/util/axiosInstance'
import { ref, onMounted } from 'vue';
import { useTheme } from 'vuetify';
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  ChartData,
  ChartOptions,
  BarElement,
  CategoryScale,
  LinearScale,
} from 'chart.js'
import { Pie, Bar } from 'vue-chartjs'
import { getSkyColor, getRandomColorList } from '@/plugins/util/color';
import '@/style.css';

ChartJS.register(ArcElement, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

const skyColor = getSkyColor();
const requestError = ref()
const isErrorHappened = ref(false)
const snackbar = ref(false)
const prompt = ref('')

const totalHistoryBorrowedBooks = ref(0)
const totalCurrentBorrowedBooks = ref(0)
const totalUsers = ref(0);
const monthlyBorrowdBooks = ref([])
const borrowedBooksByClassification = ref({})
const averageBorrowDuration = ref('')
const unhandledRecommends = ref(0)

const theme = useTheme();

const borrowByClassfication = ref<ChartData<'pie'>>({
  labels: [],
  datasets: [
    {
      backgroundColor: [],
      data: [],
    },
  ],
})

const monthlyBorrow = ref<ChartData<'bar'>>({
  labels: [],
  datasets: [
    {
      backgroundColor: [],
      data: [],
    },
  ],
})

// 不知道为什么标题显示不出来，暂时在表格外面加个手动标题
const pieOptions: ChartOptions<'pie'> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    title: {
      display: true,
      text: '借阅书籍分类统计'
    }
  }
};

// 不知道为什么标题显示不出来，暂时在表格外面加个手动标题
const barOptions: ChartOptions<'bar'> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    title: {
      display: true,
      text: '每月借阅书籍数量统计'
    },
    legend: {
      display: false
    }
  }
};

onMounted(() => {
  request();
})

async function request() {
  const url = '/admin/statistics';
  try {
    const result = await axiosInstance.get(url);
    const data = result.data.data;
    totalHistoryBorrowedBooks.value = data.totalHistoryBorrowedBooks;
    totalCurrentBorrowedBooks.value = data.totalCurrentBorrowedBooks;
    totalUsers.value = data.totalUsers;
    monthlyBorrowdBooks.value = data.monthlyBorrowedBooks;
    borrowedBooksByClassification.value = data.borrowedBooksByClassification;
    averageBorrowDuration.value = data.averageBorrowDuration;
    unhandledRecommends.value = data.unhandledRecommends;

    formatData();
  } catch (error) {
    requestError.value = error;
    isErrorHappened.value = true;
  }
}

function formatData() {
  const currentTheme = theme.global.name.value == "dark" ? "dark" : "light";

  borrowByClassfication.value = {
    labels: Object.keys(borrowedBooksByClassification.value),
    datasets: [
      {
        backgroundColor: getRandomColorList(Object.keys(borrowedBooksByClassification.value).length, currentTheme),
        data: Object.values(borrowedBooksByClassification.value),
      },
    ],
  };

  monthlyBorrow.value = {
    labels: Object.keys(monthlyBorrowdBooks.value),
    datasets: [
      {
        backgroundColor: getRandomColorList(Object.keys(monthlyBorrowdBooks.value).length, currentTheme),
        data: Object.values(monthlyBorrowdBooks.value),
      },
    ],
  };
}

</script>
  
<style scoped>
.big-font {
  font-size: 2rem;
}

.chart {
  height: 40vh;
}

.chart-title {
  font-size: 1.5rem;
  margin: 0 auto;
}
</style>