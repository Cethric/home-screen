<template>
  <div
    ref="polaroid"
    :class="[
      'flex size-fit origin-bottom items-center justify-center p-2',
      {
        'flex-col': direction === Directions.vertical,
        'flex-row': direction === Directions.horizontal,
        'rounded-2xl bg-neutral-300 drop-shadow-lg': !flat,
      },
    ]"
    :data-direction="direction"
  >
    <div class="max-h-100 max-w-100 grow">
      <ResponsivePictureAsync
        :image="image"
        :image-size="debouncedSize"
        class="size-full max-h-100 max-w-100 rounded-md object-contain drop-shadow-md"
        @click="onClick"
      />
    </div>
    <div
      :class="[
        'flex w-0 items-center justify-center overflow-y-auto text-balance px-2 py-3',
        {
          'min-w-full text-left': direction === Directions.vertical,
          'h-full min-w-96 text-center': direction === Directions.horizontal,
        },
      ]"
    >
      <div class="flex grow flex-col justify-center gap-2">
        <div class="text-left">
          <p v-if="image.location?.name">
            Location: {{ image.location?.name }}
          </p>
        </div>
        <div class="flex grow flex-col items-center justify-center gap-2">
          <slot :image="image" name="details" />
        </div>
      </div>
    </div>
  </div>
</template>

<script async lang="ts" setup>
import { type Direction, Directions } from '../properties';
import { refDebounced, useElementSize } from '@vueuse/core';
import { computed, ref } from 'vue';
import { ResponsivePictureAsync } from '@/components/ResponsivePicture/ResponsivePictureAsync';
import type { Image } from '@/components/ResponsivePicture/image';

const props = withDefaults(
  defineProps<{
    direction?: Direction;
    image: Image;
    flat?: boolean;
    onClick?: () => void;
  }>(),
  {
    direction: Directions.vertical,
    flat: false,
  },
);

const polaroid = ref<HTMLDivElement>();
const { width, height } = useElementSize(
  polaroid,
  { width: 500, height: 500 },
  {},
);

const imageSize = computed(() => ({
  width: Math.min(
    Math.trunc(width.value),
    props.direction === Directions.horizontal ? 600 : 500,
  ),
  height: Math.min(
    Math.trunc(height.value),
    props.direction === Directions.horizontal ? 600 : 500,
  ),
}));

const debouncedSize = refDebounced(imageSize, 250);
</script>
