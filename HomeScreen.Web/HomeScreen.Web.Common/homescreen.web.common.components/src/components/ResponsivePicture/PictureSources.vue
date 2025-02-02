<template>
  <Suspense>
    <MediaSource
      :key="`${MediaTransformOptionsFormat.Avif}-${computedSize.width}-${computedSize.height}`"
      :blur="false"
      :format="MediaTransformOptionsFormat.Avif"
      :image="image"
      :image-size="imageSize"
    />
  </Suspense>
  <Suspense>
    <MediaSource
      :key="`${MediaTransformOptionsFormat.JpegXl}-${computedSize.width}-${computedSize.height}`"
      :blur="false"
      :format="MediaTransformOptionsFormat.JpegXl"
      :image="image"
      :image-size="imageSize"
    />
  </Suspense>
  <Suspense>
    <MediaSource
      :key="`${MediaTransformOptionsFormat.WebP}-${computedSize.width}-${computedSize.height}`"
      :blur="false"
      :format="MediaTransformOptionsFormat.WebP"
      :image="image"
      :image-size="imageSize"
    />
  </Suspense>
  <Suspense>
    <MediaSource
      :key="`${MediaTransformOptionsFormat.Png}-${computedSize.width}-${computedSize.height}`"
      :blur="false"
      :format="MediaTransformOptionsFormat.Png"
      :image="image"
      :image-size="imageSize"
    />
  </Suspense>
  <Suspense>
    <MediaSource
      :key="`${MediaTransformOptionsFormat.Jpeg}-${computedSize.width}-${computedSize.height}`"
      :blur="false"
      :format="MediaTransformOptionsFormat.Jpeg"
      :image="image"
      :image-size="imageSize"
    />
  </Suspense>
</template>

<script lang="ts" setup>
import MediaSource from '@/components/ResponsivePicture/PictureSource.vue';
import { computed, toValue } from 'vue';
import type {
  ComputedMediaSize,
  Image,
} from '@/components/ResponsivePicture/image';
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';

const props = defineProps<{ image: Image; imageSize: ComputedMediaSize }>();
const computedSize = computed(() => {
  return toValue(props.imageSize);
});
</script>
