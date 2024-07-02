<template>
  <DateTimeWeatherCombo
    :kind="DateTimeWeatherComboKinds.header"
    :weather-forecast="weatherForecast"
  />
  <main class="h-dvh w-dvw overflow-hidden">
    <transition-group
      :duration="{ enter: 2 * 1000, leave: 3 * 1000 }"
      class="relative"
      enter-active-class="animate__animated animate__stampIn"
      leave-active-class="animate__animated animate__shrinkOut"
      tag="div"
    >
      <PolaroidCard
        v-for="item in slice"
        :key="item.image.id"
        :image="item.image"
        :location="{
          top: item.top,
          left: item.left,
          rotation: item.rotation,
        }"
      >
        <template #details="{ image }">
          <ModalDialog @hide="() => resume()" @show="() => pause()">
            <template #activator="props">
              <ActionButton :variant="Variants.primary" v-bind="props">
                Details
              </ActionButton>
            </template>
            <template #default>
              <PolaroidCard
                :direction="Directions.horizontal"
                :image="image"
                flat
              >
                <template #details="{ image }">
                  <OpenLayersMap
                    :latitude="image.location.latitude"
                    :longitude="image.location.longitude"
                  />
                </template>
              </PolaroidCard>
            </template>
          </ModalDialog>
        </template>
      </PolaroidCard>
    </transition-group>
  </main>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';
import { Directions, type Image, Variants } from '@components/properties';
import PolaroidCard from '@components/PolaroidCard.vue';
import { useIntervalFn } from '@vueuse/core';
import ModalDialog from '@components/ModalDialog.vue';
import OpenLayersMap from '@components/OpenLayersMap.vue';
import ActionButton from '@components/ActionButton.vue';
import type { IWeatherForecast } from '@/domain/api/homescreen-slideshow-api';
import { range } from '@/helpers/random';
import DateTimeWeatherCombo from '@/components/DateTimeWeatherCombo.vue';
import { DateTimeWeatherComboKinds } from '@/components/properties';

const props = withDefaults(
  defineProps<{
    images: Image[];
    intervalSeconds?: number;
    visibleCount?: number;
    weatherForecast: IWeatherForecast;
  }>(),
  {
    intervalSeconds: 8,
    visibleCount: 40,
  },
);

const items = computed(() =>
  props.images.map((image) => ({
    image,
    top: range(-12.5, 87.5),
    left: range(-6.25, 100),
    rotation: range(-15, 15),
  })),
);

const start = range(0, items.value.length);
const head = ref<number>((start + props.visibleCount) % items.value.length);
const tail = ref<number>(start);
const slice = computed(() =>
  head.value < tail.value
    ? [...items.value.slice(tail.value), ...items.value.slice(0, head.value)]
    : items.value.slice(tail.value, head.value),
);

const { pause, resume } = useIntervalFn(() => {
  tail.value = (tail.value + 1) % items.value.length;
  head.value = (tail.value + props.visibleCount) % items.value.length;
  document.createElement('img').src =
    items.value[head.value + (1 % items.value.length)].image.loading;
}, props.intervalSeconds * 1000);
</script>
