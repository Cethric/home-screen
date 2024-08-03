<template>
  <Suspense>
    <SlideshowContainer
      :active-slideshow="activeSlideshow"
      :forecast="forecast"
      :images="imageIds"
      :total="total"
    />

    <template #fallback>
      <FullscreenMainLoader />
    </template>
  </Suspense>
  <div class="fixed bottom-4 left-4 z-50">
    <button
      class="rounded-full bg-stone-400/10 p-4 text-stone-50 backdrop-blur hover:text-stone-200 active:text-stone-300"
      @click="navigateToDashboard"
    >
      <span class="sr-only">Dashboard</span>
      <FontAwesomeIcon :icon="faHome" class="text-4xl" />
    </button>
  </div>
</template>

<script async lang="ts" setup>
import SlideshowContainer from '@/slideshows/SlideshowContainer.vue';
import { useAsyncState, useBrowserLocation, useIntervalFn } from '@vueuse/core';
import { computed, nextTick, onBeforeUnmount, onMounted, ref } from 'vue';
import { type Slideshow, Slideshows } from '@/slideshows/properties';
import { choice } from '@/helpers/random';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faHome } from '@fortawesome/free-solid-svg-icons';
import { type Image } from '@homescreen/web-components-client/src/index';
import { useNProgress } from '@vueuse/integrations';
import { loadWeather as loadWeatherBase } from '@/domain/weather';
import { loadMedia as loadMediaBase } from '@/domain/media';
import FullscreenMainLoader from '@/components/FullscreenMainLoader.vue';

const props = defineProps<{
  total: number;
  loadWeather: typeof loadWeatherBase;
  loadMedia: typeof loadMediaBase;
}>();

const location = useBrowserLocation({});
const activeSlideshow = ref<Slideshow>(choice(Object.values(Slideshows)));

const forecast = await props.loadWeather();

const images = ref<Image[]>([]);
const imageIds = computed<Record<Image['id'], Image>>(() =>
  images.value.reduce(
    (p: Record<Image['id'], Image>, c: Image) => ({ ...p, [c['id']]: c }),
    {},
  ),
);

const abort = ref<AbortController>(new AbortController());
const { isLoading, progress } = useNProgress(0, {
  trickle: false,
  minimum: 0.0,
  speed: 0,
});

const { execute } = useAsyncState(
  async (signal?: AbortSignal) => {
    console.log('Loading images');
    isLoading.value = true;
    progress.value = 0.0;
    let loaded = 0;
    for await (const item of props.loadMedia(props.total, signal)) {
      signal?.throwIfAborted();
      if (
        item &&
        item.id !== undefined &&
        item.created !== undefined &&
        item.enabled !== undefined
      ) {
        if (images.value.length >= props.total) {
          await nextTick(() => {
            images.value.splice(0, 1);
          });
        }
        if (
          images.value.filter((img: Image) => img.id === item.id).length > 0
        ) {
          console.warn('Duplicate key found, skipping', item);
          await nextTick(() => {
            ++loaded;
            progress.value = loaded / props.total;
          });
          continue;
        }
        await nextTick(() => {
          images.value.push({
            id: item.id!,
            dateTime: item.created!,
            enabled: item.enabled!,
            location:
              item.location?.name &&
              item.location?.latitude &&
              item.location?.longitude
                ? {
                    name: item.location!.name,
                    latitude: item.location!.latitude,
                    longitude: item.location!.longitude,
                  }
                : undefined,
          } satisfies Image);
          ++loaded;
          progress.value = loaded / props.total;
        });
      }
    }
    isLoading.value = false;
    console.log('Loaded all images');
  },
  undefined,
  { immediate: false },
);

onMounted(() => {
  execute(0, abort.value.signal);
});

onBeforeUnmount(() => {
  abort.value.abort('Navigated away from page');
});

useIntervalFn(
  () => {
    abort.value.abort('Interval reached');
    abort.value = new AbortController();
    execute(0, abort.value.signal);
  },
  10 * 60 * 1000,
  { immediate: true },
);

useIntervalFn(
  () => {
    const path = location.value.pathname?.replace('/', '');
    if (Object.values(Slideshows).includes(path as Slideshows)) {
      activeSlideshow.value = path as Slideshow;
    } else {
      activeSlideshow.value = choice(Object.values(Slideshows));
    }
    console.log('Loading slideshow variant', activeSlideshow.value);
  },
  15 * 60 * 1000,
  { immediateCallback: true },
);

const navigateToDashboard = () => {
  window.location.assign('https://localhost:5174');
};
</script>
