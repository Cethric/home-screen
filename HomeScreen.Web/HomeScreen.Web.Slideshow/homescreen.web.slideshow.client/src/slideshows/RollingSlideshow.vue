<template>
  <main
    v-if="props.images.length > 290"
    :class="['grid h-dvh w-dvw overflow-hidden', `grid-${direction}`]"
  >
    <RollingSlider
      v-for="(group, idx) in imageGroups"
      :key="idx"
      :count="count"
      :direction="direction"
      :duration-seconds="durationSeconds"
      :images="group"
      :load-image="loadImage"
      :rolling="
        idx % 2 === 0 ? RollingDirections.backward : RollingDirections.forward
      "
    />
  </main>
  <main v-else class="h-dvh w-dvw">
    <LoadingSpinner :variant="Variants.primary" class="absolute size-full" />
  </main>
  <DateTimeWeatherCombo
    :kind="DateTimeWeatherComboKinds.footer"
    :weather-forecast="weatherForecast"
  />
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  Variants,
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
import DateTimeWeatherCombo from '@/components/DateTimeWeatherCombo.vue';
import LoadingSpinner from '@components/LoadingSpinner.vue';

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
      blur: number,
      format: MediaTransformOptionsFormat,
    ) => Promise<string>;
  }>(),
  {
    direction: Directions.horizontal,
    count: 2,
    durationSeconds: 24,
  },
);

const imageGroups = computed(() =>
  props.images.length > 290
    ? Array.from({ length: props.count }).map((_, idx) =>
        props.images.slice(
          (props.images.length / props.count) * idx,
          (props.images.length / props.count) * idx +
            props.images.length / props.count,
        ),
      )
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
