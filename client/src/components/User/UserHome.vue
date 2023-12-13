<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">主页</v-banner>
    <v-card rounded="xl" variant="elevated" elevation="5" class="card">
      <v-container>
        <v-row>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                你在过去一共借阅了<br>
                <span class="big-font ml-5 mr-2">{{ totalHistoryBorrowedBooks }}</span>本书籍
              </div>
            </v-card>
          </v-col>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                你当前借阅了<br>
                <span class="big-font ml-5 mr-2">{{ totalCurrentBorrowedBooks }}</span>本书籍
              </div>
            </v-card>
          </v-col>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                你在过去一个月借阅了<br>
                <span class="big-font ml-5 mr-2">{{ lastMonthBorrowedBooks }}</span>本书籍
              </div>
            </v-card>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <span class="chart-title">借阅书籍分类统计</span>
            <div class="chart">
              <span v-if="isPieEmpty">暂无借阅记录</span>
              <Pie v-if="!isPieEmpty" :data="borrowByClassfication" :options="pieOptions" />
            </div>
          </v-col>
          <v-col>
            <span class="chart-title">过去12个月借阅书籍数量统计</span>
            <div class="chart">
              <span v-if="isPieEmpty">暂无借阅记录</span>
              <Bar v-if="!isPieEmpty" :data="monthlyBorrow" :options="barOptions" />
            </div>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-card color="homecard" rounded="xl" variant="elevated" elevation="2">
              <div class="ml-5 mr-5 mt-5 mb-5">
                你的平均借阅时长为<br>
                <span class="big-font ml-5 mr-2">{{ averageBorrowDuration }}</span>天
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
const lastMonthBorrowedBooks = ref(0)
const monthlyBorrowdBooks = ref([])
const borrowedBooksByClassification = ref({})
const averageBorrowDuration = ref('')
const isPieEmpty = ref(false)
const isBarEmpty = ref(false)

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
  const url = '/user/statistics';
  try {
    const result = await axiosInstance.get(url);
    const data = result.data.data;
    totalHistoryBorrowedBooks.value = data.totalHistoryBorrowedBooks;
    totalCurrentBorrowedBooks.value = data.totalCurrentBorrowedBooks;
    lastMonthBorrowedBooks.value = data.lastMonthBorrowedBooks;
    monthlyBorrowdBooks.value = data.monthlyBorrowedBooks;
    borrowedBooksByClassification.value = data.borrowedBooksByClassification;
    averageBorrowDuration.value = data.averageBorrowDuration;

    formatData();
  } catch (error) {
    requestError.value = error;
    isErrorHappened.value = true;
  }
}

function formatData() {
  const currentTheme = theme.global.name.value == "dark" ? "dark" : "light";

  // 仅当有历史借阅记录时才格式化饼图数据
  if (totalHistoryBorrowedBooks.value != 0) {
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
    isPieEmpty.value = false;
    isBarEmpty.value = false;
  } else {
    isPieEmpty.value = true;
    isBarEmpty.value = true;
  }
}

</script>

<style scoped>
.big-font {
  font-size: 2rem;
}

.chart {
  height: 40vh;
  display: flex;
  justify-content: center;
  align-items: center;
}

.chart-title {
  font-size: 1.5rem;
  margin: 0 auto;
}
</style>