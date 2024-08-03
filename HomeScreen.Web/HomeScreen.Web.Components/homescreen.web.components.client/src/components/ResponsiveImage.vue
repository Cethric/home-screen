<template>
  <picture>
    <source v-if="fullAvif && hasAvif" :srcset="fullAvif" type="image/avif" />
    <source v-if="fullJxl && hasJxl" :srcset="fullJxl" type="image/jxl" />
    <source v-if="fullWebP && hasWebP" :srcset="fullWebP" type="image/webp" />
    <source v-if="fullPng && hasPng" :srcset="fullPng" type="image/png" />
    <source v-if="fullJpeg && hasJpeg" :srcset="fullJpeg" type="image/jpeg" />
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
import { computedAsync, useMemoize } from '@vueuse/core';
import Bowser from 'bowser';

const hasFormatSupport = useMemoize((mime: string): boolean => {
  const parser = Bowser.getParser(navigator.userAgent);
  const isWebKit = /WebKit/.test(parser.getEngineName());
  return isWebKit ? mime !== 'image/avif' : true;
});

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
}>();

const loading = await props.loadImage(
  props.image.id,
  Math.max(props.imageSize.width, 200),
  Math.max(props.imageSize.height, 200),
  true,
  'Jpeg',
);

const hasAvif = hasFormatSupport('image/avif');
const fullAvif = computedAsync(async (onCancel) => {
  if (hasAvif) {
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
  }
  return loading;
}, loading);

const hasJxl = hasFormatSupport('image/jxl');
const fullJxl = computedAsync(async (onCancel) => {
  if (hasJxl) {
    const abortController = new AbortController();
    onCancel(() => abortController.abort('Component unload'));
    return await props.loadImage(
      props.image.id,
      Math.max(props.imageSize.width, 200),
      Math.max(props.imageSize.height, 200),
      false,
      'JpegXL',
      abortController.signal,
    );
  }
  return loading;
}, loading);

const hasWebP = hasFormatSupport('image/webp');
const fullWebP = computedAsync(async (onCancel) => {
  if (hasWebP) {
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
  }
  return loading;
}, loading);

const hasPng = hasFormatSupport('image/png');
const fullPng = computedAsync(async (onCancel) => {
  if (hasPng) {
    const abortController = new AbortController();
    onCancel(() => abortController.abort('Component unload'));
    return await props.loadImage(
      props.image.id,
      Math.max(props.imageSize.width, 200),
      Math.max(props.imageSize.height, 200),
      false,
      'Png',
      abortController.signal,
    );
  }
  return loading;
}, loading);

const hasJpeg = hasFormatSupport('image/jpeg');
const fullJpeg = computedAsync(async (onCancel) => {
  if (hasJpeg) {
    const abortController = new AbortController();
    onCancel(() => abortController.abort('Component unload'));
    return await props.loadImage(
      props.image.id,
      Math.max(props.imageSize.width, 200),
      Math.max(props.imageSize.height, 200),
      false,
      'Jpeg',
      abortController.signal,
    );
  }
  return loading;
}, loading);
</script>
