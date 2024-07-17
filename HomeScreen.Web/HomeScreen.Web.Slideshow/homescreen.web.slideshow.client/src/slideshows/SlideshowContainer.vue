<template>
  <template v-if="images.length >= Math.ceil(Math.max(total / 10, 20))">
    <component
      :is="slideshows[activeSlideshow]"
      :count="count[activeSlideshow]"
      :direction="Directions.vertical"
      :images="images"
      :load-image="loadImage"
      :total="total"
      :weather-forecast="forecast"
    />
  </template>
  <template v-else>
    <FullscreenMainLoader />
  </template>
</template>

<script async lang="ts" setup>
import { type Slideshow, Slideshows } from './properties';
import { Directions, type Image } from '@/helpers/component_properties';
import { loadImage } from '@/domain/media';
import type { WeatherForecast } from '@/domain/api/homescreen-slideshow-api';
import { defineAsyncComponent } from 'vue';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';

const slideshows = {
  [Slideshows.rolling_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/RollingSlideshow.vue'),
    loadingComponent: FullscreenMainLoader,
  }),
  [Slideshows.polaroid_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/PolaroidSlideshow.vue'),
    loadingComponent: FullscreenMainLoader,
  }),
  [Slideshows.fullscreen_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/FullscreenSlideshow.vue'),
    loadingComponent: FullscreenMainLoader,
  }),
};
const count = {
  [Slideshows.rolling_slideshow]: 3,
  [Slideshows.polaroid_slideshow]: 40,
  [Slideshows.fullscreen_slideshow]: 1,
};

defineProps<{
  activeSlideshow: Slideshow;
  forecast: WeatherForecast;
  images: Image[];
  total: number;
}>();
</script>
