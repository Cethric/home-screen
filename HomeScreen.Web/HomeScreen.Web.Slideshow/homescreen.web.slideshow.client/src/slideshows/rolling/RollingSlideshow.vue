<template>
  <main
    v-if="hasImages"
    :class="['grid h-dvh w-dvw overflow-hidden', `grid-${actualDirection}`]"
  >
    <RollingSlider
      v-for="group in imageGroups"
      :key="group.id"
      :count="count"
      :direction="actualDirection"
      :duration-seconds="durationSeconds"
      :images="group.images"
      :rolling="group.direction"
    />
  </main>
  <FullscreenMainLoader v-else />
  <DateTimeWeatherComboAsync
    :kind="DateTimeWeatherComboKinds.footer"
    :weather-forecast="weatherForecast"
  />
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  openobserveRum,
  type WeatherForecast,
} from '@homescreen/web-common-components';
import { computed, onBeforeMount } from 'vue';
import RollingSlider from '@/slideshows/rolling/RollingSlider.vue';
import {
  DateTimeWeatherComboKinds,
  RollingDirections,
} from '@/components/properties';
import { v4 as uuid } from 'uuid';
import { choice } from '@/helpers/random';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
import { DateTimeWeatherComboAsync } from '@/components/DateTimeWeatherComboAsync';

onBeforeMount(() => {
  openobserveRum.startView('RollingSlideshow');
});

const props = withDefaults(
  defineProps<{
    images: Record<Image['id'], Image>;
    weatherForecast: WeatherForecast;
    direction?: Direction;
    count?: number;
    durationSeconds?: number;
    total: number;
  }>(),
  {
    direction: Directions.random,
    count: 2,
    durationSeconds: 24,
  },
);

const actualDirection = computed(() =>
  props.direction === Directions.random
    ? choice([Directions.vertical, Directions.horizontal])
    : props.direction,
);

const length = computed(() => Object.keys(props.images).length);
const size = computed(() => Math.ceil(length.value / props.count));
const hasImages = computed(() => length.value >= props.total - 100);

const images = computed(() =>
  hasImages.value ? Object.values(props.images) : [],
);

const imageGroups = computed(() =>
  hasImages.value
    ? Array.from({ length: props.count }).map((_, idx) => ({
        images: images.value.slice(
          idx * size.value,
          idx * size.value + size.value,
        ),
        id: uuid(),
        direction:
          idx % 2 === 0
            ? RollingDirections.backward
            : RollingDirections.forward,
      }))
    : [],
);
</script>
