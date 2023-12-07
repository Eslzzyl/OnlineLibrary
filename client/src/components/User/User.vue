<template>
  <v-card theme="light">
    <v-layout>
      <v-navigation-drawer rounded="xl" elevation="5">
        <!--如果要加背景图片，就用：-->
        <!-- <v-navigation-drawer image="https://cdn.vuetifyjs.com/images/backgrounds/bg-2.jpg" permanent theme="light"> -->
        <v-sheet color="indigo-lighten-5" class="pa-4" rounded="xl">
          <v-avatar :image="studentAvatar !== '' ? studentAvatar : '/avatar/chino.jpg'" size="64" class="mb-4"></v-avatar>
          <div>{{ studentName }}</div>
        </v-sheet>

        <v-list nav>
          <!-- <v-list-subheader v-if="!rail">毕业生去向信息共享系统</v-list-subheader>
          <v-list-subheader v-if="rail"></v-list-subheader> -->
          <v-list-item prepend-icon="mdi-home" title="主页" value="home" rounded="xl"
            @click="changeView(Home);"></v-list-item>
          <v-list-item prepend-icon="mdi-table" title="表格" value="table" rounded="xl"
            @click="changeView(Table);"></v-list-item>
          <v-list-item prepend-icon="mdi-account" title="个人信息" value="user_account" rounded="xl"
            @click="changeView(UserAccount);"></v-list-item>
        </v-list>

        <template v-slot:append>
          <div class="pa-2" style="width: 70%; margin: 0 auto;">
            <v-dialog width="500">
              <template v-slot:activator="{ props }">
                <v-btn block v-bind="props" variant="tonal">登出</v-btn>
              </template>

              <template v-slot:default="{ isActive }">
                <v-card title="确认登出">
                  <v-card-text>
                    你确定要登出吗？
                  </v-card-text>

                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn text="取消" @click="isActive.value = false;"></v-btn>
                    <v-btn text="确定" @click="isActive.value = false; logout()"></v-btn>
                  </v-card-actions>
                </v-card>
              </template>
            </v-dialog>
          </div>
        </template>

      </v-navigation-drawer>
      <v-main style="height: 100vh; overflow-y: auto;">
        <v-container>
          <v-slide-x-transition>
            <component :is="currentView" @change-avatar="(avatar: string) => { studentAvatar = avatar; console.log(studentAvatar) }"></component>
          </v-slide-x-transition>
        </v-container>
      </v-main>
    </v-layout>
  </v-card>
</template>

<script setup lang="ts">

import Home from './Home.vue'
import UserAccount from './UserAccount.vue'
import Table from './Table.vue'

import { ref, shallowRef, onMounted } from 'vue'
import { useRouter } from 'vue-router';

// https://router.vuejs.org/zh/guide/advanced/composition-api.html
const router = useRouter()

const currentView = shallowRef()
const studentName = ref<string | undefined>('')
const studentAvatar = ref<string | undefined>('')

function changeView(view: any) {
  currentView.value = view
}

function logout() {
  window.localStorage.removeItem("name")
  window.localStorage.removeItem("avatar")
  window.localStorage.removeItem("token")
  router.push('/')
}

onMounted(() => {
  studentName.value = window.localStorage.getItem("name") as string;
  studentAvatar.value = window.localStorage.getItem("avatar") as string;
  currentView.value = Home
})

</script>
