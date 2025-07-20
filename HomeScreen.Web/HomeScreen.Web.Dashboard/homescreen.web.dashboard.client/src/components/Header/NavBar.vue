<template>
  <header class="sticky inset-x-0 top-0 shrink-0 grow-0 bg-amber-800 px-4 py-2">
    <nav class="bg-amber-800">
      <ul class="flex list-none flex-row gap-2">
        <NavLink
          :to="{ name: 'Dashboard' }"
          active-class="text-stone-200 hover:text-stone-100"
          inactive-class="text-stone-300 hover:text-stone-50"
        >
          Dashboard
        </NavLink>
        <NavLink
          :to="config?.slideshowUrl ?? ''"
          active-class="text-stone-200 hover:text-stone-100"
          inactive-class="text-stone-300 hover:text-stone-50"
        >
          Slideshow
        </NavLink>
        <NavLink
          :to="{ name: 'Gallery' }"
          active-class="text-stone-200 hover:text-stone-100"
          inactive-class="text-stone-300 hover:text-stone-50"
        >
          Gallery
        </NavLink>
      </ul>
    </nav>
  </header>
</template>

<script lang="ts" setup>
import NavLink from "@/components/Header/NavLink.vue";
import { injectConfig } from "@/domain/client/config";

const config = injectConfig();
</script>
