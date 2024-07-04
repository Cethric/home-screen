<template>
  <picture>
    <source :srcset="fullAvif" />
    <source :srcset="fullWebP" />
    <img
      :src="loading"
      alt="Example media"
      class="h-full min-h-96 w-auto min-w-96 max-w-fit rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
    />
  </picture>
</template>

<script lang="ts" setup>
import { computedAsync, useWindowSize } from '@vueuse/core';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import { type Image } from '@/helpers/component_properties';

const props = defineProps<{
  image: Image;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: number,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();

const { width, height } = useWindowSize();

const loading = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value / 2),
      Math.trunc(height.value / 2),
      20,
      MediaTransformOptionsFormat.Jpeg,
    ),
);
const fullAvif = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value - 200),
      Math.trunc(height.value - 200),
      0,
      MediaTransformOptionsFormat.Avif,
    ),
);
const fullWebP = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value - 200),
      Math.trunc(height.value - 200),
      0,
      MediaTransformOptionsFormat.WebP,
    ),
);
</script>
