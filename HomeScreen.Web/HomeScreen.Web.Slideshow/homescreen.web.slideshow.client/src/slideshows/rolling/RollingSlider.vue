<template>
  <div
    ref="slider"
    :class="[
      'm-0',
      {
        'flex min-w-dvw flex-row px-1 py-2 first:pt-4 last:pb-4':
          direction === Directions.horizontal,
        'flex h-dvh flex-col flex-nowrap content-between self-center px-2 py-1 first:pl-4 last:pr-4':
          direction === Directions.vertical,
      },
    ]"
    :style="{ transform }"
  >
    <RollingSlide
      :key="`first-${imageSize}`"
      :direction="direction"
      :image-size="imageSize"
      :images="images.slice(0, images.length)"
      @pause="() => pause()"
      @resume="() => resume()"
    />

    <RollingSlide
      :key="`second-${imageSize}`"
      :direction="direction"
      :image-size="imageSize"
      :images="images"
      @pause="() => pause()"
      @resume="() => resume()"
    />

    <RollingSlide
      :key="`third-${imageSize}`"
      :direction="direction"
      :image-size="imageSize"
      :images="images.slice(images.length)"
      @pause="() => pause()"
      @resume="() => resume()"
    />
  </div>
</template>

<script lang="ts" setup>
import { type Direction, Directions, type Image } from '@homescreen/web-common-components';
import { useElementSize, useRafFn, useWindowSize } from '@vueuse/core';
import { reactiveTransform } from '@vueuse/motion';
import { computed, ref, toValue } from 'vue';
import { type RollingDirection, RollingDirections } from '@/components/properties';
import { range } from '@/helpers/random';
import RollingSlide from '@/slideshows/rolling/RollingSlide.vue';

const props = withDefaults(
  defineProps<{
    images: Image[];
    durationSeconds?: number;
    direction: Direction;
    rolling: RollingDirection;
    count: number;
  }>(),
  {
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

function wrap(value: number, upperBound: number, lowerBound: number) {
  const range = upperBound - lowerBound + 1;
  return ((((value - lowerBound) % range) + range) % range) + lowerBound;
}

const speed = computed(() => {
  let offset = 0;
  switch (props.rolling) {
    case RollingDirections.forward:
      offset = 1;
      break;
    case RollingDirections.backward:
      offset = -1;
      break;
  }

  switch (props.direction) {
    case Directions.horizontal:
      return width.value * 1e-9 * offset;
    case Directions.vertical:
      return height.value * 2e-9 * offset;
  }
  return 1e-7;
});

const { pause, resume } = useRafFn(
  ({ delta }) => {
    progress.value = wrap(progress.value + speed.value * delta, 0, 1.5);

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

const windowSize = useWindowSize();
const imageSize = computed(() => {
  const windowWidth = toValue(windowSize.width);
  return (windowWidth / props.count) * 0.95;
});
</script>
