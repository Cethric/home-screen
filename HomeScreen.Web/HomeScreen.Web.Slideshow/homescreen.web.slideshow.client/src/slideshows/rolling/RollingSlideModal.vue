<template>
  <div
    ref="sliderModal"
    :class="[
      'flex items-center justify-center',
      {
        'h-full w-auto grow px-4': direction === Directions.horizontal,
        'col-span-1 row-span-1': direction === Directions.vertical,
      },
    ]"
  >
    <ModalDialog
      v-if="isVisible"
      @hide="() => emits('resume')"
      @show="() => emits('pause')"
    >
      <template #activator="props">
        <HSImage
          :id="image.id"
          :aspect-ratio="image.aspectRatio"
          :colour="image.colour"
          :date-time="image.dateTime"
          :enabled="image.enabled"
          :portrait="image.portrait"
          :rounded="false"
          :size="size / 2"
          class="hover:shadow-inner active:drop-shadow-lg"
          v-bind="props"
        />
      </template>

      <template #default>
        <ModalDialog>
          <template #activator="props">
            <PolaroidCard
              :direction="Directions.horizontal"
              :flat="true"
              :image="image"
              v-bind="props"
            >
              <template #details="{ image }">
                <LeafletMapAsync
                  v-if="image.location?.latitude && image.location?.longitude"
                  :latitude="image.location.latitude"
                  :longitude="image.location.longitude"
                  :tooltip="image.location.name"
                />
              </template>
            </PolaroidCard>
          </template>
          <template #default>
            <HSImage
              :id="image.id"
              :aspect-ratio="image.aspectRatio"
              :colour="image.colour"
              :date-time="image.dateTime"
              :enabled="image.enabled"
              :portrait="image.portrait"
              :rounded="false"
              :size="512"
              class="hover:shadow-inner active:drop-shadow-lg"
              v-bind="props"
            />
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
    <div v-else :style="{'--imageAspect': image.aspectRatio, maxWidth: `${size}px`, minWidth: `${size}px`, width: `${size}px`}" class="aspect-(--imageAspect) h-auto rounded-md object-contain drop-shadow-md" />
  </div>
</template>

<script async lang="ts" setup>
import {
	type Direction,
	Directions,
	HSImage,
	type Image,
	LeafletMapAsync,
	ModalDialog,
	PolaroidCard,
} from "@homescreen/web-common-components";
import { useElementVisibility } from "@vueuse/core";
import { ref, toValue } from "vue";

const props = defineProps<{
	image: Image;
	direction: Direction;
	imageSize: number;
}>();

const emits = defineEmits<{ pause: []; resume: [] }>();

const sliderModal = ref<HTMLElement>();
const isVisible = useElementVisibility(sliderModal, {
	threshold: 1,
});

const size = toValue(props.imageSize);
</script>
