<template>
  <v-card theme="light">
    <v-layout>
      <v-navigation-drawer rounded="xl" elevation="5">
        <!--如果要加背景图片，就用：-->
        <!-- <v-navigation-drawer image="https://cdn.vuetifyjs.com/images/backgrounds/bg-2.jpg" permanent theme="light"> -->
        <v-sheet color="indigo-lighten-5" class="pa-4" rounded="xl">
          <v-avatar :image="managerAvatar !== '' ? managerAvatar : '/avatar/alice.jpg'" size="64" class="mb-4"></v-avatar>
          <div>{{ managerName }}</div>
        </v-sheet>

        <v-list nav>
          <v-list-item prepend-icon="mdi-table" title="书籍管理" value="users" rounded="xl"
            @click="changeView(ManageBooks);"></v-list-item>
          <v-list-item prepend-icon="mdi-table" title="用户管理" value="users" rounded="xl"
            @click="changeView(ManageUsers);"></v-list-item>
          <v-list-item prepend-icon="mdi-table" title="日志管理" value="users" rounded="xl"
            @click="changeView(Logs);"></v-list-item>
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
            <component :is="currentView"
              @change-avatar="(avatar: string) => { managerAvatar = avatar; console.log(managerAvatar) }"></component>
          </v-slide-x-transition>
        </v-container>
      </v-main>
    </v-layout>
  </v-card>
</template>

<script setup lang="ts">

import { ref, shallowRef, onMounted } from 'vue'
import { useRouter } from 'vue-router'

import ManageBooks from './ManageBooks.vue'
import ManageUsers from './ManageUsers.vue'
import Logs from './Logs.vue'

const router = useRouter()

const currentView = shallowRef()
const managerName = ref<string | undefined>('')
const managerAvatar = ref<string | undefined>('')

function changeView(view: any) {
  currentView.value = view;
}

function logout() {
  window.localStorage.removeItem("name");
  window.localStorage.removeItem("id");
  window.localStorage.removeItem("token");
}

onMounted(() => {
  managerName.value = window.localStorage.getItem("name") as string;
  managerAvatar.value = '';
  currentView.value = ManageBooks;
})

</script>
