<template>
  <div class="card" :class="{'card-side card-lg': direction === Directions.horizontal, 'drop-shadow-lg': !flat}">
    <figure>
      <HSImage
        :id="image.id"
        :date-time="image.dateTime"
        :enabled="image.enabled"
        :aspect-ratio="image.aspectRatio"
        :portrait="image.portrait"
        :colour="image.colour"
        :size="maxSize"
        rounded
        @click="() => emits('click')"
        class="cursor-pointer hover:shadow-inner active:drop-shadow-lg"
      />
    </figure>
    <div class="card-body">
      <h2 class="card-title wrap-break-word text-center">
        <slot :image="image" name="title" />
      </h2>
      <slot :image="image" name="details" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import HSImage from '@/components/HSImage/HSImage.vue';
import { type Direction, Directions } from '@/components/properties';
import type { Image } from '@/helpers/image';

withDefaults(
  defineProps<{
    direction?: Direction;
    image: Image;
    flat?: boolean;
    maxSize?: number;
  }>(),
  {
    direction: Directions.vertical,
    flat: false,
    maxSize: 800,
  },
);

const emits = defineEmits<{ click: [] }>();
</script>
