<template>
  <component
    :is="DateTimeWeatherComboAsync"
    :kind="DateTimeWeatherComboKinds.header"
    :weather-forecast="weatherForecast"
  />
  <main v-if="hasImages" class="h-dvh w-dvw overflow-hidden">
    <Suspense>
      <template #fallback>
        <div class="relative size-full">
          <LoadingSpinner
            :variant="Variants.primary"
            class="absolute size-full"
          />
        </div>
      </template>
      <transition-group
        :duration="{ enter: 2 * 1000, leave: 3 * 1000 }"
        class="relative"
        enter-active-class="animate__animated animate__stampIn"
        leave-active-class="animate__animated animate__shrinkOut"
        tag="div"
      >
        <PolaroidModal
          v-for="item in slice"
          :key="item.image.id"
          :item="item"
          :load-image="loadImage"
          @pause="() => pause()"
          @resume="() => resume()"
        />
      </transition-group>
    </Suspense>
  </main>
  <FullscreenMainLoader v-else />
</template>

<script lang="ts" setup>
import { computed, ref, watch } from 'vue';
import {
  type Direction,
  type Image,
  Variants,
} from '@/helpers/component_properties';
import { useIntervalFn } from '@vueuse/core';
import {
  type IWeatherForecast,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import { range, rangeRNG } from '@/helpers/random';
import { DateTimeWeatherComboKinds } from '@/components/properties';
import seedrandom from 'seedrandom';
import PolaroidModal from '@/components/PolaroidModal.vue';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';
import { DateTimeWeatherComboAsync } from '@/components/DateTimeWeatherComboAsync';
import LoadingSpinner from '@components/LoadingSpinner.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    intervalSeconds?: number;
    weatherForecast: IWeatherForecast;
    direction?: Direction;
    count?: number;
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
    intervalSeconds: 8,
    count: 40,
  },
);
const hasImages = computed(() => props.images.length > props.total - 20);

const items = computed(() =>
  props.images.map((image) => {
    const rng = seedrandom(image.id);
    return {
      image,
      top: rangeRNG(-12.5, 87.5, rng),
      left: rangeRNG(-6.25, 100, rng),
      rotation: rangeRNG(-15, 15, rng),
    };
  }),
);

const head = ref<number>(0);
const tail = ref<number>(0);
const slice = computed(() =>
  head.value < tail.value
    ? [...items.value.slice(tail.value), ...items.value.slice(0, head.value)]
    : items.value.slice(tail.value, head.value),
);

watch(hasImages, (val, last) => {
  if (val && val !== last) {
    console.log('Update start images');
    const start = range(0, items.value.length);
    head.value = (start + props.count) % items.value.length;
    tail.value = start;
  }
});

const { pause, resume } = useIntervalFn(() => {
  if (hasImages.value) {
    tail.value = (tail.value + 1) % items.value.length;
    head.value = (tail.value + props.count) % items.value.length;
  }
}, props.intervalSeconds * 1000);
</script>
