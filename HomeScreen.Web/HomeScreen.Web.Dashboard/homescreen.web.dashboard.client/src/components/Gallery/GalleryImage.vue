<template>
  <div ref="galleryImage"
       :class="{'opacity-60': !image.enabled}"
       :style="{
          '--imageAspect': image.aspectRatio,
          maxWidth: `${deviceSize}px`,
          minWidth: `${deviceSize}px`,
          maxHeight: '85dvh'
        }"
       class="aspect-(--imageAspect) size-auto drop-shadow-md"
    >
    
    <polaroid-modal v-if="isVisible" :image="image" :maxSize="size" />

    <div v-else class="card">
      <div :style="{
          '--imageAspect': image.aspectRatio,
          '--color': color,
          maxWidth: `${deviceSize}px`,
          maxHeight: '85dvh'
        }"
           class="aspect-(--imageAspect) size-auto bg-(--color) drop-shadow-md" />
      <div class="card-body">
        <h2 class="card-title wrap-break-word text-center">
          {{ image.dateTime.isValid ? image.dateTime.toFormat('DDDD TTT') : JSON.stringify(image) }}
        </h2>
        <p class="text-center wrap-break-word max-w-md">
          {{ image.location?.name }}
        </p>
      </div>
    </div>
    
    <div class="absolute top-0 right-0 w-full h-fit z-99 bg-slate-700 p-1 hover:opacity-100 opacity-0 rounded-xl flex flex-row justify-between">
      <input :checked="image.enabled" class="toggle toggle-secondary" type="checkbox" @change="toggleMedia" />
    </div>
    
  </div>
</template>

<script lang="ts" setup>
import {
	type Image,
	injectComponentMediaClient,
	PolaroidModal,
} from "@homescreen/web-common-components";
import { useElementVisibility } from "@vueuse/core";
import { computed, ref } from "vue";

const props = withDefaults(defineProps<{ image: Image; size: number }>(), {
	size: 450,
});

const deviceSize = props.size / window.devicePixelRatio;
const color = computed(
	() =>
		`rgb(${props.image.colour.red}, ${props.image.colour.green}, ${props.image.colour.blue})`,
);

const galleryImage = ref<HTMLDivElement>();
const isVisible = useElementVisibility(galleryImage, {
	threshold: [0],
});

const mediaApi = injectComponentMediaClient();

function toggleMedia() {
	mediaApi.toggle(props.image.id, !props.image.enabled).then(() => {
		props.image.enabled = !props.image.enabled;
	});
}
</script>
