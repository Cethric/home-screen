<template>
  <header class="fixed inset-x-0 top-0 z-50 flex justify-center align-middle">
    <div
      class="w-96 max-w-96 text-ellipsis rounded-b-2xl bg-stone-800/10 pb-4 pl-8 pr-4 pt-8 text-center drop-shadow-md backdrop-blur"
    >
      <h1 class="text-5xl font-extrabold tabular-nums text-neutral-50">
        {{ dayFormat }}
      </h1>
      <h1 class="text-4xl font-extrabold tabular-nums text-neutral-50">
        {{ timeFormat }}
      </h1>
    </div>
  </header>
  <main class="h-dvh w-dvw overflow-hidden">
    <transition-group
      class="relative size-full"
      enter-active-class="animate__animated animate__fadeIn"
      leave-active-class="animate__animated animate__fadeOut"
      name="fullscreen-images"
      tag="div"
    >
      <div
        v-for="image in activeItems"
        v-show="image.id === images[index].id"
        :key="image.id"
        class="absolute left-1/2 top-1/2 flex h-dvh w-dvw -translate-x-1/2 -translate-y-1/2 items-center justify-center p-2"
      >
        <Suspense>
          <template #fallback>
            <div class="relative size-full">
              <LoadingSpinner
                :variant="Variants.primary"
                class="absolute size-full"
              />
            </div>
          </template>
          <FullscreenModal
            :image="image"
            :load-image="loadImage"
            @pause="() => pause()"
            @resume="() => resume()"
          />
        </Suspense>
      </div>
    </transition-group>
  </main>
  <footer
    class="fixed inset-x-0 bottom-0 z-50 flex justify-center align-middle"
  >
    <div
      class="w-96 max-w-96 text-ellipsis rounded-t-2xl bg-stone-800/10 pb-4 pl-8 pr-4 pt-8 text-center drop-shadow-md backdrop-blur"
    >
      <p class="text-4xl font-bold text-neutral-50">
        {{ weatherForecast.feelsLikeTemperature }}&deg;C
      </p>
      <p class="text-4xl font-bold text-neutral-50">
        {{ weatherForecast.weatherCode }}
      </p>
    </div>
  </footer>
</template>

<script lang="ts" setup>
import { type Image, Variants } from '@/helpers/component_properties';
import {
  type IWeatherForecast,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import { computed, ref } from 'vue';
import { useDateFormat, useIntervalFn, useNow } from '@vueuse/core';
import FullscreenModal from '@/components/fullscreen/FullscreenModal.vue';
import LoadingSpinner from '@components/LoadingSpinner.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    intervalSeconds?: number;
    weatherForecast: IWeatherForecast;
    loadImage: (
      imageId: string,
      width: number,
      height: number,
      blur: number,
      format: MediaTransformOptionsFormat,
    ) => Promise<string>;
  }>(),
  {
    intervalSeconds: 24,
  },
);

const now = useNow();
const timeFormat = useDateFormat(now, 'HH:mm');
const dayFormat = useDateFormat(now, 'MMMM Do YYYY');

const index = ref<number>(0);

const { pause, resume } = useIntervalFn(() => {
  if (props.images.length > 2) {
    index.value = (index.value + 1) % (props.images.length ?? 1);
    const next = (index.value + 1) % (props.images.length ?? 1);
    props
      .loadImage(
        props.images[next].id,
        900,
        900,
        0,
        MediaTransformOptionsFormat.Avif,
      )
      .then((src) => {
        const elm = document.createElement('img');
        elm.src = src;
      });
  }
}, props.intervalSeconds * 1000);

const activeItems = computed(() =>
  props.images.length > 2
    ? [
        props.images[index.value],
        props.images[(index.value + 1) % props.images.length],
      ]
    : [],
);
</script>
