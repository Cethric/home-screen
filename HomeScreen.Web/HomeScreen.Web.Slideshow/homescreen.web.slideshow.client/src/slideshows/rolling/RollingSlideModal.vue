<template>
  <div
    ref="sliderModal"
    :class="[
      'flex items-center justify-center',
      {
        'h-full w-auto grow px-4': direction === Directions.horizontal,
        'col-span-1 row-span-1': direction === Directions.vertical,
      },
    ]"
  >
    <ModalDialog
      v-if="isVisible"
      @hide="() => emits('resume')"
      @show="() => emits('pause')"
    >
      <template #activator="props">
        <ResponsivePictureAsync
          :image="image"
          :image-size="imageSize"
          :min-image-size="{ width: 0, height: 0 }"
          class="rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
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
              v-bind="props"
            >
              <template #details="{ image }">
                <LeafletMapAsync
                  v-if="image.location?.latitude && image.location?.longitude"
                  :latitude="image.location.latitude"
                  :longitude="image.location.longitude"
                  :tooltip="image.location.name"
                />
              </template>
            </PolaroidCard>
          </template>
          <template #default>
            <LargeImage :image="image" />
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
    <div
      v-else
      :style="styledSize"
      class="rounded-md object-contain drop-shadow-md"
    />
  </div>
</template>

<script async lang="ts" setup>
import {
  type ComputedMediaSize,
  type Direction,
  Directions,
  type Image,
  LargeImage,
  LeafletMapAsync,
  ModalDialog,
  PolaroidCard,
  ResponsivePictureAsync,
  useImageAspectSize,
} from '@homescreen/web-common-components';
import { useElementVisibility } from '@vueuse/core';
import { computed, ref, toValue } from 'vue';

const props = defineProps<{
  image: Image;
  direction: Direction;
  imageSize: ComputedMediaSize;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const sliderModal = ref<HTMLElement>();
const isVisible = useElementVisibility(sliderModal, {
  threshold: [1e-1],
});
const size = useImageAspectSize({
  image: props.image,
  size: props.imageSize,
  minSize: { width: 0, height: 0 },
});
console.log('Aspect Size Result', size);
const aspectSize = computed(() => {
  const { width, height } = toValue(size);
  return { width: `${width}px`, height: `${height}px` };
});

const styledSize = computed(() => {
  const { width, height } = toValue(aspectSize);
  return {
    minWidth: width,
    minHeight: height,
    maxWidth: width,
    maxHeight: height,
    width: width,
    height: height,
  };
});
</script>
