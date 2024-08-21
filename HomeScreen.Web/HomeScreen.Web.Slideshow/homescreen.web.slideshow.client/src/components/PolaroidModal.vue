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
            <LargeImage :image="item.image" :load-image="loadImage" />
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
  </div>
</template>

<script lang="ts" setup>
import LargeImage from '@/components/LargeImage.vue';
import {
  Directions,
  LeafletMapAsync,
  type LoadImageCallback,
  ModalDialog,
  PolaroidCard,
} from '@homescreen/web-common-components';
import type { PolaroidImage } from '@/components/properties';

defineProps<{
  item: PolaroidImage;
  loadImage: LoadImageCallback;
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
