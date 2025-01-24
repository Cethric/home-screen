<template>
  <source
    v-if="srcset"
    :height="height"
    :srcset="srcset"
    :type="type"
    :width="width"
  />
</template>

<script async lang="ts" setup>
import { loadImage } from '@/helpers/computedMedia';
import { toValue } from 'vue';
import {
  type ComputedMediaSize,
  type Image,
} from '@/components/ResponsivePicture/image';
import { MediaTransformOptionsFormat } from '@/domain/generated/schema';

const props = defineProps<{
  image: Image;
  imageSize: ComputedMediaSize;
  blur: boolean;
  format: MediaTransformOptionsFormat;
}>();

const { width, height } = toValue(props.imageSize);

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

let type = '';
switch (props.format) {
  case MediaTransformOptionsFormat.Jpeg:
    type = 'image/jpeg';
    break;
  case MediaTransformOptionsFormat.JpegXl:
    type = 'image/jxl';
    break;
  case MediaTransformOptionsFormat.Png:
    type = 'image/png';
    break;
  case MediaTransformOptionsFormat.WebP:
    type = 'image/webp';
    break;
  case MediaTransformOptionsFormat.Avif:
    type = 'image/avif';
    break;
}
</script>
