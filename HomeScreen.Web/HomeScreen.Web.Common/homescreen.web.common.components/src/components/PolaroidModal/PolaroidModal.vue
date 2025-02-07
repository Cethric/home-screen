<template>
  <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
    <template #activator="props">
      <PolaroidCard
        :image="image"
        v-bind="props"
        :class="$attrs['class']"
        :style="$attrs['style']"
        :max-size="maxSize"
      >
        <template #details="{ image }">
          <p class="text-center">
            {{ image.dateTime.isValid ? image.dateTime.toFormat('DDDD') : '' }}
            &nbsp;
            {{ image.dateTime.isValid ? image.dateTime.toFormat('TTT') : '' }}
          </p>
        </template>
      </PolaroidCard>
    </template>
    <template #header-center>
      {{ image.dateTime.toFormat('DDDD') }}
      &nbsp;
      {{ image.dateTime.toFormat('TTT') }}
    </template>
    <template #default>
      <ModalDialog>
        <template #activator="props">
          <PolaroidCard
            :direction="Directions.horizontal"
            :flat="true"
            :image="image"
            :max-size="fullSize"
            v-bind="props"
          >
            <template #details="{ image }">
              <LeafletMapAsync
                v-if="image.location?.latitude && image.location?.longitude"
                :latitude="image.location.latitude"
                :longitude="image.location.longitude"
                :tooltip="image.location.name"
              />
              <p v-else>No location recorded {{ JSON.stringify(image) }}</p>
            </template>
          </PolaroidCard>
        </template>
        <template #header-center>
          {{ image.dateTime.isValid ? image.dateTime.toFormat('DDDD') : '' }}
          &nbsp;
          {{ image.dateTime.isValid ? image.dateTime.toFormat('TTT') : '' }}
        </template>
        <template #default>
          <HSImage
            :id="image.id"
            :date-time="image.dateTime"
            :enabled="image.enabled"
            :aspect-ratio="image.aspectRatio"
            :portrait="image.portrait"
            :colour="image.colour"
            :size="size"
            rounded
          />
        </template>
      </ModalDialog>
    </template>
  </ModalDialog>
</template>

<script lang="ts" setup>
import { Directions } from '@/components/properties';
import { LeafletMapAsync } from '@/components/LeafletMap/LeafletMapAsync';
import PolaroidCard from '@/components/PolaroidCard/PolaroidCard.vue';
import ModalDialog from '@/components/ModalDialog/ModalDialog.vue';
import { type Image } from '@/helpers/image';
import HSImage from '@/components/HSImage/HSImage.vue';
import { useImageSize } from '@/helpers/size';

const props = withDefaults(
  defineProps<{
    image: Image;
    maxSize?: number;
  }>(),
  { maxSize: 0 },
);
const emits = defineEmits<{ resume: []; pause: [] }>();

const { size } = useImageSize({ image: props.image, maxSize: props.maxSize });
const { size: fullSize } = useImageSize({ image: props.image });
</script>
