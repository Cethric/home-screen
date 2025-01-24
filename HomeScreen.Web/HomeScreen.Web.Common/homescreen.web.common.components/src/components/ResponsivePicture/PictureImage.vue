<template>
  <slot />
  <img
    :alt="alt"
    :class="$attrs['class']"
    :src="srcset"
    crossorigin="anonymous"
    decoding="async"
    loading="eager"
    referrerpolicy="strict-origin-when-cross-origin"
  />
</template>

<script async lang="ts" setup>
import { loadImage } from '@/helpers/computedMedia';
import { computed, toValue } from 'vue';
import {
  type ComputedMediaSize,
  type Image,
  imageToColour,
} from '@/components/ResponsivePicture/image';
import type { MediaTransformOptionsFormat } from '@/domain/client/media.ts';

const props = withDefaults(
  defineProps<{
    image: Image;
    blur?: boolean;
    imageSize: ComputedMediaSize;
    format: MediaTransformOptionsFormat;
  }>(),
  { blur: false },
);

const { width, height } = toValue(props.imageSize);

const colour = imageToColour(props.image);
const srcset = await loadImage(
  props.image.id,
  Math.trunc(
    Math.max(width * window.devicePixelRatio * (props.blur ? 0.5 : 1), 100),
  ),
  Math.trunc(
    Math.max(height * window.devicePixelRatio * (props.blur ? 0.5 : 1), 100),
  ),
  props.blur,
  props.format,
);

const alt = computed(
  () =>
    props.image.location?.name ||
    `Taken at ${props.image.dateTime.toFormat('MMMM Do YYYY HH:mm')}`,
);
</script>

<style lang="scss" scoped>
img {
  min-width: calc(v-bind(width) * 1px);
  min-height: calc(v-bind(height) * 1px);
  width: calc(v-bind(width) * 1px);
  height: calc(v-bind(height) * 1px);
  background-color: v-bind(colour);
}
</style>
