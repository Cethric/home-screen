<template>
  <img
    class="aspect-(--imageAspect)"
    :class="{ 'rounded-2xl': rounded }"
    :style="{
      '--imageAspect': aspectRatio,
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
import { loadImage } from '@/helpers/computedMedia.ts';
import { computed, toValue } from 'vue';
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { type Image } from '@/helpers/image';

const props = defineProps<
  Image & { width: number; height: number; color: string; rounded: boolean }
>();

const srcset = await loadImage(
  props.id,
  Math.trunc(props.width * 0.5 * (window.devicePixelRatio ?? 1)),
  Math.trunc(props.height * 0.5 * (window.devicePixelRatio ?? 1)),
  true,
  MediaTransformOptionsFormat.Jpeg,
);

const alt = computed(
  () =>
    props.location?.name ||
    (props.dateTime.isValid
      ? `Taken at ${props.dateTime.toFormat('MMMM Do YYYY HH:mm')}`
      : ''),
);
</script>

<style scoped lang="css">
img {
  max-width: calc(1px * v-bind(width));
  max-height: calc(1px * v-bind(height));

  width: 100%;
  height: auto;

  background-color: v-bind(color);
}
</style>
