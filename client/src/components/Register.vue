<template>
  <v-container>
    <v-row>
      <span class="subheader">注册</span>
    </v-row>
    <v-row>
      <v-form class="form" fast-fail @submit.prevent>
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
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                @click:append="showPassword = !showPassword"  />
            </v-col>
          </v-row>
          <v-row>
            <v-col class="icon" cols="1">
              <v-icon icon="mdi-lock"></v-icon>
            </v-col>
            <v-col cols="11">
              <v-text-field v-model="passwordConfirmed" :rules="passwordRule" label="确认密码"
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                @click:append="showPassword = !showPassword" />
            </v-col>
          </v-row>
          <v-row>
            <v-btn :color="getSkyColor()" class="w-25 mx-auto" type="submit" @click="onRegisterSubmit"
              :disabled="submitButtonDisabled" :loading="submitButtonLoading">注册</v-btn>
          </v-row>
        </v-container>
      </v-form>
    </v-row>
    <v-snackbar v-model="snackbar" timeout="5000" rounded="pill" :color="getSkyColor()">
      {{ registerPrompt }}
    </v-snackbar>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'

import { getSkyColor } from '@/plugins/util/color';
import axiosInstance from '@/plugins/util/axiosInstance';

const userID = ref<string>('')
const password = ref<string>('')
const passwordConfirmed = ref<string>('')

const snackbar = ref<boolean>(false)
const registerPrompt = ref<string>('')

const submitButtonDisabled = ref(true)
const submitButtonLoading = ref(false)
const showPassword = ref(false)

const passwordRule = [
  (value: string | any[]) => {
    if (password.value === passwordConfirmed.value) {
      if (password.value !== '') {
        submitButtonDisabled.value = false;
        return true;
      } else {
        submitButtonDisabled.value = true;
        return '请填入信息';
      }
    } else {
      submitButtonDisabled.value = true;
      return '两次输入的密码不一致';
    }
  },
]

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

function onRegisterSubmit() {
  submitButtonLoading.value = true;

  const url = '/Account/Register';
  axiosInstance.post(url, {
    userName: userID.value,
    password: password.value,
  }).then(() => {
    registerPrompt.value = "注册成功，请登录";
    snackbar.value = true
  }).catch((error) => {
    registerPrompt.value = "遇到错误：" + error.response.data.detail;
    snackbar.value = true
    console.error('尝试注册时遇到错误：', error)
  })
  submitButtonLoading.value = false
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
