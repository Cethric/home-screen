<template>
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
    timeout: 10,
    loadingComponent: FullscreenMainLoader,
  }),
  [Slideshows.polaroid_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/PolaroidSlideshow.vue'),
    timeout: 10,
    loadingComponent: FullscreenMainLoader,
  }),
  [Slideshows.fullscreen_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/FullscreenSlideshow.vue'),
    timeout: 10,
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
  images: Record<Image['id'], Image>;
  total: number;
}>();
</script>
