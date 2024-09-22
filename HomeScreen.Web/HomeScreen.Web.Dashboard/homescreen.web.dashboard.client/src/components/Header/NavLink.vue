<template>
  <li>
    <a
      v-if="isExternalLink"
      :class="inactiveClass"
      :href="to as string"
      rel="opener external noreferrer nofollow"
      target="_self"
      v-bind="$attrs"
    >
      <slot />
    </a>
    <router-link
      v-else
      v-slot="{ isActive, href, navigate }"
      custom
      v-bind="$props"
    >
      <a
        :class="isActive ? activeClass : inactiveClass"
        :href="href"
        v-bind="$attrs"
        @click="navigate"
      >
        <slot />
      </a>
    </router-link>
  </li>
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import { type RouterLinkProps } from 'vue-router';

defineOptions({
  inheritAttrs: false,
});

const props = defineProps<RouterLinkProps & { inactiveClass: string }>();

const isExternalLink = computed(() => {
  return typeof props.to === 'string' && props.to.startsWith('http');
});
</script>
