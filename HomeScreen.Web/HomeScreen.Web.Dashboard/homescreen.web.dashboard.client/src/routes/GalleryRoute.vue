<template>
  <div
    ref="element"
    class="flex flex-row flex-wrap items-center justify-center gap-2 overflow-y-auto overflow-x-clip"
  >
    <GalleryImage v-for="image in images" :key="image.id" :image="image" />
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import {
  type Image,
  injectMediaApi,
  transformMediaItemToImage,
} from '@homescreen/web-common-components';
import { useNProgress } from '@vueuse/integrations';
import { useInfiniteScroll } from '@vueuse/core';
import GalleryImage from '@/components/Gallery/GalleryImage.vue';

const mediaApi = injectMediaApi();
const images = ref<Image[]>([]);
const { isLoading, progress } = useNProgress(0, {
  trickle: false,
  minimum: 0.0,
  speed: 0,
});

const currentTotal = ref<number>(1);
const element = ref<HTMLDivElement | null>(null);

useInfiniteScroll(
  element,
  async () => {
    console.log('Loading more images');
    isLoading.value = true;
    progress.value = 0;

    const media = mediaApi.paginate(images.value.length, 1000);

    let loaded = 0;
    for await (const item of media) {
      if (item.mediaItem) {
        const transformed = transformMediaItemToImage(item.mediaItem);
        console.log('Loaded image', item.mediaItem, transformed);
        images.value.push(transformed);
        progress.value = ++loaded / 20;
      }
      currentTotal.value = item.totalPages ?? 0;
    }

    progress.value = 1;
    isLoading.value = false;
  },
  {
    distance: 10,
    canLoadMore: () => images.value.length < currentTotal.value,
    idle: 900,
  },
);
</script>
