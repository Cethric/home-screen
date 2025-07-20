<template>
  <img
    class="aspect-(--imageAspect) size-auto bg-(--color)"
    :class="{ 'rounded-2xl': rounded }"
    :style="{
      '--imageAspect': aspectRatio,
      '--color': color,
      'width': `${size}px`,
      maxHeight: '85dvh'
    }"
    :alt="alt"
    :src="srcset"
    :width="width"
    :height="height"
    crossorigin="anonymous"
    decoding="async"
    loading="lazy"
    referrerpolicy="strict-origin-when-cross-origin"
  />
</template>

<script async setup lang="ts">
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { computed } from 'vue';
import { loadImage } from '@/helpers/computedMedia.ts';
import type { Image } from '@/helpers/image';

const props = defineProps<
  Image & {
    width: number;
    height: number;
    size: number;
    color: string;
    rounded: boolean;
  }
>();

const srcset = await loadImage(
  props.id,
  Math.trunc(props.width * 0.5 * (window.devicePixelRatio ?? 1)),
  Math.trunc(props.height * 0.5 * (window.devicePixelRatio ?? 1)),
  true,
  MediaTransformOptionsFormat.JPEG,
);

const alt = computed(
  () =>
    props.location?.name ||
    (props.dateTime.isValid
      ? `Taken at ${props.dateTime.toFormat('MMMM Do YYYY HH:mm')}`
      : ''),
);
</script>
