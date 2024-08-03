<template>
  <picture>
    <source v-if="fullAvif" :srcset="fullAvif" type="image/avif" />
    <source v-if="fullJxl" :srcset="fullJxl" type="image/jxl" />
    <source v-if="fullWebP" :srcset="fullWebP" type="image/webp" />
    <source v-if="fullPng" :srcset="fullPng" type="image/png" />
    <source v-if="fullJpeg" :srcset="fullJpeg" type="image/jpeg" />
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
import {
  type ComputedMediaSize,
  type LoadImageCallback,
  responsiveImageLoader,
} from '../helpers/computedMedia';
import { useMemoize } from '@vueuse/core';

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
}>();

const loadingImage = useMemoize(
  async (imageId: string, width: number, height: number) =>
    await props.loadImage(
      imageId,
      Math.max(width, 200),
      Math.max(height, 200),
      true,
      'Jpeg',
    ),
);

const loading = await loadingImage(
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
);
const fullAvif = responsiveImageLoader(
  loading,
  'Avif',
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  props.loadImage,
);
const fullJxl = responsiveImageLoader(
  loading,
  'JpegXL',
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  props.loadImage,
);
const fullWebP = responsiveImageLoader(
  loading,
  'WebP',
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  props.loadImage,
);
const fullPng = responsiveImageLoader(
  loading,
  'Png',
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  props.loadImage,
);
const fullJpeg = responsiveImageLoader(
  loading,
  'Jpeg',
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  props.loadImage,
);
</script>
