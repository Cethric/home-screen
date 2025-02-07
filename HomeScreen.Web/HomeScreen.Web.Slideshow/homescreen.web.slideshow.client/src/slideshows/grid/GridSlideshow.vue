<template>
  <header class="fixed inset-x-0 top-0 z-50 flex justify-center align-middle">
    <div
      class="w-98 max-w-110 rounded-b-2xl bg-stone-400/40 pt-8 pr-4 pb-4 pl-8 text-center text-ellipsis drop-shadow-md backdrop-blur"
    >
      <h1 class="text-5xl font-extrabold text-neutral-50 tabular-nums">
        {{ dayFormat }}
      </h1>
      <h1 class="mt-4 text-5xl font-extrabold text-neutral-50 tabular-nums">
        {{ timeFormat }}
      </h1>
    </div>
  </header>
  <main
    v-if="hasImages"
    class="flex h-dvh w-dvw justify-evenly gap-8 overflow-hidden portrait:flex-col landscape:flex-row"
  >
    <GridItem
      v-for="offset in 2"
      :key="offset"
      :images="images"
      :length="Math.floor(length / 2)"
      :offset="offset"
    />
  </main>
  <FullscreenMainLoader v-else />
  <footer
    class="fixed inset-x-0 bottom-0 z-50 flex justify-center align-middle"
  >
    <div
      class="w-98 max-w-110 rounded-t-2xl bg-stone-400/40 pt-8 pr-4 pb-4 pl-8 text-center text-ellipsis drop-shadow-md backdrop-blur"
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
  openobserveRum,
  type WeatherForecast,
} from '@homescreen/web-common-components';
import { computed, onBeforeMount } from 'vue';
import { useDateFormat, useNow } from '@vueuse/core';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
import GridItem from '@/slideshows/grid/GridItem.vue';

onBeforeMount(() => {
  openobserveRum.startView('GridSlideshow');
});

const props = withDefaults(
  defineProps<{
    images: Record<Image['id'], Image>;
    intervalSeconds?: number;
    weatherForecast: WeatherForecast;
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
</script>
