<template>
  <component
    :is="ResponsiveImageSuspenseAsync"
    :image="image"
    :image-size="fullSize"
    :load-image="loadImage"
    :loading-size="loadingSize"
    class="h-full min-h-96 w-auto min-w-96 max-w-fit rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
  />
</template>

<script lang="ts" setup>
import { useWindowSize } from '@vueuse/core';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import { type Image } from '@/helpers/component_properties';
import { computed } from 'vue';
import { ResponsiveImageSuspenseAsync } from '@/components/ResponsiveImageSuspenseAsync';

defineProps<{
  image: Image;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();

const { width, height } = useWindowSize();

const loadingSize = computed(() => ({
  width: Math.floor(width.value / 3),
  height: Math.floor(height.value / 3),
}));
const fullSize = computed(() => ({
  width: Math.floor(width.value - 200),
  height: Math.floor(height.value - 200),
}));
</script>
