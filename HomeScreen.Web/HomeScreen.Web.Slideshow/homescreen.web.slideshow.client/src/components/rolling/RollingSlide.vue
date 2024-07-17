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
      <Suspense>
        <template #fallback>
          <div class="relative size-full">
            <LoadingSpinner
              :variant="Variants.primary"
              class="absolute size-full"
            />
          </div>
        </template>
        <RollingSlideModal
          v-for="image in images"
          :key="image.id"
          :direction="direction"
          :image="image"
          :load-image="loadImage"
          @pause="() => emits('pause')"
          @resume="() => emits('resume')"
        />
      </Suspense>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
  type Direction,
  Directions,
  type Image,
  Variants,
} from '@/helpers/component_properties';
import { MediaTransformOptionsFormat } from '@/domain/api/homescreen-slideshow-api';
import RollingSlideModal from '@/components/rolling/RollingSlideModal.vue';
import LoadingSpinner from '@components/LoadingSpinner.vue';

defineProps<{
  images: Image[];
  direction: Direction;
  loadImage: (
    imageId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
  ) => Promise<string>;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();
</script>

<style lang="scss" scoped>
.grid-vertical {
  grid-template-columns: repeat(1, 100%);
  grid-template-rows: masonry;
}
</style>
