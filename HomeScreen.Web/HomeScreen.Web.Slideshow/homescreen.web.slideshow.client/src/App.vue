<template>
  <Suspense>
    <template #default>
      <SlideshowView />
    </template>
    <template #fallback>
      <FullscreenMainLoader />
    </template>
  </Suspense>
</template>

<script lang="ts" setup>
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
import SlideshowView from '@/slideshows/SlideshowView.vue';
</script>
