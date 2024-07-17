<template>
  <main
    v-if="hasImages"
    :class="['grid h-dvh w-dvw overflow-hidden', `grid-${direction}`]"
  >
    <RollingSlider
      v-for="group in imageGroups"
      :key="group.id"
      :count="count"
      :direction="direction"
      :duration-seconds="durationSeconds"
      :images="group.images"
      :load-image="loadImage"
      :rolling="group.direction"
    />
  </main>
  <FullscreenMainLoader v-else />
  <component
    :is="DateTimeWeatherComboAsync"
    :kind="DateTimeWeatherComboKinds.footer"
    :weather-forecast="weatherForecast"
  />
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
} from '@/helpers/component_properties';
import {
  type IWeatherForecast,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import { computed } from 'vue';
import RollingSlider from '@/components/rolling/RollingSlider.vue';
import {
  DateTimeWeatherComboKinds,
  RollingDirections,
} from '@/components/properties';
import { v4 as uuid } from 'uuid';
import { range } from '@/helpers/random';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';
import { DateTimeWeatherComboAsync } from '@/components/DateTimeWeatherComboAsync';

const props = withDefaults(
  defineProps<{
    images: Image[];
    weatherForecast: IWeatherForecast;
    direction?: Direction;
    count?: number;
    durationSeconds?: number;
    loadImage: (
      imageId: string,
      width: number,
      height: number,
      blur: boolean,
      format: MediaTransformOptionsFormat,
    ) => Promise<string>;
    total: number;
  }>(),
  {
    direction: Directions.horizontal,
    count: 2,
    durationSeconds: 24,
  },
);

const hasImages = computed(() => props.images.length > props.total - 20);

const images = computed(() =>
  hasImages.value ? props.images.slice(range(0, props.images.length), 75) : [],
);

const imageGroups = computed(() =>
  hasImages.value
    ? Array.from({ length: props.count }).map((_, idx) => ({
        images: images.value.slice(
          (images.value.length / props.count) * idx,
          (images.value.length / props.count) * idx +
            images.value.length / props.count,
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
