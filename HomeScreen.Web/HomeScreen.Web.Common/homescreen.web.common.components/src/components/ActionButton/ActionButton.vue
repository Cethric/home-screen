<template>
  <button
    :class="[
      'btn',
      {
        'btn-neutral':
          variant === undefined,
        'btn-primary':
          variant === Variants.primary,
        'btn-secondary':
          variant === Variants.secondary,
      },
    ]"
  >
    <slot />
  </button>
</template>

<script lang="ts" setup>
import { type Variant, Variants } from "@/components/properties";

defineProps<{ variant?: Variant }>();
</script>
