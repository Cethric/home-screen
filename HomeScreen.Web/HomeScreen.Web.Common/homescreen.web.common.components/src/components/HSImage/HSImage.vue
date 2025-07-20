<template>
  <div
    class="aspect-(--imageAspect) size-auto bg-(--color) drop-shadow-md"
    :style="{
      '--imageAspect': aspectRatio,
      '--color': color,
      maxWidth: `${deviceSize}px`,
      maxHeight: '85dvh'
    }"
    :class="{ 'rounded-2xl': rounded }"
  >
    <picture>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.AVIF"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.JPEG_XL"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.WEB_P"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.PNG"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :width="imageWidth"
          :height="imageHeight"
          :format="MediaTransformOptionsFormat.JPEG"
        />
      </Suspense>
      <Suspense>
        <HSImageImg
          :width="imageWidth"
          :height="imageHeight"
          :size="deviceSize"
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
          <div class="flex items-center justify-center aspect-(--imageAspect) size-auto bg-(--color)" :style="{
      '--imageAspect': aspectRatio,
      '--color': color,
      'width': `${deviceSize}px`,
      maxHeight: '90dvh'
      }">
            <LoadingSpinner variant="primary" />
          </div>
        </template>
      </Suspense>
    </picture>
  </div>
</template>

<script setup lang="ts">
import { MediaTransformOptionsFormat } from '@homescreen/web-common-components-api';
import { computed } from 'vue';
import HSImageImg from '@/components/HSImage/HSImageImg.vue';
import HSImageSource from '@/components/HSImage/HSImageSource.vue';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';
import type { Image } from '@/helpers/image';

const props = withDefaults(
  defineProps<Image & { size: number; rounded: boolean }>(),
  { rounded: false },
);

const color = computed(
  () => `rgb(${props.colour.red}, ${props.colour.green}, ${props.colour.blue})`,
);

const deviceSize = props.size / window.devicePixelRatio;

const imageWidth = computed(
  () => props.size * (props.portrait ? 1 : props.aspectRatio),
);
const imageHeight = computed(
  () => props.size * (props.portrait ? props.aspectRatio : 1),
);
</script>
