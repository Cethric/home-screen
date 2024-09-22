<template>
  <NavBar />

  <main class="flex grow flex-col overflow-y-auto overflow-x-clip">
    <router-view />
  </main>
</template>
<script lang="ts" setup>
import NavBar from '@/components/Header/NavBar.vue';
</script>
