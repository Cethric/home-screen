<template>
  <div
    :class="[
      {
        'inline-block': direction === Directions.horizontal,
        'w-full': direction === Directions.vertical,
      },
    ]"
  >
    <div
      :class="[
        {
          'flex size-full flex-row': direction === Directions.horizontal,
          'grid-vertical grid min-h-12 w-full grow gap-4 py-2':
            direction === Directions.vertical,
        },
      ]"
    >
      <RollingSlideModal
        v-for="image in images"
        :key="image.id"
        :direction="direction"
        :image="image"
        :load-image="loadImage"
        :image-size="imageSize"
        @pause="() => emits('pause')"
        @resume="() => emits('resume')"
      />
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
  type ComputedMediaSize,
  type Direction,
  Directions,
  type Image,
  type LoadImageCallback,
} from '@homescreen/web-common-components';
import RollingSlideModal from '@/components/rolling/RollingSlideModal.vue';

defineProps<{
  images: Image[];
  direction: Direction;
  loadImage: LoadImageCallback;
  imageSize: ComputedMediaSize;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();
</script>

<style lang="scss" scoped>
.grid-vertical {
  grid-template-columns: repeat(1, 100%);
  grid-template-rows: masonry;
}
</style>
