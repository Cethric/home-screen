<template>
  <ResponsivePictureAsync
    :image="image"
    :image-size="imageSize"
    :max-image-size="maxImageSize"
    class="h-full min-h-96 w-auto min-w-96 max-w-fit rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
  />
</template>

<script lang="ts" setup>
import { useWindowSize } from '@vueuse/core';
import { computed } from 'vue';
import type { Image } from '@/components/ResponsivePicture/image';
import { ResponsivePictureAsync } from '@/components/ResponsivePicture/ResponsivePictureAsync';

const props = defineProps<{
  image: Image;
}>();

const { width, height } = useWindowSize();

const imageSize = computed(() => ({
  width: Math.floor(width.value * 0.95),
  height: Math.floor(height.value * 0.75),
}));

const maxImageSize = computed(() => ({
  width: Math.floor(
    width.value *
      0.95 *
      (height.value > width.value
        ? props.image.aspectHeight
        : props.image.aspectWidth),
  ),
  height: Math.floor(
    height.value *
      0.65 *
      (height.value > width.value ? props.image.aspectHeight : 1),
  ),
}));
</script>
