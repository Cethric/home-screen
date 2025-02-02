<template>
  <div ref="galleryImage" class="flex size-auto items-center justify-center">
    <PolaroidModal v-if="isVisible" ref="polaroidModal" :image="image" />
    <div v-else :style="styledSize" />
  </div>
</template>

<script lang="ts" setup>
import {
  type Image,
  PolaroidModal,
  useImageAspectSize,
} from '@homescreen/web-common-components';
import { computed, ref, toValue, watch } from 'vue';
import {
  type MaybeElement,
  useElementSize,
  useElementVisibility,
} from '@vueuse/core';

const props = defineProps<{ image: Image }>();

const galleryImage = ref<HTMLDivElement>();
const polaroidModal = ref<MaybeElement>();
const isVisible = useElementVisibility(galleryImage, {
  threshold: [0],
});
const polaroidSize = useElementSize(polaroidModal, { width: 0, height: 0 });
const polaroidImageSize = ref<{ width: string; height: string }>({
  width: '',
  height: '',
});

watch(polaroidSize.width, (value) => {
  if (value > 0) {
    polaroidImageSize.value.width = `${value}px`;
  }
});
watch(polaroidSize.height, (value) => {
  if (value > 0) {
    polaroidImageSize.value.height = `${value}px`;
  }
});

const size = useImageAspectSize({
  image: props.image,
  size: { width: 500, height: 500 },
  minSize: { width: 0, height: 0 },
});
const aspectSize = computed(() => {
  const { width, height } = toValue(size);
  return { width: `${width + 16}px`, height: `${height + 75}px` };
});
const styledSize = computed(() => {
  let { width, height } = toValue(aspectSize);
  const { width: pWidth, height: pHeight } = toValue(polaroidImageSize);
  if (pWidth.trim().length > 0 && pHeight.trim().length > 0) {
    width = pWidth;
    height = pHeight;
  }
  return {
    minWidth: width,
    minHeight: height,
    maxWidth: width,
    maxHeight: height,
    width: width,
    height: height,
  };
});
</script>
