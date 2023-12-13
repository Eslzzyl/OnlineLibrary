<template>
  <v-card>
    <v-layout>
      <v-navigation-drawer rounded="xl" elevation="5">
        <!--如果要加背景图片，就用：-->
        <!-- <v-navigation-drawer image="https://cdn.vuetifyjs.com/images/backgrounds/bg-2.jpg" permanent theme="light"> -->
        <v-sheet color="profile" class="pa-4" rounded="xl">
          <v-avatar :image="userAvatar" size="64" class="mb-4"></v-avatar>
          <div>{{ userName }}</div>
        </v-sheet>

        <v-list :nav="true">
          <!-- <v-list-subheader v-if="!rail">毕业生去向信息共享系统</v-list-subheader>
          <v-list-subheader v-if="rail"></v-list-subheader> -->
          <v-list-item prepend-icon="mdi-home" title="主页" value="user_home" rounded="xl"
            @click="changeView(UserHome);"></v-list-item>
          <v-list-item prepend-icon="mdi-book-search-outline" title="查看书籍" value="user_books" rounded="xl"
            @click="changeView(UserBooks);"></v-list-item>
          <v-list-item prepend-icon="mdi-book-open-page-variant-outline" title="当前借阅" value="user_current_borrow" rounded="xl"
            @click="changeView(UserCurrentBorrow);"></v-list-item>
          <v-list-item prepend-icon="mdi-book-open-variant" title="借阅历史" value="user_borrow_history" rounded="xl"
            @click="changeView(UserBorrowHistory);"></v-list-item>
          <v-list-item prepend-icon="mdi-account-edit-outline" title="荐购" value="user_recommend" rounded="xl"
            @click="changeView(UserRecommend);"></v-list-item>
          <v-list-item prepend-icon="mdi-account" title="个人信息" value="user_account" rounded="xl"
            @click="changeView(UserAccount);"></v-list-item>
        </v-list>

        <template v-slot:append>
          <div class="pa-2" style="width: 70%; margin: 0 auto;">
            

            <v-dialog width="500">
              <template v-slot:activator="{ props }">
                <v-btn variant="tonal" @click="toggleTheme" class="mr-1">更改主题</v-btn>
                <v-btn v-bind="props" variant="tonal">登出</v-btn>
              </template>

              <template v-slot:default="{ isActive }">
                <v-card title="确认登出">
                  <v-card-text>
                    你确定要登出吗？
                  </v-card-text>

                  <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn text="取消" @click="isActive.value = false;"></v-btn>
                    <v-btn text="确定" @click="isActive.value = false; logout();"></v-btn>
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
            <component :is="currentView" @change-avatar="(avatar: string) => { userAvatar = avatar;}"></component>
          </v-slide-x-transition>
        </v-container>
      </v-main>
    </v-layout>
  </v-card>
</template>

<script setup lang="ts">

import UserHome from './UserHome.vue';
import UserAccount from './UserAccount.vue';
import UserBooks from './UserBooks.vue';
import UserBorrowHistory from './UserBorrowHistory.vue';
import UserCurrentBorrow from './UserCurrentBorrow.vue';
import UserRecommend from './UserRecommend.vue';

import { ref, shallowRef, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useTheme } from 'vuetify'

const theme = useTheme()

function toggleTheme () {
  theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark'
}

// https://router.vuejs.org/zh/guide/advanced/composition-api.html
const router = useRouter()

const currentView = shallowRef()
const userName = ref<string | undefined>('')
const userAvatar = ref<string | undefined>('')

function changeView(view: any) {
  currentView.value = view
}

function logout() {
  window.localStorage.removeItem("name");
  window.localStorage.removeItem("avatar");
  window.localStorage.removeItem("token");
  window.localStorage.removeItem("id");
  router.push('/');
}

onMounted(() => {
  userName.value = window.localStorage.getItem("name") as string;
  userAvatar.value = window.localStorage.getItem("avatar") as string;
  currentView.value = UserHome;
})

</script>
