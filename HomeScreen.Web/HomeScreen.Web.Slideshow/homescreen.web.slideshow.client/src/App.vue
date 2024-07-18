<template>
  <Suspense>
    <template #default>
      <SlideshowView
        :loadMedia="mediaLoader"
        :loadWeather="weatherLoader"
        :total="200"
      />
    </template>
    <template #fallback>
      <FullscreenMainLoader />
    </template>
  </Suspense>
</template>
<script lang="ts" setup>
import SlideshowView from '@/slideshows/SlideshowView.vue';
import { loadWeather } from '@/domain/weather';
import { loadMedia } from '@/domain/media';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';

const weatherLoader = loadWeather;
const mediaLoader = loadMedia;
</script>
