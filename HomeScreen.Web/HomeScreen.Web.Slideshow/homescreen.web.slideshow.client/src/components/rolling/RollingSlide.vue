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
        @pause="() => emits('pause')"
        @resume="() => emits('resume')"
      />
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  type LoadImageCallback,
} from '@homescreen/web-components-client/src/index';
import RollingSlideModal from '@/components/rolling/RollingSlideModal.vue';

defineProps<{
  images: Image[];
  direction: Direction;
  loadImage: LoadImageCallback;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();
</script>

<style lang="scss" scoped>
.grid-vertical {
  grid-template-columns: repeat(1, 100%);
  grid-template-rows: masonry;
}
</style>
