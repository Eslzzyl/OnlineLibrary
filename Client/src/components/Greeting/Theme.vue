<template>
  <v-container>
    <v-row>
      <v-col>
        <v-switch label="切换主题" @click="toggleTheme" inset></v-switch>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">

import { useTheme } from 'vuetify'

const theme = useTheme()

function toggleTheme () {
  theme.global.name.value = theme.global.current.value.dark ? 'light' : 'dark'
}

</script>
