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
import SlideshowView from '@/slideshows/SlideshowView.vue';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
</script>
