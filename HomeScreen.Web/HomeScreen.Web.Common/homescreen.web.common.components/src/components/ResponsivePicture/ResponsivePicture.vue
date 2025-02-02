<template>
  <picture>
    <Suspense>
      <PictureImage
        :key="`img-${size.width}-${size.width}`"
        :blur="true"
        :class="$attrs['class']"
        :format="MediaTransformOptionsFormat.Jpeg"
        :image="image"
        :image-size="size"
      >
        <PictureSources
          :key="`src-${size.width}-${size.width}`"
          :image="image"
          :image-size="imageSize"
        />
      </PictureImage>
      <template #fallback>
        <div
          :style="{
            width: `${size.width}px`,
            height: `${size.height}px`,
            maxWidth: `${size.width}px`,
            maxHeight: `${size.height}px`,
            backgroundColor: colour,
          }"
        >
          <LoadingSpinner
            :style="{
              width: `${size.width}px`,
              height: `${size.height}px`,
              maxWidth: `${size.width}px`,
              maxHeight: `${size.height}px`,
            }"
            :variant="Variants.primary"
          />
        </div>
      </template>
    </Suspense>
  </picture>
</template>

<script lang="ts" setup>
import { Variants } from '@/components/properties';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';
import PictureImage from '@/components/ResponsivePicture/PictureImage.vue';
import PictureSources from '@/components/ResponsivePicture/PictureSources.vue';
import {
  imageToColour,
  type ResponsivePictureProps,
  useImageAspectSize,
} from '@/components/ResponsivePicture/image';
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';

const props = defineProps<ResponsivePictureProps>();

const size = useImageAspectSize({
  image: props.image,
  size: props.imageSize,
  minSize: props.minImageSize,
  maxSize: props.maxImageSize,
});
const colour = imageToColour(props.image);
</script>
