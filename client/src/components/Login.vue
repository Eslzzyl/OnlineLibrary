<template>
  <v-container>
    <v-row>
      <span class="subheader">登录</span>
    </v-row>
    <v-row>
      <v-form class="form" :fast-fail="true" @submit.prevent>
        <v-container>
          <v-row>
            <v-col class="icon" cols="1">
              <v-icon icon="mdi-account"></v-icon>
            </v-col>
            <v-col cols="11">
              <v-text-field v-model="userID" :rules="notNullRule" label="账号" />
            </v-col>
          </v-row>
          <v-row>
            <v-col class="icon" cols="1">
              <v-icon icon="mdi-lock"></v-icon>
            </v-col>
            <v-col cols="11">
              <v-text-field v-model="password" :rules="notNullRule" label="密码"
                :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                @click:append="showPassword = !showPassword" />
            </v-col>
          </v-row>
          <v-row>
            <v-btn :color="skyColor" class="w-25 mx-auto" type="submit" :disabled="submitButtonDisabled"
              :loading="submitButtonLoading" @click="onLoginSubmit">登录</v-btn>
          </v-row>
        </v-container>
      </v-form>
    </v-row>
    <v-snackbar v-model="snackbar" timeout="5000" rounded="pill" :color="skyColor">
      {{ loginPrompt }}
    </v-snackbar>
  </v-container>
</template>

<script setup lang="ts">

import { ref } from 'vue'
import { useRouter } from 'vue-router';

import { getSkyColor } from '@/plugins/util/color';
import axiosInstance from '@/plugins/util/axiosInstance';

const  skyColor = getSkyColor();

const userID = ref<string>('')
const password = ref<string>('')

const snackbar = ref<boolean>(false)
const loginPrompt = ref<string>('')

const router = useRouter()

const submitButtonDisabled = ref(true)
const submitButtonLoading = ref(false)
const showPassword = ref(false)

const notNullRule = [
  (value: string | any[]) => {
    if (value !== '') {
      submitButtonDisabled.value = false;
      return true;
    } else {
      submitButtonDisabled.value = true;
      return '请填入信息';
    }
  },
]

function onLoginSubmit() {
  submitButtonLoading.value = true;

  const url = '/Account/Login'
  axiosInstance.post(url, {
    account: userID.value,
    password: password.value,
  }).then((response) => {
    const { userName: name, token: token, role: userType, avatar: avatar } = response.data;
    window.localStorage.setItem("name", name);
    window.localStorage.setItem("token", token);
    window.localStorage.setItem("id", userID.value);
    window.localStorage.setItem("avatar", avatar);

    if (userType === 'User') {
      router.push('/user');
    } else if (userType === 'Admin') {
      router.push('/admin');
    } else {
      loginPrompt.value = "暂不支持此用户类型登录：" + userType.value;
      snackbar.value = true;
    }
  }).catch((error) => {
    let message;
    if (typeof error.response !== 'undefined') {   // 后端返回错误的情况
      message = error.response.data.detail
    } else {    // axios 本身遇到错误的情况
      message = error
    }
    loginPrompt.value = "遇到错误：" + message;
    snackbar.value = true;
    console.error('尝试注册时遇到错误：', error);
  })

  submitButtonLoading.value = false;
}

</script>

<style scoped>
.subheader {
  font-size: 1.5rem;
  font-weight: bold;
  margin: 0 auto;
  display: block;
  text-align: center;
}

.form {
  width: 80%;
  margin: 20px auto;
  text-align: center;
}

.icon {
  display: flex;
  margin-top: 15px;
  justify-content: center;
  vertical-align: center;
}
</style>
