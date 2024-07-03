<template>
  <component
    :is="activeComponent"
    :images="images"
    :weather-forecast="forecast"
  />
  <!--  <RollingSlideshow-->
  <!--    v-if="activeSlideshow === Slideshows.rolling_slideshow"-->
  <!--    :images="images"-->
  <!--    :weather-forecast="forecast"-->
  <!--  />-->
  <!--  <PolaroidSlideshow-->
  <!--    v-if="activeSlideshow === Slideshows.polaroid_slideshow"-->
  <!--    :images="images"-->
  <!--    :weather-forecast="forecast"-->
  <!--  />-->
  <!--  <FullscreenSlideshow-->
  <!--    v-if="activeSlideshow === Slideshows.fullscreen_slideshow"-->
  <!--    :images="images"-->
  <!--    :weather-forecast="forecast"-->
  <!--  />-->
</template>

<script async lang="ts" setup>
import { DateTime } from 'luxon';
import {
  MediaClient,
  WeatherForecast,
  WeatherForecastClient,
} from '@/domain/api/homescreen-slideshow-api';
import { type Slideshow, Slideshows } from './properties';
import { Image } from '@components/properties';
import { v4 as uuid } from 'uuid';
import { computed } from 'vue';

const props = defineProps<{
  activeSlideshow: Slideshow;
}>();

const rollingComponent = async () =>
  await import('@/slideshows/RollingSlideshow.vue');
const polaroidComponent = async () =>
  await import('@/slideshows/PolaroidSlideshow.vue');
const fullscreenComponent = async () =>
  await import('@/slideshows/FullscreenSlideshow.vue');

const activeComponent = computed(() => {
  switch (props.activeSlideshow) {
    case Slideshows.rolling_slideshow:
      return rollingComponent;
    case Slideshows.polaroid_slideshow:
      return polaroidComponent;
    case Slideshows.fullscreen_slideshow:
      return fullscreenComponent;
  }
  return null;
});

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

const mediaApi = new MediaClient('/api');

let images: Image[];
try {
  const response = await mediaApi.getRandomMediaItems(1);
  images = response.result.map(
    (item) =>
      ({
        id: item.id,
        dateTime: item.created,
        location: {
          name: item.location!.name,
          latitude: item.location!.latitude,
          longitude: item.location!.longitude,
        },
      }) satisfies Image,
  );
} catch (e) {
  console.error('Unable to load media items', e);
  images = Array.from({ length: 300 }).map(
    (_, i) =>
      ({
        id: uuid(),
        images: [`https://picsum.photos/id/${i}/400/500`],
        thumbnail: `https://picsum.photos/id/${i}/400/500/?blur=2`,
        width: 400,
        height: 500,
        dateTime: DateTime.now(),
        location: { name: 'Sydney', latitude: 151.20732, longitude: -33.86785 },
      }) satisfies Image,
  );
}
</script>
