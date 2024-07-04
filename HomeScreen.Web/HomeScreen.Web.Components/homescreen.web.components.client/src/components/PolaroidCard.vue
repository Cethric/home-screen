<template>
  <div
    ref="polaroid"
    :class="[
      'polaroid',
      'flex size-fit origin-bottom items-center justify-center p-2',
      {
        'flex-col': direction === Directions.vertical,
        'flex-row': direction === Directions.horizontal,
        'rounded-2xl bg-neutral-300 drop-shadow-lg': !flat,
      },
    ]"
    :data-direction="direction"
  >
    <div class="grow">
      <picture @click="onClick">
        <source :srcset="fullAvif" />
        <source :srcset="fullWebP" />
        <img
          :src="loading"
          alt="Example media"
          class="size-auto rounded-md object-contain drop-shadow-md"
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

<script async lang="ts" setup>
import { type Direction, Directions, type Image } from './properties';
import { computedAsync, useElementSize } from '@vueuse/core';
import { ref } from 'vue';

const props = withDefaults(
  defineProps<{
    direction?: Direction;
    image: Image;
    flat?: boolean;
    loadImage: (
      imageId: string,
      width: number,
      height: number,
      blur: number,
      format: string
    ) => Promise<string>;
    onClick?: () => void;
  }>(),
  {
    direction: Directions.vertical,
    flat: false
  }
);

const polaroid = ref<HTMLDivElement>();
const { width, height } = useElementSize(polaroid);

const loading = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value / 2),
      Math.trunc(height.value / 2),
      20,
      'Jpeg'
    )
);
const fullAvif = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value),
      Math.trunc(height.value),
      0,
      'Avif'
    )
);
const fullWebP = computedAsync(
  async () =>
    await props.loadImage(
      props.image.id,
      Math.trunc(width.value),
      Math.trunc(height.value),
      0,
      'WebP'
    )
);
</script>

<style lang="scss" scoped>
.polaroid {
  &[data-direction='horizontal'] {
    @media (orientation: landscape) {
      max-width: 50dvw;
    }
    @media (orientation: portrait) {
      max-width: 80dvw;
    }

    picture {
      img {
        @media (orientation: landscape) {
          max-height: 50dvh;
          max-width: 50dvw;
        }
        @media (orientation: portrait) {
          max-height: 65dvh;
          max-width: 80dvw;
        }
      }
    }
  }

  &[data-direction='vertical'] {
    max-width: 27rem;

    picture {
      img {
        max-height: 96rem;
        max-width: 96rem;
      }
    }
  }

  picture {
    img {
      min-width: 20rem;
    }
  }
}
</style>
