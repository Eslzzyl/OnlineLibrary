<template>
  <v-container>
    <v-banner sticky style="font-size: 2rem;">个人信息</v-banner>
    <v-container>
      <v-fade-transition>
        <v-alert v-if="isGetInfoErrorHappened" rounded="xl" variant="elevated" elevation="5" class="card">
          加载似乎出了一点问题。尝试刷新页面？<br>{{ requestError }}
        </v-alert>
      </v-fade-transition>

      <v-fab-transition>
        <v-card variant="elevated" elevation="5" class="card" rounded="xl">
          <v-card-title>账号</v-card-title>
          <v-form fast-fail @submit.prevent>
            <v-container>
              <v-row>
                <v-col>
                  用户名：{{ userName }}
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" v-model="userEmail" label="邮箱"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" v-model="userPhone" label="电话"></v-text-field>
                </v-col>
              </v-row>
              <v-row>
                <v-divider></v-divider>
              </v-row>
              <v-row>
                <v-col cols="3">
                  头像：
                  <v-avatar v-if="userAvatar !== ''" :image="userAvatar" size="64"></v-avatar>
                  <span v-if="userAvatar === ''">你似乎还没有头像。试试上传一个！</span>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="4">
                  <v-btn variant="tonal" :loading="isSelectingAvatar" @click="handleAvatarButton">上传新头像</v-btn>
                  <!-- Create a File Input that will be hidden but triggered with JavaScript -->
                  <input v-show="false" ref="avatarSelector" type="file" @change="updateAvatar">
                </v-col>
              </v-row>
              <v-row>
                <v-divider></v-divider>
              </v-row>
              <v-row>
                <v-col>
                  修改密码：
                </v-col>
              </v-row>
              <v-row>
                <v-col>
                  <v-text-field variant="outlined" v-model="newPassword" label="新密码"></v-text-field>
                </v-col>
                <v-col>
                  <v-text-field variant="outlined" v-model="newPasswordConfirmed" label="再次输入以确认密码"
                    :rules="[passwordConfirmRule]"></v-text-field>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card>
      </v-fab-transition>
      <v-btn @click="updateInfo">提交更改</v-btn>
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
import axios from 'axios';
import '@/style.css';

const userName = ref('999999')
const userEmail = ref('')
const userPhone = ref('')
const newPassword = ref('')
const newPasswordConfirmed = ref('')

const userAvatar = ref('')
const isSelectingAvatar = ref(false)
const isAvatarUpdated = ref(false)

const isGetInfoErrorHappened = ref(false)
const isUpdateInfoErrorHappened = ref(false)
const requestError = ref()

const snackbar = ref(false)
const prompt = ref('')

const emit = defineEmits<{
  (event: 'change-avatar', avatar: string): void
}>()

// https://cn.vuejs.org/api/built-in-special-attributes.html#ref
const avatarSelector = ref<HTMLElement>()

const passwordConfirmRule = () => {
    if (newPassword.value === newPasswordConfirmed.value) {
      return true
    } else {
      return '两次输入的密码不一致'
    }
  }

onMounted(() => {
  let username = window.localStorage.getItem("name")
  if (username) {
    userName.value = username
  }
  const url = '/Account/GetInfo';
  axiosInstance.get(url).then((response) => {
    console.log(response)
    if (response.data.code === 0) {
      userEmail.value = response.data.data.email;
      userPhone.value = response.data.data.phoneNumber;
      userAvatar.value = response.data.data.avatar;
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
  const url = '/Account/UpdateInfo';
  axiosInstance.put(url, {
    Email: userEmail.value,
    PhoneNumber: userPhone.value,
    Password: newPassword.value,
    Avatar: userAvatar.value,
  }).then((response) => {
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

// 这个函数仅仅用于唤起文件选择对话框，不承载任何业务逻辑
function handleAvatarButton() {
  isSelectingAvatar.value = true

  // After obtaining the focus when closing the FilePicker, return the button state to normal
  window.addEventListener('focus', () => {
    isSelectingAvatar.value = false
  }, { once: true });

  // Trigger click on the FileInput
  (avatarSelector.value as HTMLElement).click();
}

function updateAvatar(e: Event) {
  isSelectingAvatar.value = true    // 上传时也转圈
  const target = e.target as HTMLInputElement
  if (target !== null && target.files !== null) {
    isAvatarUpdated.value = true
    const selectedAvatar = target.files[0]
    const formData = new FormData();
    formData.append('file', selectedAvatar);

    axios.post("https://img.eslzzyl.eu.org/upload", formData)
      .then((response) => {
        userAvatar.value = response.data
        window.localStorage.setItem("avatar", userAvatar.value)
        prompt.value = "头像上传成功，你仍然需要点击下面的提交按钮来提交更改"
        snackbar.value = true
        console.log("新头像URL：", userAvatar.value)
        isSelectingAvatar.value = false
        emit('change-avatar', userAvatar.value)
      }).catch((error) => {
        console.error('上传头像时遇到问题：', error)
        prompt.value = "上传头像时遇到问题：" + error.message
        snackbar.value = true
        isSelectingAvatar.value = false
        return
      })
  }
}

</script>
