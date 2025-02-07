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
import { loadImage } from '@/helpers/computedMedia.ts';
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { computed } from 'vue';

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
    case MediaTransformOptionsFormat.Jpeg:
      return 'image/jpeg';
    case MediaTransformOptionsFormat.JpegXl:
      return 'image/jxl';
    case MediaTransformOptionsFormat.Png:
      return 'image/png';
    case MediaTransformOptionsFormat.WebP:
      return 'image/webp';
    case MediaTransformOptionsFormat.Avif:
      return 'image/avif';
  }
});
</script>
