<template>
  <teleport to="body">
    <dialog
      v-if="isOpen"
      ref="dialog"
      class="fixed z-50 m-0 h-dvh w-dvw border-none p-0 outline-none open:grid"
      @close="() => emits('hide')"
    >
      <div
        class="z-50 m-auto flex size-fit flex-col justify-center rounded-2xl bg-neutral-200/25 px-2 drop-shadow-lg backdrop-blur"
      >
        <header class="flex flex-row items-center justify-between">
          <div class="flex grow flex-row items-center justify-start">
            <slot name="header-start" />
          </div>
          <div class="flex grow flex-row items-center justify-center">
            <slot name="header-center" />
          </div>
          <div class="flex grow flex-row items-center justify-end">
            <slot name="header-end" />
            <button
              class="border-none text-stone-800 outline-none hover:border-none hover:text-stone-900 focus:border-none active:text-stone-950"
              @click="closeDialog"
            >
              <FontAwesomeIcon :icon="faClose" />
              <span class="sr-only">Close Dialog</span>
            </button>
          </div>
        </header>
        <main class="flex flex-col items-center justify-center pt-2">
          <slot name="default" @click="closeDialog" />
        </main>
        <footer class="flex flex-row pt-2">
          <slot name="footer" @click="closeDialog" />
        </footer>
      </div>
    </dialog>
  </teleport>
  <slot name="activator" @click="openDialog" />
</template>

<script lang="ts" setup>
import { nextTick, ref } from 'vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faClose } from '@fortawesome/free-solid-svg-icons';

const emits = defineEmits<{ show: []; hide: [] }>();
const isOpen = ref<boolean>(false);
const dialog = ref<HTMLDialogElement>();

const openDialog = () => {
  isOpen.value = true;
  nextTick(() => {
    dialog.value?.showModal();
    emits('show');
  });
};

const closeDialog = () => {
  dialog.value?.close();
  nextTick(() => {
    isOpen.value = false;
  });
};
</script>

<style lang="scss" scoped>
dialog {
  background: none;

  &::backdrop {
    @apply bg-stone-900/60 backdrop-blur-sm;
  }
}
</style>
