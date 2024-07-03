<template>
  <main :class="['grid h-dvh w-dvw overflow-hidden', `grid-${direction}`]">
    <RollingSlider
      v-for="(group, idx) in imageGroups"
      :key="idx"
      :count="count"
      :direction="direction"
      :duration-seconds="durationSeconds"
      :images="group"
      :rolling="
        idx % 2 === 0 ? RollingDirections.backward : RollingDirections.forward
      "
    />
  </main>
  <DateTimeWeatherCombo
    :kind="DateTimeWeatherComboKinds.footer"
    :weather-forecast="weatherForecast"
  />
</template>

<script lang="ts" setup>
import { type Direction, Directions, type Image } from '@components/properties';
import type { IWeatherForecast } from '@/domain/api/homescreen-slideshow-api';
import { computed } from 'vue';
import RollingSlider from '@/components/RollingSlider.vue';
import {
  DateTimeWeatherComboKinds,
  RollingDirections,
} from '@/components/properties';
import DateTimeWeatherCombo from '@/components/DateTimeWeatherCombo.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    weatherForecast: IWeatherForecast;
    direction?: Direction;
    count?: number;
    durationSeconds?: number;
  }>(),
  {
    direction: Directions.horizontal,
    count: 2,
    durationSeconds: 24,
  },
);

const imageGroups = computed(() =>
  Array.from({ length: props.count }).map((_, idx) =>
    props.images.slice(
      (props.images.length / props.count) * idx,
      (props.images.length / props.count) * idx +
        props.images.length / props.count,
    ),
  ),
);
</script>

<style lang="scss" scoped>
.grid-horizontal {
  grid-template-columns: repeat(1, minmax(0, 1fr));
  grid-template-rows: repeat(v-bind(count), minmax(0, 1fr));
}

.grid-vertical {
  grid-template-columns: repeat(v-bind(count), minmax(0, 1fr));
  grid-template-rows: repeat(1, minmax(0, 1fr));
}
</style>
