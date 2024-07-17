<template>
  <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
    <template #activator="props">
      <div
        :class="[
          {
            'h-full w-auto grow px-4': direction === Directions.horizontal,
            'col-span-1 row-span-1 flex items-center justify-center':
              direction === Directions.vertical,
          },
        ]"
        v-bind="props"
      >
        <picture>
          <source :srcset="fullAvif" />
          <source :srcset="fullWebP" />
          <img
            :class="[
              'rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg',
              {
                'h-full w-auto max-w-fit': direction === Directions.horizontal,
              },
            ]"
            :src="loading"
            alt="Example media"
            loading="lazy"
          />
        </picture>
      </div>
    </template>

    <template #default>
      <PolaroidCard
        :direction="Directions.horizontal"
        :flat="true"
        :image="image"
        :load-image="loadImage"
      >
        <template #details="{ image }">
          <component
            :is="LeafletMapAsync"
            v-if="image.location?.latitude && image.location?.longitude"
            :latitude="image.location.latitude"
            :longitude="image.location.longitude"
            :tooltip="image.location.name"
          />
        </template>
      </PolaroidCard>
    </template>
  </ModalDialog>
</template>

<script async lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
} from '@/helpers/component_properties';
import PolaroidCard from '@components/PolaroidCard.vue';
import ModalDialog from '@components/ModalDialog.vue';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import { useWindowSize } from '@vueuse/core';
import { LeafletMapAsync } from '@/components/LeafletMapAsync';

const props = defineProps<{
  image: Image;
  direction: Direction;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const { width, height } = useWindowSize();

const loading = await props.loadImage(
  props.image.id,
  Math.trunc(width.value / 3),
  Math.trunc(height.value / 3),
  true,
  MediaTransformOptionsFormat.Jpeg,
);
const fullAvif = await props.loadImage(
  props.image.id,
  Math.trunc(width.value - 50),
  Math.trunc(height.value - 50),
  false,
  MediaTransformOptionsFormat.Avif,
);
const fullWebP = await props.loadImage(
  props.image.id,
  Math.trunc(width.value - 50),
  Math.trunc(height.value - 50),
  false,
  MediaTransformOptionsFormat.WebP,
);
</script>
