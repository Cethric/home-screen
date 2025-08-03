<template>
  <header class="sticky inset-0 top-0 shrink-0 grow-0 z-99">
    <nav class="navbar bg-amber-800 shadow-sm">
      <div class="navbar-start">
        <div class="dropdown">
          <button class="btn btn-ghost" tabindex="0" type="button" @click="">
            <font-awesome-icon :icon="faBars"/>
          </button>
          <ul
            class="menu menu-sm dropdown-content bg-amber-800 rounded-box z-1 mt-3 w-52 p-2 shadow">
            <nav-link :to="{name : 'Dashboard'}" active-class="" inactive-class="">Dashboard</nav-link>
            <nav-link :to="config?.slideshowUrl ?? ''" active-class="" inactive-class="">Slideshow</nav-link>
            <nav-link :to="{name : 'Gallery'}" active-class="" inactive-class="">Gallery</nav-link>
          </ul>
        </div>
        <nav-link :to="{name: 'Dashboard'}" active-class="" class="btn btn-ghost text-xl" inactive-class="">HomeScreen</nav-link>
      </div>
    </nav>
  </header>
</template>

<script lang="ts" setup>
import { faBars } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import NavLink from '@/components/Header/NavLink.vue';
import { injectConfig } from '@/domain/client/config';

const config = injectConfig();
</script>
