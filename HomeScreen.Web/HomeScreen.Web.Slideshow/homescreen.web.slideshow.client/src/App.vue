<template>
  <Suspense>
    <SlideshowContainer :active-slideshow="activeSlideshow" />

    <template #fallback>
      <main class="relative h-dvh w-dvw">
        <LoadingSpinner
          :variant="Variants.primary"
          class="absolute size-full"
        />
      </main>
    </template>
  </Suspense>
  <div class="fixed bottom-0 left-0 z-50">
    <button
      class="m-16 rounded-full bg-stone-800/10 p-4 text-stone-50 backdrop-blur hover:text-stone-200 active:text-stone-300"
      @click="navigateToDashboard"
    >
      <span class="sr-only">Dashboard</span>
      <FontAwesomeIcon :icon="faHome" class="text-4xl" />
    </button>
  </div>
</template>

<script lang="ts" setup>
import SlideshowContainer from '@/slideshows/SlideshowContainer.vue';
import { useBrowserLocation, useIntervalFn } from '@vueuse/core';
import { ref } from 'vue';
import { type Slideshow, Slideshows } from '@/slideshows/properties';
import { choice } from '@/helpers/random';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faHome } from '@fortawesome/free-solid-svg-icons';
import { Variants } from '@/helpers/component_properties';
import LoadingSpinner from '@components/LoadingSpinner.vue';

const location = useBrowserLocation({});
const activeSlideshow = ref<Slideshow>(Slideshows.polaroid_slideshow);

useIntervalFn(
  () => {
    const path = location.value.pathname?.replace('/', '');
    if (Object.values(Slideshows).includes(path as Slideshows)) {
      activeSlideshow.value = path as Slideshow;
    } else {
      activeSlideshow.value = choice(Object.values(Slideshows));
    }
  },
  2 * 60 * 1000,
  { immediateCallback: true },
);

const navigateToDashboard = () => {
  window.location.assign('https://localhost:5174');
};
</script>
