<template>
  <main class="relative h-dvh w-dvw">
    <LoadingSpinner :variant="Variants.primary" class="absolute size-full" />
  </main>
</template>

<script lang="ts" setup>
import { Variants } from '@/helpers/component_properties';
import LoadingSpinner from '@components/LoadingSpinner.vue';
</script>
