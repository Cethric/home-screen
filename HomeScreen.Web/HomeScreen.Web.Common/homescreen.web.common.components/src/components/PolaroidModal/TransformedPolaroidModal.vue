<template>
  <PolaroidModal
    :image="item.image"
    :style="{
      '--offset-top': item.top,
      '--offset-left': item.left,
      '--rotation': item.rotation,
    }"
    class="polaroid-modal absolute size-fit"
    @pause="() => emits('pause')"
    @resume="() => emits('resume')"
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

<style lang="scss" scoped>
.polaroid-modal {
  transform: translate(
      calc(var(--offset-left) * 1dvw),
      calc(var(--offset-top) * 1dvh)
    )
    rotate(calc(var(--rotation) * 1deg));
}
</style>
