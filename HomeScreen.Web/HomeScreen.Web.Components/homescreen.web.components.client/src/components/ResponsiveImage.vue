<template>
  <picture>
    <source v-if="fullAvif" :srcset="fullAvif" />
    <source v-if="fullWebP" :srcset="fullWebP" />
    <img
      :alt="
        image.location?.name ||
        `Taken at ${image.dateTime.toFormat('MMMM Do YYYY HH:mm')}`
      "
      :class="$attrs['class']"
      :src="loading"
    />
  </picture>
</template>

<script async lang="ts" setup>
import { type Image } from './properties';
import type {
  ComputedMediaSize,
  LoadImageCallback,
} from '../helpers/computedMedia';
import { computedAsync } from '@vueuse/core';

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  loadingSize: ComputedMediaSize;
  imageSize: ComputedMediaSize;
}>();

const loading = await props.loadImage(
  props.image.id,
  Math.max(props.loadingSize.width, 200),
  Math.max(props.loadingSize.height, 200),
  true,
  'Jpeg',
);

const fullAvif = computedAsync(async (onCancel) => {
  const abortController = new AbortController();
  onCancel(() => abortController.abort('Component unload'));
  return await props.loadImage(
    props.image.id,
    Math.max(props.imageSize.width, 200),
    Math.max(props.imageSize.height, 200),
    false,
    'Avif',
    abortController.signal,
  );
});
const fullWebP = computedAsync(async (onCancel) => {
  const abortController = new AbortController();
  onCancel(() => abortController.abort('Component unload'));
  return await props.loadImage(
    props.image.id,
    Math.max(props.imageSize.width, 200),
    Math.max(props.imageSize.height, 200),
    false,
    'WebP',
    abortController.signal,
  );
});
</script>
