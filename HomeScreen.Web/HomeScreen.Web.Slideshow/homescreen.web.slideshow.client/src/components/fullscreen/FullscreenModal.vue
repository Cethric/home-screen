<template>
  <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
    <template #activator="props">
      <picture v-bind="props">
        <source :srcset="fullAvif" />
        <source :srcset="fullWebP" />
        <img
          :src="loading"
          alt="Example media"
          class="size-auto max-h-full max-w-full rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
        />
      </picture>
    </template>

    <template #default>
      <PolaroidCard
        :direction="Directions.horizontal"
        :image="image"
        :load-image="loadImage"
        flat
      >
        <template #details="{ image }">
          <OpenLayersMap
            v-if="image.location.name.trim().length > 0"
            :latitude="image.location.latitude"
            :longitude="image.location.longitude"
          />
        </template>
      </PolaroidCard>
    </template>
  </ModalDialog>
</template>

<script lang="ts" setup>
import { Directions, type Image } from '@/helpers/component_properties';
import PolaroidCard from '@components/PolaroidCard.vue';
import ModalDialog from '@components/ModalDialog.vue';
import OpenLayersMap from '@components/OpenLayersMap.vue';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import { useWindowSize } from '@vueuse/core';

const props = defineProps<{
  image: Image;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: number,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const { width, height } = useWindowSize();

const loading = await props.loadImage(
  props.image.id,
  Math.trunc(width.value / 2),
  Math.trunc(height.value / 2),
  20,
  MediaTransformOptionsFormat.Jpeg,
);
const fullAvif = await props.loadImage(
  props.image.id,
  Math.trunc(width.value - 50),
  Math.trunc(height.value - 50),
  0,
  MediaTransformOptionsFormat.Avif,
);
const fullWebP = await props.loadImage(
  props.image.id,
  Math.trunc(width.value - 50),
  Math.trunc(height.value - 50),
  0,
  MediaTransformOptionsFormat.WebP,
);
</script>
