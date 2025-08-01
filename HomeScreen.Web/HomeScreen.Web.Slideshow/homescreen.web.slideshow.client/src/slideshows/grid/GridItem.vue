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
      class="absolute top-1/2 left-1/2 flex size-full -translate-x-1/2 -translate-y-1/2 items-center justify-center p-2"
    >
      <PolaroidModal
        :image="images[imageId]"
        :max-size="size"
        @pause="() => pause()"
        @resume="() => resume()"
      />
    </div>
  </transition-group>
</template>

<script lang="ts" setup>
import { type Image, PolaroidModal } from "@homescreen/web-common-components";
import { useIntervalFn } from "@vueuse/core";
import { computed, ref } from "vue";

const props = withDefaults(
  defineProps<{
    images: Record<Image["id"], Image>;
    intervalSeconds?: number;
    length: number;
    offset: number;
  }>(),
  {
    intervalSeconds: 24,
  },
);

const size = window.innerWidth - 300;

const index = ref<number>(0);
const imageIds = computed(() =>
  Object.keys(props.images).slice(
    props.length * (props.offset - 1),
    props.length + props.length * (props.offset - 1),
  ),
);
const currentId = ref<Image["id"]>(imageIds.value[index.value]);
const nextId = ref<Image["id"]>(
  imageIds.value[(index.value + 1) % props.length],
);

const { pause, resume } = useIntervalFn(() => {
  index.value = (index.value + 1) % (props.length ?? 1);
  currentId.value = imageIds.value[index.value];
  nextId.value = imageIds.value[(index.value + 1) % props.length];
  console.log("Next image", index.value, currentId.value, nextId.value);
}, props.intervalSeconds * 1000);

const activeItems = computed(() =>
  currentId.value && nextId.value ? [currentId.value, nextId.value] : [],
);
</script>
