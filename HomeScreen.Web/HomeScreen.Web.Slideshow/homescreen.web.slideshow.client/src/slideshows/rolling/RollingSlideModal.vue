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
        <HSImage
          :id="image.id"
          :aspect-ratio="image.aspectRatio"
          :colour="image.colour"
          :date-time="image.dateTime"
          :enabled="image.enabled"
          :portrait="image.portrait"
          :rounded="false"
          :size="256"
          class="hover:shadow-inner active:drop-shadow-lg"
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
            <HSImage
              :id="image.id"
              :aspect-ratio="image.aspectRatio"
              :colour="image.colour"
              :date-time="image.dateTime"
              :enabled="image.enabled"
              :portrait="image.portrait"
              :rounded="false"
              :size="512"
              class="hover:shadow-inner active:drop-shadow-lg"
              v-bind="props"
            />
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
    <div v-else class="hidden-box rounded-md object-contain drop-shadow-md" />
  </div>
</template>

<script async lang="ts" setup>
import {
  type ComputedMediaSize,
  type Direction,
  Directions,
  HSImage,
  type Image,
  LeafletMapAsync,
  ModalDialog,
  PolaroidCard,
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

const size =
  props.direction === Directions.horizontal
    ? toValue(props.imageSize).height
    : toValue(props.imageSize).width;

const imageWidth = computed(
  () => size * (props.image.portrait ? 1 : props.image.aspectRatio),
);
const imageHeight = computed(
  () => size * (props.image.portrait ? props.image.aspectRatio : 1),
);
</script>

<style lang="scss" scoped>
.hidden-box {
  width: calc(1px * v-bind(imageWidth));
  height: calc(1px * v-bind(imageHeight));
}
</style>
