<template>
  <source
    v-if="srcset"
    :height="height"
    :srcset="srcset"
    :type="type"
    :width="width"
  />
</template>

<script async setup lang="ts">
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { computed } from 'vue';
import { loadImage } from '@/helpers/computedMedia.ts';

const props = defineProps<{
  id: string;
  width: number;
  height: number;
  format: MediaTransformOptionsFormat;
}>();

const srcset = await loadImage(
  props.id,
  Math.trunc(props.width * (window.devicePixelRatio ?? 1)),
  Math.trunc(props.height * (window.devicePixelRatio ?? 1)),
  false,
  props.format,
);

const type = computed(() => {
  switch (props.format) {
    case MediaTransformOptionsFormat.JPEG:
      return 'image/jpeg';
    case MediaTransformOptionsFormat.JPEG_XL:
      return 'image/jxl';
    case MediaTransformOptionsFormat.WEB_P:
      return 'image/webp';
    case MediaTransformOptionsFormat.AVIF:
      return 'image/avif';
    case MediaTransformOptionsFormat.PNG:
      return 'image/png';
  }
});
</script>
