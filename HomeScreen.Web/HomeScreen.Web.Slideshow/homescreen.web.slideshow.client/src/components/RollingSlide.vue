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
          'grid-vertical grid w-full grow gap-4 py-2':
            direction === Directions.vertical,
        },
      ]"
    >
      <ModalDialog
        v-for="image in images"
        :key="image.id"
        @hide="() => emits('resume')"
        @show="() => emits('pause')"
      >
        <template #activator="props">
          <div
            :class="[
              {
                'h-full w-auto grow px-4': direction === Directions.horizontal,
                'col-span-1 row-span-1 flex items-center justify-center':
                  direction === Directions.vertical,
              },
            ]"
            v-bind="props"
          >
            <picture>
              <source v-for="src in image.src" :key="src" :srcset="src" />
              <img
                :class="[
                  'rounded-md object-contain drop-shadow-md hover:shadow-inner active:drop-shadow-lg',
                  {
                    'h-full w-auto max-w-fit':
                      direction === Directions.horizontal,
                  },
                ]"
                :src="image.loading"
                alt="Example media"
                loading="lazy"
              />
            </picture>
          </div>
        </template>

        <template #default>
          <PolaroidCard :direction="Directions.horizontal" :image="image" flat>
            <template #details="{ image }">
              <OpenLayersMap
                :latitude="image.location.latitude"
                :longitude="image.location.longitude"
              />
            </template>
          </PolaroidCard>
        </template>
      </ModalDialog>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { type Direction, Directions, Image } from '@components/properties';
import ModalDialog from '@components/ModalDialog.vue';
import OpenLayersMap from '@components/OpenLayersMap.vue';
import PolaroidCard from '@components/PolaroidCard.vue';

defineProps<{ images: Image[]; direction: Direction }>();

const emits = defineEmits<{ pause: []; resume: [] }>();
</script>

<style lang="scss" scoped>
.grid-vertical {
  grid-template-columns: repeat(1, 100%);
  grid-template-rows: masonry;
}
</style>
