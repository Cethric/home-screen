<template>
  <template v-if="isReady">
    <component
      :is="currentSlideshow"
      :count="currentCount"
      :direction="Directions.random"
      :images="imageIds"
      :total="currentTotal"
      :weather-forecast="forecast"
    />
  </template>
  <FullscreenMainLoader v-else />
</template>

<script lang="ts" setup>
import {
  Directions,
  type Image,
  injectComponentMediaClient,
  type MediaItem,
  transformMediaItemToImage,
  type WeatherForecast,
} from "@homescreen/web-common-components";
import { useAsyncState, useIntervalFn } from "@vueuse/core";
import { useNProgress } from "@vueuse/integrations";
import {
  computed,
  nextTick,
  onBeforeUnmount,
  onMounted,
  ref,
  toValue,
} from "vue";
import FullscreenMainLoader from "@/slideshows/fullscreen/FullscreenMainLoader.vue";
import { useCurrentSlideshow } from "@/slideshows/useCurrentSlideshow";
import type { Slideshow } from "./properties";

const props = defineProps<{
  activeSlideshow: Slideshow;
  forecast: WeatherForecast;
}>();

const { currentSlideshow, currentTotal, currentCount } = useCurrentSlideshow({
  activeSlideshow: props.activeSlideshow,
});

const images = ref<Image[]>([]);
const imageIds = computed<Record<Image["id"], Image>>(() =>
  images.value.reduce<Record<Image["id"], Image>>((p, c) => {
    p[c.id] = c;
    return p;
  }, {}),
);

const abort = ref<AbortController>(new AbortController());
const { isLoading, progress } = useNProgress(0, {
  trickle: false,
  minimum: 0.0,
  speed: 0,
});

const mediaApi = injectComponentMediaClient();

const { execute, isReady } = useAsyncState(
  async (signal?: AbortSignal) => {
    console.log(`Loading ${currentTotal.value} images`);
    isLoading.value = true;
    progress.value = 0.0;

    await nextTick(() => {
      images.value = [];
    });
    let loaded = 0;
    let media: AsyncGenerator<MediaItem | undefined> =
      (async function* () {})();
    try {
      media = mediaApi.random(currentTotal.value, signal);
    } catch (e) {
      console.error("Failed to load images", e);
    }
    for await (const item of media) {
      signal?.throwIfAborted();
      if (item && item.id !== undefined) {
        const duplicateIndex = images.value.findIndex(
          (img: Image) => img.id === item.id,
        );
        if (duplicateIndex >= 0) {
          if (images.value.length >= currentTotal.value) {
            const removed = images.value.splice(duplicateIndex, 1);
            console.log("Removed duplicate image", removed);
          } else {
            console.warn("Duplicate image found, skipping", item);
            ++loaded;
            progress.value = loaded / currentTotal.value;
            continue;
          }
        } else if (images.value.length >= currentTotal.value) {
          const removed = images.value.pop();
          console.log("Removed image", removed);
        }
        await nextTick(() => {
          images.value.push(
            transformMediaItemToImage(item as Required<MediaItem>),
          );
          ++loaded;
          progress.value = loaded / currentTotal.value;
        });
      }
    }
    console.log("loaded images", toValue(images));

    isLoading.value = false;
  },
  undefined,
  { immediate: false },
);

onMounted(() => {
  execute(0, abort.value.signal);
});

onBeforeUnmount(() => {
  abort.value.abort("Navigated away from page");
});

useIntervalFn(
  () => {
    abort.value.abort("Interval reached");
    abort.value = new AbortController();
    execute(0, abort.value.signal);
  },
  10 * 60 * 1000,
  { immediate: true },
);
</script>
