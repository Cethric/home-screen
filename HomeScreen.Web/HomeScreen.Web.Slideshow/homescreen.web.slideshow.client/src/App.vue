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
import SlideshowView from '@/SlideshowView.vue';
import { loadWeather } from '@/domain/weather';
import { loadMedia } from '@/domain/media';
import { useSeoMeta } from '@vueuse/head';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';

const weatherLoader = loadWeather;
const mediaLoader = loadMedia;

useSeoMeta(
  {
    colorScheme: 'dark light',
    themeColor: { content: '#111827', media: '(prefers-color-scheme: dark)' },
    author: 'Cethric',
    robots: 'noindex, nofollow',
  },
  {},
);
</script>
