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
      <main class="relative h-dvh w-dvw">
        <LoadingSpinner
          :variant="Variants.primary"
          class="absolute size-full"
        />
      </main>
    </template>
  </Suspense>
</template>
<script lang="ts" setup>
import { Variants } from '@/helpers/component_properties';
import LoadingSpinner from '@components/LoadingSpinner.vue';
import SlideshowView from '@/SlideshowView.vue';
import { loadWeather } from '@/domain/weather';
import { loadMedia } from '@/domain/media';
import { useSeoMeta } from '@vueuse/head';

const weatherLoader = loadWeather;
const mediaLoader = loadMedia;

useSeoMeta(
  {
    colorScheme: 'dark light',
    themeColor: { content: '#4285f4', media: '(prefers-color-scheme: dark)' },
    author: 'Cethric',
    robots: 'noindex, nofollow',
  },
  {},
);
</script>
