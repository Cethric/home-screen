<template>
  <picture v-if="loading">
    <source v-if="loading && fullAvif" :srcset="fullAvif" type="image/avif" />
    <source v-if="loading && fullJxl" :srcset="fullJxl" type="image/jxl" />
    <source v-if="loading && fullWebP" :srcset="fullWebP" type="image/webp" />
    <source v-if="loading && fullPng" :srcset="fullPng" type="image/png" />
    <source v-if="loading && fullJpeg" :srcset="fullJpeg" type="image/jpeg" />
    <img
      :alt="
        image.location?.name ||
        `Taken at ${image.dateTime.toFormat('MMMM Do YYYY HH:mm')}`
      "
      :class="$attrs['class']"
      :src="loading"
    />
  </picture>
  <LoadingSpinner
    v-else
    :style="{ width: imageSize.width, height: imageSize.height }"
    :variant="Variants.primary"
  />
</template>

<script lang="ts" setup>
import { type Image, Variants } from './properties';
import {
  asyncImage,
  type ComputedMediaSize,
  type LoadImageCallback,
  responsiveImageLoader,
} from '@/helpers/computedMedia';
import LoadingSpinner from './LoadingSpinner.vue';
import { MediaTransformOptionsFormat } from '@/domain/generated/homescreen-common-api';

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
}>();

const loading = asyncImage(
  props.loadImage,
  MediaTransformOptionsFormat.Jpeg,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  true,
);

const fullAvif = responsiveImageLoader(
  props.loadImage,
  MediaTransformOptionsFormat.Avif,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  loading,
);
const fullJxl = responsiveImageLoader(
  props.loadImage,
  MediaTransformOptionsFormat.JpegXL,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  loading,
);
const fullWebP = responsiveImageLoader(
  props.loadImage,
  MediaTransformOptionsFormat.WebP,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  loading,
);
const fullPng = responsiveImageLoader(
  props.loadImage,
  MediaTransformOptionsFormat.Png,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  loading,
);
const fullJpeg = responsiveImageLoader(
  props.loadImage,
  MediaTransformOptionsFormat.Jpeg,
  props.image.id,
  props.imageSize.width,
  props.imageSize.height,
  loading,
);
</script>
