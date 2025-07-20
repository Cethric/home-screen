<template>
  <PolaroidModal
    :image="item.image"
    :style="{
      transform: `translate(
      calc(${item.left} * 1dvw),
      calc(${item.top} * 1dvh)
    )
    rotate(calc(${item.rotation} * 1deg))`,
    }"
    class="absolute"
    @pause="() => emits('pause')"
    @resume="() => emits('resume')"
    :max-size="750"
  />
</template>

<script lang="ts" setup>
import PolaroidModal from '@/components/PolaroidModal/PolaroidModal.vue';
import type { PolaroidImage } from '@/components/PolaroidModal/types';

defineProps<{
  item: PolaroidImage;
}>();
const emits = defineEmits<{ resume: []; pause: [] }>();
</script>
