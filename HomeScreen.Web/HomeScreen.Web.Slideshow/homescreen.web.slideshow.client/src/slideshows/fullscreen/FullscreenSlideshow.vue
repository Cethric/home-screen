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
        class="absolute top-1/2 left-1/2 flex h-dvh w-dvw -translate-x-1/2 -translate-y-1/2 items-center justify-center p-2"
      >
        <PolaroidModal
          :image="images[imageId]"
          :max-size="size"
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
  PolaroidModal,
  type WeatherForecast,
} from "@homescreen/web-common-components";
import { useDateFormat, useIntervalFn, useNow } from "@vueuse/core";
import { computed, ref, watch } from "vue";
import FullscreenMainLoader from "@/slideshows/fullscreen/FullscreenMainLoader.vue";

const size = window.innerWidth * devicePixelRatio;

const props = withDefaults(
  defineProps<{
    images: Record<Image["id"], Image>;
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
const timeFormat = useDateFormat(now, "HH:mm");
const dayFormat = useDateFormat(now, "MMMM Do YYYY");

const index = ref<number>(0);
const currentId = ref<Image["id"]>();
const nextId = ref<Image["id"]>();

watch(hasImages, (val, last) => {
  if (val && val !== last) {
    index.value = 0;
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
}, props.intervalSeconds * 1000);

const activeItems = computed(() =>
  currentId.value && nextId.value && hasImages.value
    ? [currentId.value, nextId.value]
    : [],
);
</script>
