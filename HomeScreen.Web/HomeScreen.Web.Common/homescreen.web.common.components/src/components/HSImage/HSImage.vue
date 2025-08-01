<template>
  <div
    :class="{ 'rounded-2xl': rounded }"
    :style="{
      '--imageAspect': aspectRatio,
      '--color': color,
      maxWidth: `${deviceSize}px`,
      maxHeight: '85dvh',
    }"
    class="aspect-(--imageAspect) size-auto bg-(--color) drop-shadow-md"
  >
    <picture>
      <Suspense>
        <HSImageSource
          :id="id"
          :format="MediaTransformOptionsFormat.AVIF"
          :height="imageHeight"
          :width="imageWidth"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :format="MediaTransformOptionsFormat.JPEG_XL"
          :height="imageHeight"
          :width="imageWidth"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :format="MediaTransformOptionsFormat.WEB_P"
          :height="imageHeight"
          :width="imageWidth"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :format="MediaTransformOptionsFormat.PNG"
          :height="imageHeight"
          :width="imageWidth"
        />
      </Suspense>
      <Suspense>
        <HSImageSource
          :id="id"
          :format="MediaTransformOptionsFormat.JPEG"
          :height="imageHeight"
          :width="imageWidth"
        />
      </Suspense>
      <Suspense>
        <HSImageImg
          :id="id"
          :aspect-ratio="aspectRatio"
          :class="$attrs['class']"
          :color="color"
          :colour="colour"
          :date-time="dateTime"
          :enabled="enabled"
          :height="imageHeight"
          :location="location"
          :portrait="portrait"
          :rounded="rounded"
          :size="deviceSize"
          :width="imageWidth"
        />
        <template #fallback>
          <div
            :style="{
              '--imageAspect': aspectRatio,
              '--color': color,
              width: `${deviceSize}px`,
              maxHeight: '90dvh',
            }"
            class="flex items-center justify-center aspect-(--imageAspect) size-auto bg-(--color)"
          >
            <LoadingSpinner variant="primary" />
          </div>
        </template>
      </Suspense>
    </picture>
  </div>
</template>

<script lang="ts" setup>
import { MediaTransformOptionsFormat } from "@homescreen/web-common-components-api";
import { computed } from "vue";
import HSImageImg from "@/components/HSImage/HSImageImg.vue";
import HSImageSource from "@/components/HSImage/HSImageSource.vue";
import LoadingSpinner from "@/components/LoadingSpinner/LoadingSpinner.vue";
import type { Image } from "@/helpers/image";

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
