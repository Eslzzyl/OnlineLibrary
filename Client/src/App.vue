<template>
  <v-app>
    <v-main class="h-screen">
      <router-view v-slot="{ Component }">
        <transition name="fade">
          <component :is="Component" />
        </transition>
      </router-view>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useTheme } from 'vuetify';

const theme = useTheme();

onMounted(() => {
  // 检测用户操作系统颜色主题
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)')
  theme.global.name.value = prefersDark.matches ? "dark" : "light";
  prefersDark.addEventListener('change', (e) => {
    theme.global.name.value = e.matches ? "dark" : "light";
  })
});

</script>
