<template>
  <div>
    <ModalDialog @hide="() => emits('resume')" @show="() => emits('pause')">
      <template #activator="props">
        <PolaroidCard :image="image" v-bind="props">
          <template #details="{ image }">
            <p class="text-center">
              {{ image.dateTime.toFormat('DDDD') }}
              &nbsp;
              {{ image.dateTime.toFormat('TTT') }}
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
          <template #header-center>
            {{ image.dateTime.toFormat('DDDD') }}
            &nbsp;
            {{ image.dateTime.toFormat('TTT') }}
          </template>
          <template #default>
            <LargeImage :image="image" />
          </template>
        </ModalDialog>
      </template>
    </ModalDialog>
  </div>
</template>

<script lang="ts" setup>
import { Directions } from '@/components/properties';
import { LeafletMapAsync } from '@/components/LeafletMap/LeafletMapAsync';
import PolaroidCard from '@/components/PolaroidCard/PolaroidCard.vue';
import ModalDialog from '@/components/ModalDialog/ModalDialog.vue';
import LargeImage from '@/components/LargeImage/LargeImage.vue';
import type { Image } from '@/components/ResponsivePicture/image';

defineProps<{
  image: Image;
}>();
const emits = defineEmits<{ resume: []; pause: [] }>();
</script>
