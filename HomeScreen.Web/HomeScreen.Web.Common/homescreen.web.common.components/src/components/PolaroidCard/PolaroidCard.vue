<template>
  <div
    class="flex size-fit origin-bottom items-center justify-center p-2"
    :class="{
      'flex-col': direction === Directions.vertical,
      'flex-row': direction === Directions.horizontal,
      'rounded-2xl bg-neutral-300 drop-shadow-lg': !flat,
    }"
    :style="{
      '--imageAspect': image.aspectRatio,
    }"
  >
    <div class="flex grow object-cover">
      <HSImage
        :id="image.id"
        :date-time="image.dateTime"
        :enabled="image.enabled"
        :aspect-ratio="image.aspectRatio"
        :portrait="image.portrait"
        :colour="image.colour"
        :size="size"
        @click="() => emits('click')"
        rounded
        class="drop-shadow-md hover:shadow-inner active:drop-shadow-lg"
      />
    </div>
    <div
      class="flex w-0 items-center justify-center overflow-y-auto px-2 py-3 text-balance"
      :class="{
        'min-w-full text-left': direction === Directions.vertical,
        'h-full min-w-96 text-center': direction === Directions.horizontal,
      }"
    >
      <div class="flex grow flex-col justify-center gap-2">
        <div class="flex grow flex-col items-center justify-center gap-2">
          <p v-if="image.location?.name" class="text-center">
            {{ image.location?.name }}
          </p>
        </div>
        <div class="flex grow flex-col items-center justify-center gap-2">
          <slot :image="image" name="details" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { type Direction, Directions } from '@/components/properties';
import { type Image } from '@/helpers/image';
import HSImage from '@/components/HSImage/HSImage.vue';
import { useImageSize } from '@/helpers/size';

const props = withDefaults(
  defineProps<{
    direction?: Direction;
    image: Image;
    flat?: boolean;
    maxSize?: number;
  }>(),
  {
    direction: Directions.vertical,
    flat: false,
    maxSize: 0,
  },
);

const emits = defineEmits<{ click: [] }>();

const { size } = useImageSize({ image: props.image, maxSize: props.maxSize });
</script>
