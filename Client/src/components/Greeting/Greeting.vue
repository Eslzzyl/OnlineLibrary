<template>
  <!-- 动态背景：https://www.vantajs.com/-->
  <div class="d-flex justify-center align-center h-screen" ref="vantaRef">
    <!-- rounded: see https://vuetifyjs.com/en/styles/border-radius/#usage -->
    <v-sheet class="d-flex align-center mx-auto ml-16" elevation="10" rounded="xl" style="width: 30%;">
      <v-container>
        <v-row>
          <span class="mx-auto mt-2 header">网上图书馆</span>
        </v-row>
        <v-row>
          <v-tabs v-model="currComponent" :color="skyColor" align-tabs="center">
            <v-tab :value="Login">登录</v-tab>
            <v-tab :value="Register">注册</v-tab>
            <v-tab :value="Theme">主题</v-tab>
          </v-tabs>
          <component :is="currComponent"></component>
        </v-row>
      </v-container>
    </v-sheet>
    <v-spacer></v-spacer>
  </div>
</template>

<script setup lang="ts">
import {onMounted, onBeforeUnmount, ref, shallowRef} from 'vue';
import {useTheme} from 'vuetify';
import * as THREE from 'three';
import GLOBE from "vanta/dist/vanta.globe.min";

import Login from './Login.vue';
import Register from './Register.vue';
import Theme from './Theme.vue';

import {getSkyColor} from "@/plugins/util/color";

const skyColor = getSkyColor();
const theme = useTheme();

const vantaRef = ref(null)
let vantaEffect: any = null

const currComponent = shallowRef<any>()
currComponent.value = Login

let backgroundColor = ref(0);
let lineColor = ref(0);
let lineColor2 = ref(0);

onMounted(() => {
  const preferDark = theme.global.current.value.dark;
  if (preferDark) {
    backgroundColor.value = 0x11182a;
    lineColor.value = 0xdfe0ff;
    lineColor2.value = 0xffffff;
  } else {
    backgroundColor.value = 0xdbdbe3;
    lineColor.value = 0x3e3ef2;
    lineColor2.value = 0x5e8cf5;
  }
  vantaEffect = GLOBE({
    el: vantaRef.value,
    THREE: THREE,
    minHeight: 200.00,
    minWidth: 200.00,
    scale: 1.00,
    scaleMobile: 1.00,
    mouseControls: true,
    touchControls: true,
    gyroControls: false,
    backgroundColor: backgroundColor.value,
    color: lineColor.value,
    color2: lineColor2.value
  })
})

onBeforeUnmount(() => {
  if (vantaEffect) {
    vantaEffect.destroy()
  }
})

</script>

<style scoped>
.header {
  font-size: 2rem;
  font-weight: bold;
}

.sheet {
  width: 35%;
  margin: auto;
  display: flex;
  justify-content: center;
  align-items: center;
  vertical-align: center;
}

.main-container {
  display: flex;
  align-items: center;
  justify-content: center;
  vertical-align: center;
}
</style>
