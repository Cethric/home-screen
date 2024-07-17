<template>
  <div
    :style="{
      '--offset-top': item.top,
      '--offset-left': item.left,
      '--rotation': item.rotation,
    }"
    class="polaroid-modal absolute size-fit"
  >
    <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
      <template #activator="props">
        <PolaroidCard
          :image="item.image"
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
              :image="item.image"
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
            <Suspense>
              <template #fallback>
                <div class="relative size-full min-h-96 min-w-96">
                  <LoadingSpinner
                    :variant="Variants.primary"
                    class="absolute size-full"
                  />
                </div>
              </template>
              <LargeImage :image="item.image" :load-image="loadImage" />
            </Suspense>
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
  </div>
</template>

<script lang="ts" setup>
import { Directions, Variants } from '@/helpers/component_properties';
import LargeImage from '@/components/LargeImage.vue';
import PolaroidCard from '@components/PolaroidCard.vue';
import ModalDialog from '@components/ModalDialog.vue';
import {
  MediaItem,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import LoadingSpinner from '@components/LoadingSpinner.vue';
import { LeafletMapAsync } from '@/components/LeafletMapAsync';

defineProps<{
  item: { image: MediaItem; top: number; left: number; rotation: number };
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();
const emits = defineEmits<{ resume: []; pause: [] }>();
</script>

<style lang="scss" scoped>
.polaroid-modal {
  transform: translate(
      calc(var(--offset-left) * 1dvw),
      calc(var(--offset-top) * 1dvh)
    )
    rotate(calc(var(--rotation) * 1deg));
}
</style>
