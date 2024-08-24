<template>
  <source v-if="srcset" :srcset="srcset" :type="type" />
</template>

<script async lang="ts" setup>
import type { Image } from '@/components/properties';
import type {
  ComputedMediaSize,
  LoadImageCallback,
} from '@/helpers/computedMedia';
import { MediaTransformOptionsFormat } from '@/domain/generated/homescreen-common-api';

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
  blur: boolean;
  format: MediaTransformOptionsFormat;
}>();

const srcset = await props.loadImage(
  props.image.id,
  Math.max(props.imageSize.width, 250),
  Math.max(props.imageSize.height, 250),
  props.blur,
  props.format,
);

let type = '';
switch (props.format) {
  case MediaTransformOptionsFormat.Jpeg:
    type = 'image/jpeg';
    break;
  case MediaTransformOptionsFormat.JpegXL:
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
