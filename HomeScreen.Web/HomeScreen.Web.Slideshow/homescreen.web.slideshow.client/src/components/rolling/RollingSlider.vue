<template>
  <div
    ref="slider"
    :class="[
      'm-0',
      {
        'min-w-dvw flex flex-row px-1 py-2 first:pt-4 last:pb-4':
          direction === Directions.horizontal,
        'flex h-dvh flex-col flex-nowrap content-between self-center px-2 py-1 first:pl-4 last:pr-4':
          direction === Directions.vertical,
      },
    ]"
    :style="{ transform }"
  >
    <RollingSlide
      :direction="direction"
      :images="images"
      :load-image="loadImage"
      :image-size="imageSize"
      @pause="() => pause()"
      @resume="() => resume()"
    />

    <RollingSlide
      :direction="direction"
      :images="images"
      :load-image="loadImage"
      :image-size="imageSize"
      @pause="() => pause()"
      @resume="() => resume()"
    />

    <RollingSlide
      :direction="direction"
      :images="images"
      :load-image="loadImage"
      :image-size="imageSize"
      @pause="() => pause()"
      @resume="() => resume()"
    />
  </div>
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  type LoadImageCallback,
} from '@homescreen/web-common-components';
import {
  type RollingDirection,
  RollingDirections,
} from '@/components/properties';
import { computed, ref } from 'vue';
import { useElementSize, useRafFn } from '@vueuse/core';
import { reactiveTransform } from '@vueuse/motion';
import { range } from '@/helpers/random';
import RollingSlide from '@/components/rolling/RollingSlide.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    durationSeconds?: number;
    direction?: Direction;
    rolling: RollingDirection;
    loadImage: LoadImageCallback;
  }>(),
  {
    direction: Directions.horizontal,
    durationSeconds: 24,
  },
);

const slider = ref<HTMLDivElement>();
const progress = ref<number>(range(0, 2));

const { width, height } = useElementSize(slider);

const { state, transform } = reactiveTransform({
  translateX: 0,
  translateY: 0,
});

const { pause, resume } = useRafFn(
  ({ delta }) => {
    switch (props.rolling) {
      case RollingDirections.forward:
        progress.value += 0.005 * (delta * 0.001);
        progress.value = progress.value % 2;
        break;
      case RollingDirections.backward:
        progress.value -= 0.005 * (delta * 0.001);
        if (progress.value < 0) {
          progress.value = 2;
        }
        break;
    }

    switch (props.direction) {
      case Directions.horizontal:
        state.translateX = width.value * progress.value * -1;
        break;
      case Directions.vertical:
        state.translateY = height.value * progress.value * -1;
        break;
    }
  },
  { fpsLimit: 24, immediate: true },
);

const imageSize = computed(() => ({
  width: Math.floor(
    props.direction === Directions.vertical ? width.value / 4 : width.value / 2,
  ),
  height: Math.floor(
    props.direction === Directions.vertical
      ? height.value / 4
      : height.value / 2,
  ),
}));
</script>
