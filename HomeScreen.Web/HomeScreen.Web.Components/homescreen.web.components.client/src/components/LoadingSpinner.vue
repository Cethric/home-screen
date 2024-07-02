<template>
  <div class="flex grow items-center justify-center">
    <span class="sr-only">Loading...</span>
    <div
      :class="[
        'm-1 size-8 animate-bounce rounded-full bg-black drop-shadow-md [animation-delay:-0.3s]',
        {
          'bg-orange-500': variant === Variants.primary,
          'bg-purple-500': variant === Variants.secondary,
        },
        variant,
      ]"
    />
    <div
      :class="[
        'm-1 size-8 animate-bounce rounded-full bg-black drop-shadow-md [animation-delay:-0.15s]',
        {
          'bg-orange-500': variant === Variants.primary,
          'bg-purple-500': variant === Variants.secondary,
        },
        variant,
      ]"
    />
    <div
      :class="[
        'm-1 size-8 animate-bounce rounded-full bg-black drop-shadow-md',
        {
          'bg-orange-500': variant === Variants.primary,
          'bg-purple-500': variant === Variants.secondary,
        },
        variant,
      ]"
    />
  </div>
</template>

<script lang="ts" setup>
import { type Variant, Variants } from './properties';

defineProps<{ variant?: Variant }>();
</script>
