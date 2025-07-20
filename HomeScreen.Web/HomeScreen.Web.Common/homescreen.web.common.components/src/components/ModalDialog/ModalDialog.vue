<template>
  <teleport to="body">
    <dialog
      v-if="isOpen"
      ref="dialog"
      class="modal"
      @close="() => emits('hide')"
    >
      <div class="modal-box max-w-fit">
        <form method="dialog">
          <button
            class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2"
            type="submit"
          >
            <FontAwesomeIcon :icon="faClose" />
            <span class="sr-only">Close Dialog</span>
          </button>
        </form>
        <header>
          <slot name="header" @click="closeDialog" />
        </header>
        <main>
          <slot name="default" @click="closeDialog" />
        </main>
        <footer class="modal-action">
          <slot name="footer" @click="closeDialog" />
        </footer>
      </div>
      <form method="dialog" class="modal-backdrop">
        <button type="submit">close</button>
      </form>
    </dialog>
  </teleport>
  <slot name="activator" @click="openDialog" />
</template>

<script lang="ts" setup>
import { faClose } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { nextTick, ref } from "vue";

const emits = defineEmits<{ show: []; hide: [] }>();
const isOpen = ref<boolean>(false);
const dialog = ref<HTMLDialogElement>();

const openDialog = () => {
  isOpen.value = true;
  nextTick(() => {
    dialog.value?.showModal();
    emits("show");
  });
};

const closeDialog = () => {
  dialog.value?.close();
  nextTick(() => {
    isOpen.value = false;
  });
};
</script>
