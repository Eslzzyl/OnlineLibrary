<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">系统设置</v-banner>
    <v-container>
      <v-fade-transition>
        <v-alert v-if="isGetInfoErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          加载似乎出了一点问题。尝试刷新页面？<br>{{ requestError }}
        </v-alert>
      </v-fade-transition>

      <v-fab-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>借阅规则设置</v-card-title>
          <v-form @submit.prevent>
            <v-container>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" v-model="borrowLimit" label="用户借阅册数上限" hint="设为0时，表示不设上限"
                    persistent-hint :rules="isNumberRules"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" v-model="borrowDuration" label="用户借阅时间上限（天）" hint="设为0时，表示不设上限"
                    persistent-hint :rules="isNumberRules"></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-btn type="submit" @click="updateInfo">提交更改</v-btn>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fab-transition>
      <v-fade-transition>
        <v-alert v-if="isUpdateInfoErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          上传数据时似乎出了一点问题。尝试重新上传？<br>{{ requestError }}
        </v-alert>
      </v-fade-transition>
    </v-container>
    <v-snackbar v-model="snackbar" timeout="5000" rounded="pill" color="indigo-lighten-4">
      {{ prompt }}
      <v-btn variant="text" @click="snackbar = false">关闭</v-btn>
    </v-snackbar>
  </v-container>
</template>
    
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axiosInstance from '@/plugins/util/axiosInstance';
import '@/style.css';

const borrowLimit = ref('0')
const borrowDuration = ref('0')

const isGetInfoErrorHappened = ref(false)
const isUpdateInfoErrorHappened = ref(false)
const requestError = ref()

const snackbar = ref(false)
const prompt = ref('')

const isNumberRules = [
  (value: string) => {
    if (/^\d+$/.test(borrowLimit.value)) {
      return true;
    } else {
      return '请输入数字！';
    }
  },
]

onMounted(() => {
  const url = '/admin/settings';
  axiosInstance.get(url).then((response) => {
    console.log(response)
    
    if (response.data.code === 0) {
      borrowLimit.value = response.data.data.borrowLimit;
      borrowDuration.value = response.data.data.borrowDurationDays;
    } else {
      isGetInfoErrorHappened.value = true
      console.log('请求失败！');
      requestError.value = response.data.message
      prompt.value = '请求失败！' + response.data.message;
      snackbar.value = true;
    }
  }).catch((error) => {
    console.log(error)
    requestError.value = error
    isGetInfoErrorHappened.value = true
  })
})

async function updateInfo() {
  const url = `/admin/settings?borrowLimit=${borrowLimit.value}&borrowDurationDays=${borrowDuration.value}`;
  axiosInstance.put(url).then((response) => {
    console.log(response)
    console.log('更新成功');
    prompt.value = '更新成功';
    snackbar.value = true;
  }).catch((error) => {
    console.log(error)
    requestError.value = error
    isUpdateInfoErrorHappened.value = true
  })
}

</script>
    