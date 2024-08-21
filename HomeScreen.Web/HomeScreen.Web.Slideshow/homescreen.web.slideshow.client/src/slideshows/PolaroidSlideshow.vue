<template>
  <component
    :is="DateTimeWeatherComboAsync"
    :kind="DateTimeWeatherComboKinds.header"
    :weather-forecast="weatherForecast"
  />
  <main v-if="hasImages" class="h-dvh w-dvw overflow-hidden">
    <transition-group
      :duration="{ enter: 2 * 1000, leave: 3 * 1000 }"
      class="relative"
      enter-active-class="animate__animated animate__stampIn"
      leave-active-class="animate__animated animate__shrinkOut"
      tag="div"
    >
      <PolaroidModal
        v-for="imageId in slice"
        :key="imageId"
        :item="makeItem(images[imageId])"
        :load-image="loadImage"
        @pause="() => pause()"
        @resume="() => resume()"
      />
    </transition-group>
  </main>
  <FullscreenMainLoader v-else />
</template>

<script lang="ts" setup>
import { computed, ref, watch } from 'vue';
import {
  type Direction,
  type Image,
  type LoadImageCallback,
  type IWeatherForecast,
} from '@homescreen/web-common-components';
import { useIntervalFn } from '@vueuse/core';
import { range, rangeRNG } from '@/helpers/random';
import {
  DateTimeWeatherComboKinds,
  type PolaroidImage,
} from '@/components/properties';
import seedrandom from 'seedrandom';
import PolaroidModal from '@/components/PolaroidModal.vue';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';
import { DateTimeWeatherComboAsync } from '@/components/DateTimeWeatherComboAsync';

const props = withDefaults(
  defineProps<{
    images: Record<Image['id'], Image>;
    intervalSeconds?: number;
    weatherForecast: IWeatherForecast;
    direction?: Direction;
    count?: number;
    loadImage: LoadImageCallback;
    total: number;
  }>(),
  {
    intervalSeconds: 8,
    count: 40,
  },
);
const length = computed(() => Object.keys(props.images).length);
const hasImages = computed(
  () => length.value > Math.min(props.count + 10, props.total - 20),
);

const makeItem = (image: Image) => {
  const rng = seedrandom(image.id);
  return {
    image,
    top: rangeRNG(-12.5, 87.5, rng),
    left: rangeRNG(-6.25, 100, rng),
    rotation: rangeRNG(-15, 15, rng),
  } satisfies PolaroidImage;
};

const head = ref<number>(0);
const tail = ref<number>(0);
const slice = ref<Image['id'][]>([]);

const makeSlice = () => {
  const keys = Object.keys(props.images);
  slice.value =
    head.value < tail.value
      ? [...keys.slice(tail.value), ...keys.slice(0, head.value)]
      : keys.slice(tail.value, head.value);
};

watch(hasImages, (val, last) => {
  if (val && val !== last) {
    console.log('Update start images');
    const start = range(0, length.value);
    head.value = (start + props.count) % length.value;
    tail.value = start;
    makeSlice();
  }
});

const { pause, resume } = useIntervalFn(() => {
  if (hasImages.value) {
    tail.value = (tail.value + 1) % length.value;
    head.value = (tail.value + props.count) % length.value;
    makeSlice();
  }
}, props.intervalSeconds * 1000);
</script>
