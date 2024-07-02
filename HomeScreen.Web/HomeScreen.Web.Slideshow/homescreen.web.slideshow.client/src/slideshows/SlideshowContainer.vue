<template>
  <RollingSlideshow
    v-if="activeSlideshow === Slideshows.rolling_slideshow"
    :images="images"
    :weather-forecast="forecast"
  />
  <PolaroidSlideshow
    v-if="activeSlideshow === Slideshows.polaroid_slideshow"
    :images="images"
    :weather-forecast="forecast"
  />
  <FullscreenSlideshow
    v-if="activeSlideshow === Slideshows.fullscreen_slideshow"
    :images="images"
    :weather-forecast="forecast"
  />
</template>

<script async lang="ts" setup>
import { DateTime } from 'luxon';
import RollingSlideshow from '@/slideshows/RollingSlideshow.vue';
import PolaroidSlideshow from '@/slideshows/PolaroidSlideshow.vue';
import FullscreenSlideshow from '@/slideshows/FullscreenSlideshow.vue';
import {
  WeatherForecast,
  WeatherForecastClient,
} from '@/domain/api/homescreen-slideshow-api';
import { type Slideshow, Slideshows } from './properties';
import { v4 as uuid } from 'uuid';

defineProps<{
  activeSlideshow: Slideshow;
}>();

const images = Array.from({ length: 300 }).map((_, i) => ({
  id: uuid(),
  src: [`https://picsum.photos/id/${i}/400/500`],
  loading: `https://picsum.photos/id/${i}/400/500/?blur=2`,
  width: 400,
  height: 500,
  dateTime: DateTime.now(),
  location: { name: 'Sydney', latitude: 151.20732, longitude: -33.86785 },
}));
const weatherApi = new WeatherForecastClient('/api');
let forecast: WeatherForecast;
try {
  forecast = (await weatherApi.getCurrentForecast(151.20732, -33.86785)).result;
} catch (e) {
  console.log('Unable to load weather data', e);
  forecast = new WeatherForecast({
    feelsLikeTemperature: 0,
    weatherCode: 'Unable to load weather data',
  });
}
</script>
