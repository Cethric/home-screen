<template>
  <button
    :class="[
      'rounded-md p-2 drop-shadow focus:outline-none',
      {
        'bg-neutral-500 text-stone-100 hover:bg-neutral-600 hover:text-stone-50 active:bg-neutral-900 active:text-stone-50':
          variant === undefined,
        'bg-orange-500 text-neutral-100 hover:bg-orange-600 hover:text-neutral-50 active:bg-orange-900 active:text-neutral-50':
          variant === Variants.primary,
        'bg-purple-500 text-neutral-100 hover:bg-purple-600 hover:text-neutral-50 active:bg-purple-900 active:text-neutral-50':
          variant === Variants.secondary,
      },
    ]"
  >
    <slot />
  </button>
</template>

<script lang="ts" setup>
import { type Variant, Variants } from './properties';

defineProps<{ variant?: Variant }>();
</script>
