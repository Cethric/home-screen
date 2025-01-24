<template>
  <Suspense v-if="isReady">
    <component
      :is="currentSlideshow"
      :count="currentCount"
      :direction="Directions.random"
      :images="imageIds"
      :total="currentTotal"
      :weather-forecast="forecast"
    />

    <template #fallback>
      <FullscreenMainLoader />
    </template>
  </Suspense>
  <FullscreenMainLoader v-else />
</template>

<script lang="ts" setup>
import { type Slideshow } from './properties';
import {
  Directions,
  type Image,
  injectMediaApi,
  transformMediaItemToImage,
  type WeatherForecast,
} from '@homescreen/web-common-components';
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
import { useNProgress } from '@vueuse/integrations';
import { useAsyncState, useIntervalFn } from '@vueuse/core';
import { useCurrentSlideshow } from '@/slideshows/useCurrentSlideshow';

const props = defineProps<{
  activeSlideshow: Slideshow;
  forecast: WeatherForecast;
}>();

const { currentSlideshow, currentTotal, currentCount } = useCurrentSlideshow({
  activeSlideshow: props.activeSlideshow,
});

const images = ref<Image[]>([]);
const imageIds = computed<Record<Image['id'], Image>>(() =>
  images.value.reduce(
    (p: Record<Image['id'], Image>, c: Image) => ({ ...p, [c['id']]: c }),
    {},
  ),
);

const abort = ref<AbortController>(new AbortController());
const { isLoading, progress } = useNProgress(0, {
  trickle: false,
  minimum: 0.0,
  speed: 0,
});

const mediaApi = injectMediaApi();

const { execute, isReady } = useAsyncState(
  async (signal?: AbortSignal) => {
    console.log(`Loading ${currentTotal.value} images`);
    isLoading.value = true;
    progress.value = 0.0;

    await nextTick(() => {
      images.value = [];
    });
    let loaded = 0;
    const media = mediaApi.random(currentTotal.value, signal);

    for await (const item of media) {
      signal?.throwIfAborted();
      if (item && item.id !== undefined) {
        const duplicateIndex = images.value.findIndex(
          (img: Image) => img.id === item.id,
        );
        if (duplicateIndex >= 0) {
          if (images.value.length >= currentTotal.value) {
            const removed = images.value.splice(duplicateIndex, 1);
            console.log('Removed duplicate image', removed);
          } else {
            console.warn('Duplicate image found, skipping', item);
            ++loaded;
            progress.value = loaded / currentTotal.value;
            continue;
          }
        } else if (images.value.length >= currentTotal.value) {
          const removed = images.value.pop();
          console.log('Removed image', removed);
        }
        await nextTick(() => {
          images.value.push(transformMediaItemToImage(item));
          ++loaded;
          progress.value = loaded / currentTotal.value;
        });
      }
    }
    isLoading.value = false;
    console.log('Loaded all images');
  },
  undefined,
  { immediate: false },
);

onMounted(() => {
  execute(0, abort.value.signal);
});

onBeforeUnmount(() => {
  abort.value.abort('Navigated away from page');
});

useIntervalFn(
  () => {
    abort.value.abort('Interval reached');
    abort.value = new AbortController();
    execute(0, abort.value.signal);
  },
  10 * 60 * 1000,
  { immediate: true },
);
</script>
