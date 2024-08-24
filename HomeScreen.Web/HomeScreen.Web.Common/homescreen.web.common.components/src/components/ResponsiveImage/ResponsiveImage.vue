<template>
  <picture v-if="loading">
    <Suspense>
      <ResponsiveImageSource
        :blur="false"
        :format="MediaTransformOptionsFormat.Avif"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
      />
    </Suspense>
    <Suspense>
      <ResponsiveImageSource
        :blur="false"
        :format="MediaTransformOptionsFormat.JpegXL"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
      />
    </Suspense>
    <Suspense>
      <ResponsiveImageSource
        :blur="false"
        :format="MediaTransformOptionsFormat.WebP"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
      />
    </Suspense>
    <Suspense>
      <ResponsiveImageSource
        :blur="false"
        :format="MediaTransformOptionsFormat.Png"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
      />
    </Suspense>
    <Suspense>
      <ResponsiveImageSource
        :blur="false"
        :format="MediaTransformOptionsFormat.Jpeg"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
      />
    </Suspense>
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
import { type Image, Variants } from '@/components/properties';
import {
  asyncImage,
  type ComputedMediaSize,
  type LoadImageCallback,
} from '@/helpers/computedMedia';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';
import { MediaTransformOptionsFormat } from '@/domain/generated/homescreen-common-api';
import ResponsiveImageSource from '@/components/ResponsiveImage/ResponsiveImageSource.vue';

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
</script>
