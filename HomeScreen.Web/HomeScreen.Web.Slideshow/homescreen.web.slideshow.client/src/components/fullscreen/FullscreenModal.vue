<template>
  <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
    <template #activator="props">
      <component
        :is="ResponsiveImageSuspenseAsync"
        :image="image"
        :image-size="imageSize"
        :load-image="loadImage"
        class="size-auto max-h-full max-w-full rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
        v-bind="props"
      />
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
          <ActionButton :disabled="isLoading" @click="() => execute()">
            Toggle Media
          </ActionButton>
        </template>
      </PolaroidCard>
    </template>
  </ModalDialog>
</template>

<script lang="ts" setup>
import {
  ActionButton,
  Directions,
  type Image,
  LeafletMapAsync,
  type LoadImageCallback,
  ModalDialog,
  PolaroidCard,
  ResponsiveImageSuspenseAsync,
} from '@homescreen/web-common-components';
import { useAsyncState, useWindowSize } from '@vueuse/core';
import { toggleMedia } from '@/domain/media';
import { computed } from 'vue';

const props = defineProps<{
  image: Image;
  loadImage: LoadImageCallback;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const { execute, isLoading } = useAsyncState(
  async () => {
    console.log('Toggle Media start', props.image.id, props.image.enabled);
    const result = await toggleMedia(props.image.id, !props.image.enabled);
    console.log('Toggled Media end', result.id, result.enabled);
  },
  undefined,
  { immediate: false },
);

const { width, height } = useWindowSize();

const imageSize = computed(() => ({
  width: Math.floor(width.value - 100),
  height: Math.floor(height.value - 100),
}));
</script>
