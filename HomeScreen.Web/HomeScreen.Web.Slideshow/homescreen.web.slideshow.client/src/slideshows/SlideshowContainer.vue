<template>
  <template v-if="images.length > 2">
    <RollingSlideshow
      v-if="activeSlideshow === Slideshows.rolling_slideshow"
      :images="images"
      :load-image="loadImage"
      :weather-forecast="forecast"
    />
    <PolaroidSlideshow
      v-if="activeSlideshow === Slideshows.polaroid_slideshow"
      :images="images"
      :load-image="loadImage"
      :weather-forecast="forecast"
    />
    <FullscreenSlideshow
      v-if="activeSlideshow === Slideshows.fullscreen_slideshow"
      :images="images"
      :load-image="loadImage"
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
import {
  MediaTransformOptionsFormat,
  WeatherForecast,
  WeatherForecastClient,
} from '@/domain/api/homescreen-slideshow-api';
import { type Slideshow, Slideshows } from './properties';
import { type Image, Variants } from '@/helpers/component_properties';
import { nextTick, onBeforeUnmount, onMounted, ref } from 'vue';
import LoadingSpinner from '@components/LoadingSpinner.vue';
import { StreamingMediaApi } from '@/domain/StreamingMediaApi';
import { useAsyncState } from '@vueuse/core';
import { useNProgress } from '@vueuse/integrations';

defineProps<{
  activeSlideshow: Slideshow;
}>();

const weatherApi = new WeatherForecastClient('/api');

let forecast: WeatherForecast;
try {
  const response = await weatherApi.getCurrentForecast(151.20732, -33.86785);
  forecast = response.result;
} catch (e) {
  console.log('Unable to load weather data', e);
  forecast = new WeatherForecast({
    feelsLikeTemperature: 0,
    weatherCode: 'Unable to load weather data',
  });
}

const mediaApi = new StreamingMediaApi();

const images = ref<Image[]>([]);

const abort = new AbortController();
const { isLoading, progress } = useNProgress(0);

const { execute } = useAsyncState(
  async () => {
    images.value = [];
    console.log('Loading images');
    isLoading.value = true;
    progress.value = 0.0;
    for await (let response of mediaApi.getRandomMediaItemsStream(
      300,
      abort.signal,
    )) {
      const item = response.result;
      await nextTick(() => {
        images.value.push({
          id: item.id,
          dateTime: item.created,
          location: {
            name: item.location!.name,
            latitude: item.location!.latitude,
            longitude: item.location!.longitude,
          },
        } satisfies Image);
        progress.value = images.value.length / 300;
      });
    }
    isLoading.value = false;
    console.log('Loaded all images');
  },
  undefined,
  { immediate: false },
);

onMounted(() => {
  execute(0);
});

onBeforeUnmount(() => {
  abort.abort('Navigated away from page');
});

const loadImage = async (
  imageId: string,
  width: number,
  height: number,
  blur: number,
  format: MediaTransformOptionsFormat,
) => {
  const response = await mediaApi.getTransformMediaItemUrl(
    imageId,
    width,
    height,
    blur,
    format,
  );
  return response.headers['location'];
};
</script>
