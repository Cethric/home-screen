<template>
  <Suspense suspensible timeout="0">
    <template #fallback>
      <div class="size-96">
        <LoadingSpinner :variant="Variants.primary" class="size-full" />
      </div>
    </template>
    <ResponsiveImage
      :image="image"
      :image-size="imageSize"
      :load-image="loadImage"
      :loading-size="loadingSize"
      v-bind="$attrs"
    />
  </Suspense>
</template>

<script lang="ts" setup>
import { type Image, Variants } from './properties';
import LoadingSpinner from './LoadingSpinner.vue';
import ResponsiveImage from './ResponsiveImage.vue';
import type {
  ComputedMediaSize,
  LoadImageCallback,
} from '@/helpers/computedMedia';

defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
  loadingSize: ComputedMediaSize;
  imageSize: ComputedMediaSize;
}>();
</script>
