<template>
  <div
    class="hs-image aspect-(--imageAspect) drop-shadow-md"
    :style="{
      '--imageAspect': aspectRatio,
    }"
    :class="{ 'rounded-2xl': rounded }"
  >
    <picture class="flex size-full">
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.Avif"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.JpegXl"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.WebP"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.Png"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.Jpeg"
        />
      </Suspense>
      <Suspense>
        <HSImageImg
          :width="imageWidth"
          :height="imageHeight"
          :id="id"
          :date-time="dateTime"
          :location="location"
          :color="color"
          :colour="colour"
          :aspect-ratio="aspectRatio"
          :enabled="enabled"
          :portrait="portrait"
          :rounded="rounded"
          :class="$attrs['class']"
        />
        <template #fallback>
          <LoadingSpinner variant="primary" />
        </template>
      </Suspense>
    </picture>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { type Image } from '@/helpers/image';
import HSImageImg from '@/components/HSImage/HSImageImg.vue';
import HSImageSource from '@/components/HSImage/HSImageSource.vue';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';

const props = withDefaults(
  defineProps<Image & { size: number; rounded: boolean }>(),
  { rounded: false },
);

const color = computed(
  () => `rgb(${props.colour.red}, ${props.colour.green}, ${props.colour.blue})`,
);

const imageWidth = computed(
  () => props.size * (props.portrait ? 1 : props.aspectRatio),
);
const imageHeight = computed(
  () => props.size * (props.portrait ? props.aspectRatio : 1),
);
</script>

<style scoped lang="css">
.hs-image {
  max-width: calc(1px * v-bind(imageWidth));
  max-height: calc(1px * v-bind(imageHeight));
  min-width: calc(1px * v-bind(imageWidth));
  min-height: calc(1px * v-bind(imageHeight));

  width: 100%;
  height: auto;

  background-color: v-bind(color);
}
</style>
