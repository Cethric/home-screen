<template>
  <main class="relative h-dvh w-dvw">
    <LoadingSpinner :variant="Variants.primary" class="absolute size-full" />
  </main>
</template>

<script lang="ts" setup>
import { LoadingSpinner, Variants } from '@homescreen/web-components-client';
</script>
