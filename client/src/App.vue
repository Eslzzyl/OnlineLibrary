<template>
  <v-app>
    <v-main class="h-screen">
      <router-view v-slot="{ Component }">
        <component :is="Component" />
      </router-view>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
</script>
