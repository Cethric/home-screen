<template>
  <Suspense v-if="isReady">
    <component
      :is="slideshows[activeSlideshow]"
      :count="count[activeSlideshow]"
      :direction="Directions.random"
      :images="imageIds"
      :load-image="loadImageCallback"
      :total="total[activeSlideshow]"
      :weather-forecast="forecast"
    />

    <template #fallback>
      <FullscreenMainLoader />
    </template>
  </Suspense>
  <FullscreenMainLoader v-else />
</template>

<script lang="ts" setup>
import { type Slideshow, Slideshows } from './properties';
import {
  Directions,
  type Image,
  type WeatherForecast,
} from '@homescreen/web-common-components';
import { injectMediaApi, loadImageCallback, loadMedia } from '@/domain/media';
import {
  computed,
  defineAsyncComponent,
  nextTick,
  onBeforeUnmount,
  onMounted,
  ref,
} from 'vue';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';
import { choice } from '@/helpers/random';
import { useNProgress } from '@vueuse/integrations';
import { useAsyncState, useIntervalFn } from '@vueuse/core';

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
  [Slideshows.grid_slideshow]: defineAsyncComponent({
    loader: () => import('@/slideshows/GridSlideshow.vue'),
    timeout: 10,
    loadingComponent: FullscreenMainLoader,
  }),
};
const count = {
  [Slideshows.rolling_slideshow]: choice([2, 3, 5]),
  [Slideshows.polaroid_slideshow]: 40,
  [Slideshows.fullscreen_slideshow]: 1,
  [Slideshows.grid_slideshow]: 4,
};
const total = {
  [Slideshows.rolling_slideshow]: 100 * count[Slideshows.rolling_slideshow],
  [Slideshows.polaroid_slideshow]: 200,
  [Slideshows.fullscreen_slideshow]: 50,
  [Slideshows.grid_slideshow]: 100,
};

const props = defineProps<{
  activeSlideshow: Slideshow;
  forecast: WeatherForecast;
}>();

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
    console.log(`Loading ${total[props.activeSlideshow]} images`);
    isLoading.value = true;
    progress.value = 0.0;

    await nextTick(() => {
      images.value = [];
    });
    let loaded = 0;
    for await (const item of loadMedia(
      mediaApi,
      total[props.activeSlideshow],
      signal,
    )) {
      signal?.throwIfAborted();
      if (
        item &&
        item.id !== undefined &&
        item.created !== undefined &&
        item.enabled !== undefined
      ) {
        if (
          images.value.filter((img: Image) => img.id === item.id).length > 0
        ) {
          console.warn('Duplicate key found, skipping', item);
          await nextTick(() => {
            ++loaded;
            progress.value = loaded / total[props.activeSlideshow];
          });
          continue;
        }
        await nextTick(() => {
          images.value.push({
            id: item.id!,
            dateTime: item.created!,
            enabled: item.enabled!,
            location:
              item.location?.name &&
              item.location?.latitude &&
              item.location?.longitude
                ? {
                    name: item.location!.name,
                    latitude: item.location!.latitude,
                    longitude: item.location!.longitude,
                  }
                : undefined,
          } satisfies Image);
          ++loaded;
          progress.value = loaded / total[props.activeSlideshow];
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
