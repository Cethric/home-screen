<template>
  <header class="fixed inset-x-0 top-0 z-50 flex justify-center align-middle">
    <div
      class="w-98 max-w-110 text-ellipsis rounded-b-2xl bg-stone-400/40 pb-4 pl-8 pr-4 pt-8 text-center drop-shadow-md backdrop-blur"
    >
      <h1 class="text-5xl font-extrabold tabular-nums text-neutral-50">
        {{ dayFormat }}
      </h1>
      <h1 class="mt-4 text-5xl font-extrabold tabular-nums text-neutral-50">
        {{ timeFormat }}
      </h1>
    </div>
  </header>
  <main v-if="hasImages" class="h-dvh w-dvw overflow-hidden">
    <transition-group
      class="relative size-full"
      enter-active-class="animate__animated animate__fadeIn"
      leave-active-class="animate__animated animate__fadeOut"
      name="fullscreen-images"
      tag="div"
    >
      <div
        v-for="(imageId, idx) in activeItems"
        v-show="idx === 0"
        :key="imageId"
        class="absolute left-1/2 top-1/2 flex h-dvh w-dvw -translate-x-1/2 -translate-y-1/2 items-center justify-center p-2"
      >
        <PolaroidModal
          :image="images[imageId]"
          @pause="() => pause()"
          @resume="() => resume()"
        />
      </div>
    </transition-group>
  </main>
  <FullscreenMainLoader v-else />
  <footer
    class="fixed inset-x-0 bottom-0 z-50 flex justify-center align-middle"
  >
    <div
      class="w-98 max-w-110 text-ellipsis rounded-t-2xl bg-stone-400/40 pb-4 pl-8 pr-4 pt-8 text-center drop-shadow-md backdrop-blur"
    >
      <p class="text-4xl font-bold text-neutral-50">
        {{ weatherForecast.feelsLikeTemperature }}&deg;C
      </p>
      <p class="mt-3 text-4xl font-bold text-neutral-50">
        {{ weatherForecast.weatherCode }}
      </p>
    </div>
  </footer>
</template>

<script lang="ts" setup>
import {
  type Direction,
  type Image,
  type IWeatherForecast,
  PolaroidModal,
} from '@homescreen/web-common-components';
import { computed, ref, watch } from 'vue';
import { useDateFormat, useIntervalFn, useNow } from '@vueuse/core';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';

const props = withDefaults(
  defineProps<{
    images: Record<Image['id'], Image>;
    intervalSeconds?: number;
    weatherForecast: IWeatherForecast;
    direction?: Direction;
    count?: number;
    total: number;
  }>(),
  {
    count: 1,
    intervalSeconds: 24,
  },
);
const length = computed(() => Object.keys(props.images).length);
const hasImages = computed(() => length.value > 4);

const now = useNow();
const timeFormat = useDateFormat(now, 'HH:mm');
const dayFormat = useDateFormat(now, 'MMMM Do YYYY');

const index = ref<number>(0);
const currentId = ref<Image['id']>();
const nextId = ref<Image['id']>();

watch(hasImages, (val, last) => {
  if (val && val !== last) {
    console.log('Update start images');
    currentId.value = Object.keys(props.images)[index.value];
    nextId.value = Object.keys(props.images)[(index.value + 1) % length.value];
  }
});

const { pause, resume } = useIntervalFn(() => {
  if (hasImages.value) {
    index.value = (index.value + 1) % (length.value ?? 1);
    currentId.value = Object.keys(props.images)[index.value];
    nextId.value = Object.keys(props.images)[(index.value + 1) % length.value];
  }
  console.log('Next image', index.value, currentId.value, nextId.value);
}, props.intervalSeconds * 1000);

const activeItems = computed(() =>
  currentId.value && nextId.value && hasImages.value
    ? [currentId.value, nextId.value]
    : [],
);
</script>
