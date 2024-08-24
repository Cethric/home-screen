<template>
  <div
    ref="sliderModal"
    :class="[
      {
        'h-full w-auto grow px-4': direction === Directions.horizontal,
        'col-span-1 row-span-1 flex items-center justify-center':
          direction === Directions.vertical,
      },
    ]"
  >
    <ModalDialog
      v-if="isVisible"
      @hide="() => emits('resume')"
      @show="() => emits('pause')"
    >
      <template #activator="props">
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
          v-bind="props"
        />
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
    <div
      v-else
      :style="{
        width: `${imageSize.width}px`,
        height: `${imageSize.height}px`,
      }"
    />
  </div>
</template>

<script async lang="ts" setup>
import {
  type ComputedMediaSize,
  type Direction,
  Directions,
  type Image,
  LeafletMapAsync,
  type LoadImageCallback,
  ModalDialog,
  PolaroidCard,
  ResponsiveImageSuspenseAsync,
} from '../../../../../HomeScreen.Web.Common/homescreen.web.common.components';
import { useElementVisibility } from '@vueuse/core';
import { ref } from 'vue';
import LargeImage from '@/components/LargeImage.vue';

defineProps<{
  image: Image;
  direction: Direction;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const sliderModal = ref<HTMLElement>();
const isVisible = useElementVisibility(sliderModal, {
  threshold: [0.1, 0.9],
});
</script>
