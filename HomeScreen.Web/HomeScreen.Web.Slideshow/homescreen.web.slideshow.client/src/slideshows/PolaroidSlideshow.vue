<template>
  <DateTimeWeatherCombo
    :kind="DateTimeWeatherComboKinds.header"
    :weather-forecast="weatherForecast"
  />
  <main class="h-dvh w-dvw overflow-hidden">
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
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';
import { type Image, Variants } from '@/helpers/component_properties';
import { useIntervalFn } from '@vueuse/core';
import {
  type IWeatherForecast,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import { range, rangeRNG } from '@/helpers/random';
import DateTimeWeatherCombo from '@/components/DateTimeWeatherCombo.vue';
import { DateTimeWeatherComboKinds } from '@/components/properties';
import LoadingSpinner from '@components/LoadingSpinner.vue';
import seedrandom from 'seedrandom';
import PolaroidModal from '@/components/PolaroidModal.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    intervalSeconds?: number;
    visibleCount?: number;
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
    intervalSeconds: 8,
    visibleCount: 40,
  },
);

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

const start = range(0, items.value.length);
const head = ref<number>((start + props.visibleCount) % items.value.length);
const tail = ref<number>(start);
const slice = computed(() =>
  props.images.length > 2
    ? head.value < tail.value
      ? [...items.value.slice(tail.value), ...items.value.slice(0, head.value)]
      : items.value.slice(tail.value, head.value)
    : [],
);

const { pause, resume } = useIntervalFn(() => {
  if (props.images.length > 2) {
    tail.value = (tail.value + 1) % items.value.length;
    head.value = (tail.value + props.visibleCount) % items.value.length;

    const next = (head.value + 1) % items.value.length;
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

    document.createElement('img').src =
      items.value[head.value + (1 % items.value.length)].image.loading;
  }
}, props.intervalSeconds * 1000);
</script>
