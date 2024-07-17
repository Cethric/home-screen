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
import { Directions, type Image } from '@/helpers/component_properties';
import PolaroidCard from '@components/PolaroidCard.vue';
import ModalDialog from '@components/ModalDialog.vue';
import ActionButton from '@components/ActionButton.vue';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import { useAsyncState, useWindowSize } from '@vueuse/core';
import { toggleMedia } from '@/domain/media';
import { LeafletMapAsync } from '@/components/LeafletMapAsync';

const props = defineProps<{
  image: Image;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
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
