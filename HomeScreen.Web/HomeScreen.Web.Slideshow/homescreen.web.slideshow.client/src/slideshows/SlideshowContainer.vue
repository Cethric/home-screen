<template>
  <template v-if="images.length >= 60">
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
    <main class="relative h-dvh w-dvw">
      <LoadingSpinner :variant="Variants.primary" class="absolute size-full" />
    </main>
  </template>
</template>

<script async lang="ts" setup>
import RollingSlideshow from '@/slideshows/RollingSlideshow.vue';
import PolaroidSlideshow from '@/slideshows/PolaroidSlideshow.vue';
import FullscreenSlideshow from '@/slideshows/FullscreenSlideshow.vue';
import { type Slideshow, Slideshows } from './properties';
import {
  Directions,
  type Image,
  Variants,
} from '@/helpers/component_properties';
import LoadingSpinner from '@components/LoadingSpinner.vue';
import { loadImage } from '@/domain/media';
import type { WeatherForecast } from '@/domain/api/homescreen-slideshow-api';

const slideshows = {
  [Slideshows.rolling_slideshow]: RollingSlideshow,
  [Slideshows.polaroid_slideshow]: PolaroidSlideshow,
  [Slideshows.fullscreen_slideshow]: FullscreenSlideshow,
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
