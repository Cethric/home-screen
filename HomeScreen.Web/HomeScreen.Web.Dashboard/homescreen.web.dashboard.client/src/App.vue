<template>
  <NavBar />

  <main class="flex grow flex-col overflow-x-clip overflow-y-auto">
    <router-view />
  </main>
</template>
<script lang="ts" setup>
import NavBar from '@/components/Header/NavBar.vue';
</script>
