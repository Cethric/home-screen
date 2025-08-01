<template>
  <div class="grow p-4">
    <masonry-wall :items="images" :ssr-columns="1" :min-columns="1" :max-columns="6" :column-width="deviceSize" :gap="7" :key-mapper="itm=>itm.id">
      <template #default="{ item }">
        <div class="p-4" :key="item.id">
          <gallery-image :image="item" :size="size" />
        </div>
      </template>
    </masonry-wall>
    
    <div ref="loaderBox" class="w-full h-24">
      <loading-spinner variant="primary" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
	type Image,
	injectComponentMediaClient,
	LoadingSpinner,
	type MediaItem,
	transformMediaItemToImage,
} from "@homescreen/web-common-components";
import { useIntersectionObserver } from "@vueuse/core";
import { useNProgress } from "@vueuse/integrations";
import MasonryWall from "@yeger/vue-masonry-wall";
import { ref, useTemplateRef } from "vue";
import GalleryImage from "@/components/Gallery/GalleryImage.vue";

const size = 450;
const deviceSize = size / window.devicePixelRatio;

const mediaApi = injectComponentMediaClient();
const images = ref<Image[]>([]);
const { isLoading, progress } = useNProgress(0, {
	trickle: false,
	minimum: 0.0,
	speed: 0,
});

const currentTotal = ref<number>(1);
const loader = useTemplateRef<HTMLDivElement>("loaderBox");

async function loadMedia() {
	const media = mediaApi.paginate(images.value.length, 1000);

	let loaded = 0;
	for await (const item of media) {
		if (item.mediaItem) {
			const transformed = transformMediaItemToImage(
				item.mediaItem as Required<MediaItem>,
			);
			console.log("Loaded image", item.mediaItem, transformed);
			images.value = [...images.value, transformed];
			progress.value = ++loaded / 1000;
		}
		currentTotal.value = item.totalPages ?? 0;
	}
}

useIntersectionObserver(
	loader,
	() => {
		isLoading.value = true;
		progress.value = 0;

		loadMedia().finally(() => {
			isLoading.value = false;
			progress.value = 1;
		});
	},
	{ threshold: [0], immediate: true },
);
</script>
