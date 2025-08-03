<template>
  <SlideshowContainer
    :active-slideshow="activeSlideshow"
    :forecast="forecast!"
  />
  <div class="fixed bottom-4 left-4 z-50">
    <button
      class="rounded-full bg-stone-400/10 p-4 text-stone-50 backdrop-blur hover:text-stone-200 active:text-stone-300"
      @click="navigateToDashboard"
    >
      <span class="sr-only">Dashboard</span>
      <FontAwesomeIcon :icon="faHome" class="text-4xl" />
    </button>
  </div>
</template>

<script async lang="ts" setup>
import { faHome } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { useBrowserLocation, useIntervalFn } from '@vueuse/core';
import { ref } from 'vue';
import { injectConfig } from '@/domain/client/config';
import { loadWeather } from '@/domain/client/weather';
import { choice } from '@/helpers/random';
import { type Slideshow, Slideshows } from '@/slideshows/properties';
import SlideshowContainer from '@/slideshows/SlideshowContainer.vue';

const location = useBrowserLocation({});
const activeSlideshow = ref<Slideshow>(choice(Object.values(Slideshows)));

const forecast = await loadWeather();

useIntervalFn(
  () => {
    const path = location.value.pathname?.replace('/', '');
    if (Object.values(Slideshows).includes(path as Slideshows)) {
      activeSlideshow.value = path as Slideshow;
    } else {
      activeSlideshow.value = choice(Object.values(Slideshows));
    }
    console.log('Loading slideshow variant', activeSlideshow.value);
  },
  15 * 60 * 1000,
  { immediateCallback: true },
);

const config = injectConfig();

const navigateToDashboard = () => {
  if (config?.dashboardUrl) {
    window.location.assign(config.dashboardUrl);
  }
};
</script>
