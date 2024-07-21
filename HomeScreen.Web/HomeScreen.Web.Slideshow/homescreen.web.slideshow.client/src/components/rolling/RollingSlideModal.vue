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
          :image-size="imageSize"
          :load-image="loadImage"
        />
      </div>
    </template>

    <template #default>
      <ModalDialog>
        <template #activator="props">
          <PolaroidCard
            :direction="Directions.horizontal"
            :flat="true"
            :image="image"
            :load-image="loadImage"
            v-bind="props"
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
        <template #default>
          <LargeImage :image="image" :load-image="loadImage" />
        </template>
      </ModalDialog>
    </template>
  </ModalDialog>
</template>

<script async lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  LeafletMapAsync,
  type LoadImageCallback,
  ModalDialog,
  PolaroidCard,
  ResponsiveImageSuspenseAsync,
} from '@homescreen/web-components-client/src/index';
import { useWindowSize } from '@vueuse/core';
import { computed } from 'vue';
import LargeImage from '@/components/LargeImage.vue';

defineProps<{
  image: Image;
  direction: Direction;
  loadImage: LoadImageCallback;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const { width, height } = useWindowSize();

const imageSize = computed(() => ({
  width: Math.floor(width.value / 4),
  height: Math.floor(height.value / 4),
}));
</script>
