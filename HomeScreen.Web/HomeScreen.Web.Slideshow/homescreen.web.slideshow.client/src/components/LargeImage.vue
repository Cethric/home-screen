<template>
  <component
    :is="ResponsiveImageSuspenseAsync"
    :image="image"
    :image-size="imageSize"
    :load-image="loadImage"
    class="h-full min-h-96 w-auto min-w-96 max-w-fit rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
  />
</template>

<script lang="ts" setup>
import { useWindowSize } from '@vueuse/core';
import { computed } from 'vue';
import {
  type Image,
  type LoadImageCallback,
  ResponsiveImageSuspenseAsync,
} from '@homescreen/web-components-client';

defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
}>();

const { width, height } = useWindowSize();

const imageSize = computed(() => ({
  width: Math.floor(width.value - 200),
  height: Math.floor(height.value - 200),
}));
</script>
