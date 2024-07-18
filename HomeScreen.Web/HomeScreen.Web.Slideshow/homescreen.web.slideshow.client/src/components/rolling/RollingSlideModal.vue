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
        <component
          :is="ResponsiveImageSuspenseAsync"
          :class="[
            'rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg',
            {
              'h-full w-auto max-w-fit': direction === Directions.horizontal,
            },
          ]"
          :image="image"
          :image-size="fullSize"
          :load-image="loadImage"
          :loading-size="loadingSize"
        />
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
import { computed } from 'vue';
import { ResponsiveImageSuspenseAsync } from '@/components/ResponsiveImageSuspenseAsync';

defineProps<{
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

const loadingSize = computed(() => ({
  width: Math.max(Math.floor(width.value / 6), 200),
  height: Math.max(Math.floor(height.value / 6), 200),
}));
const fullSize = computed(() => ({
  width: Math.floor(width.value / 4),
  height: Math.floor(height.value / 4),
}));
</script>
