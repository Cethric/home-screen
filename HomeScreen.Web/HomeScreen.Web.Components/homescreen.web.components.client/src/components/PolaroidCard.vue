<template>
  <div
    :class="[
      'polaroid',
      'flex size-fit origin-bottom items-center justify-center p-2',
      {
        'flex-col': direction === Directions.vertical,
        'flex-row': direction === Directions.horizontal,
        absolute: !!location,
        'rounded-2xl bg-neutral-300 drop-shadow-lg': !flat,
      },
    ]"
    :data-direction="direction"
    :data-translated="!!location"
    :style="{
      '--offset-top': location?.top,
      '--offset-left': location?.left,
      '--rotation': location?.rotation,
    }"
  >
    <div
      :class="[
        'grow',
        {
          'max-h-96': direction === Directions.vertical,
          'max-w-72': direction === Directions.horizontal,
        },
      ]"
    >
      <picture>
        <source v-for="src in image.images" :key="src" :srcset="src" />
        <img
          :class="[
            'size-auto max-h-full max-w-full rounded-md object-contain drop-shadow-md',
            {
              '!max-h-96': direction === Directions.vertical,
              '!max-w-72': direction === Directions.horizontal,
            },
          ]"
          :height="image.height"
          :src="image.thumbnail"
          :width="image.width"
          alt="Example media"
          loading="lazy"
        />
      </picture>
    </div>
    <div
      :class="[
        'w-0 text-balance px-2 py-3',
        {
          'min-w-full text-left': direction === Directions.vertical,
          'min-w-64 text-center': direction === Directions.horizontal,
        },
      ]"
    >
      <p>Location: {{ image.location.name }}</p>
      <p>
        Time: {{ image.dateTime.toFormat('DDDD') }}
        {{ image.dateTime.toFormat('TTT') }}
      </p>
      <slot :image="image" name="details" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { type Direction, Directions, type Image } from './properties';

withDefaults(
  defineProps<{
    direction?: Direction;
    location?: { top: number; left: number; rotation: number };
    image: Image;
    flat?: boolean;
  }>(),
  {
    direction: Directions.vertical,
    flat: false,
  },
);
</script>

<style lang="scss" scoped>
.polaroid {
  &[data-direction='horizontal'] {
    max-width: 40rem;
  }

  &[data-direction='vertical'] {
    max-width: 27rem;
  }

  &[data-translated='true'] {
    transform: translate(
        calc(var(--offset-left) * 1dvw),
        calc(var(--offset-top) * 1dvh)
      )
      rotate(calc(var(--rotation) * 1deg));
  }

  picture {
    min-width: 25rem;
  }
}
</style>
