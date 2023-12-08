<template>
  <v-card theme="light">
    <v-layout>
      <v-navigation-drawer rounded="xl" elevation="5">
        <!--如果要加背景图片，就用：-->
        <!-- <v-navigation-drawer image="https://cdn.vuetifyjs.com/images/backgrounds/bg-2.jpg" permanent theme="light"> -->
        <v-sheet color="indigo-lighten-5" class="pa-4" rounded="xl">
          <v-avatar :image="adminAvatar" size="64" class="mb-4"></v-avatar>
          <div>{{ adminName }}</div>
        </v-sheet>

        <v-list nav>
          <v-list-item prepend-icon="mdi-book-search-outline" title="书籍管理" value="admin_books" rounded="xl"
            @click="changeView(AdminBooks);"></v-list-item>
          <v-list-item prepend-icon="mdi-account-cog-outline" title="用户管理" value="admin_users" rounded="xl"
            @click="changeView(AdminUsers);"></v-list-item>
          <v-list-item prepend-icon="mdi-text-box-outline" title="日志管理" value="admin_logs" rounded="xl"
            @click="changeView(AdminLogs);"></v-list-item>
          <v-list-item prepend-icon="mdi-account-tie" title="个人信息" value="admin_account" rounded="xl"
            @click="changeView(AdminAccount);"></v-list-item>
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
              @change-avatar="(avatar: string) => { adminAvatar = avatar; console.log(adminAvatar) }"></component>
          </v-slide-x-transition>
        </v-container>
      </v-main>
    </v-layout>
  </v-card>
</template>

<script setup lang="ts">

import { ref, shallowRef, onMounted } from 'vue';
import { useRouter } from 'vue-router';

import AdminBooks from './AdminBooks.vue';
import AdminUsers from './AdminUsers.vue';
import AdminLogs from './AdminLogs.vue';
import AdminAccount from './AdminAccount.vue';

const router = useRouter()

const currentView = shallowRef()
const adminName = ref<string | undefined>('')
const adminAvatar = ref<string | undefined>('')

function changeView(view: any) {
  currentView.value = view;
}

function logout() {
  window.localStorage.removeItem("name");
  window.localStorage.removeItem("id");
  window.localStorage.removeItem("token");
  window.localStorage.removeItem("avatar");
  router.push('/');
}

onMounted(() => {
  adminName.value = window.localStorage.getItem("name") as string;
  adminAvatar.value = '';
  currentView.value = AdminBooks;
})

</script>
