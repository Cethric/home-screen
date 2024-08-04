<template>
  <transition-group
    class="relative size-full grow"
    enter-active-class="animate__animated animate__fadeIn"
    leave-active-class="animate__animated animate__fadeOut"
    name="fullscreen-images"
    tag="div"
  >
    <div
      v-for="(imageId, idx) in activeItems"
      v-show="idx === 0"
      :key="imageId"
      class="absolute left-1/2 top-1/2 flex size-full -translate-x-1/2 -translate-y-1/2 items-center justify-center p-2"
    >
      <FullscreenModal
        :image="images[imageId]"
        :load-image="loadImage"
        @pause="() => pause()"
        @resume="() => resume()"
      />
    </div>
  </transition-group>
</template>

<script lang="ts" setup>
import FullscreenModal from '@/components/fullscreen/FullscreenModal.vue';
import { useIntervalFn } from '@vueuse/core';
import { computed, ref } from 'vue';
import type {
  Image,
  LoadImageCallback,
} from '../../../../../HomeScreen.Web.Components/homescreen.web.components.client/src';

const props = withDefaults(
  defineProps<{
    images: Record<Image['id'], Image>;
    intervalSeconds?: number;
    loadImage: LoadImageCallback;
    length: number;
    offset: number;
  }>(),
  {
    intervalSeconds: 24,
  },
);

const index = ref<number>(0);
const imageIds = computed(() =>
  Object.keys(props.images).slice(
    props.length * (props.offset - 1),
    props.length + props.length * (props.offset - 1),
  ),
);
const currentId = ref<Image['id']>(imageIds.value[index.value]);
const nextId = ref<Image['id']>(
  imageIds.value[(index.value + 1) % props.length],
);

const { pause, resume } = useIntervalFn(() => {
  index.value = (index.value + 1) % (props.length ?? 1);
  currentId.value = imageIds.value[index.value];
  nextId.value = imageIds.value[(index.value + 1) % props.length];
  console.log('Next image', index.value, currentId.value, nextId.value);
}, props.intervalSeconds * 1000);

const activeItems = computed(() =>
  currentId.value && nextId.value ? [currentId.value, nextId.value] : [],
);
</script>
