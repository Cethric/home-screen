<template>
  <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
    <template #activator="props">
      <PolaroidCard
        :class="$attrs['class']"
        :image="image"
        :max-size="maxSize"
        :style="$attrs['style']"
        class="cursor-pointer bg-neutral text-neutral-100"
        v-bind="props"
      >
        <template #title="{ image }">
          {{
            image.dateTime.isValid
              ? image.dateTime.toFormat("DDDD TTT")
              : JSON.stringify(image)
          }}
        </template>
        <template #details="{ image }">
          <p class="text-center wrap-break-word max-w-md">
            {{ image.location?.name }}
          </p>
        </template>
      </PolaroidCard>
    </template>
    <template #header>
      <h3 class="text-center">{{ image.dateTime.toFormat("DDDD TTT") }}</h3>
    </template>
    <template #default>
      <ModalDialog>
        <template #activator="props">
          <PolaroidCard
            :direction="Directions.horizontal"
            :flat="true"
            :image="image"
            :max-size="size"
            v-bind="props"
          >
            <template #title="{ image }">
              {{ image.location?.name }}
            </template>
            <template #details="{ image }">
              <LeafletMapAsync
                v-if="image.location?.latitude && image.location?.longitude"
                :latitude="image.location.latitude"
                :longitude="image.location.longitude"
                :tooltip="image.location.name.split(',')[0]"
              />
              <p v-else class="text-center">No location recorded</p>
            </template>
          </PolaroidCard>
        </template>
        <template #header>
          <h3 class="text-center">{{ image.dateTime.toFormat("DDDD TTT") }}</h3>
        </template>
        <template #default>
          <HSImage
            :id="image.id"
            :aspect-ratio="image.aspectRatio"
            :colour="image.colour"
            :date-time="image.dateTime"
            :enabled="image.enabled"
            :portrait="image.portrait"
            :size="size"
            rounded
          />
        </template>
      </ModalDialog>
    </template>
  </ModalDialog>
</template>

<script lang="ts" setup>
import HSImage from "@/components/HSImage/HSImage.vue";
import { LeafletMapAsync } from "@/components/LeafletMap/LeafletMapAsync";
import ModalDialog from "@/components/ModalDialog/ModalDialog.vue";
import PolaroidCard from "@/components/PolaroidCard/PolaroidCard.vue";
import { Directions } from "@/components/properties";
import type { Image } from "@/helpers/image";

withDefaults(
  defineProps<{
    image: Image;
    maxSize?: number;
  }>(),
  { maxSize: 250 },
);
const emits = defineEmits<{ resume: []; pause: [] }>();

const size = window.innerWidth * devicePixelRatio;
</script>
