<template>
  <component
    :is="kind"
    :class="[
      'w-98 fixed z-50 max-w-100 text-ellipsis bg-stone-400/25 p-4 drop-shadow-md backdrop-blur',
      {
        'left-0 top-0 rounded-br-2xl':
          kind === DateTimeWeatherComboKinds.header,
        'bottom-0 right-0 rounded-tl-2xl':
          kind === DateTimeWeatherComboKinds.footer,
      },
    ]"
  >
    <h1
      :class="[
        'w-full text-5xl font-extrabold text-neutral-50',
        {
          'text-left': kind === DateTimeWeatherComboKinds.header,
          'text-right': kind === DateTimeWeatherComboKinds.footer,
        },
      ]"
    >
      {{ dayFormat }}
    </h1>
    <h1
      :class="[
        'w-full text-5xl font-extrabold tabular-nums text-neutral-50',
        {
          'text-left': kind === DateTimeWeatherComboKinds.header,
          'text-right': kind === DateTimeWeatherComboKinds.footer,
        },
      ]"
    >
      {{ timeFormat }}
    </h1>
    <p
      :class="[
        'w-full text-4xl font-bold text-neutral-50',
        {
          'text-left': kind === DateTimeWeatherComboKinds.header,
          'text-right': kind === DateTimeWeatherComboKinds.footer,
        },
      ]"
    >
      {{ weatherForecast.feelsLikeTemperature }}&deg;C
    </p>
    <p
      :class="[
        'w-full text-4xl font-bold text-neutral-50',
        {
          'text-left': kind === DateTimeWeatherComboKinds.header,
          'text-right': kind === DateTimeWeatherComboKinds.footer,
        },
      ]"
    >
      {{ weatherForecast.weatherCode }}
    </p>
  </component>
</template>

<script lang="ts" setup>
import { useDateFormat, useNow } from '@vueuse/core';
import type { IWeatherForecast } from '@/domain/api/homescreen-slideshow-api';
import {
  type DateTimeWeatherComboKind,
  DateTimeWeatherComboKinds,
} from '@/components/properties';

defineProps<{
  weatherForecast: IWeatherForecast;
  kind: DateTimeWeatherComboKind;
}>();

const now = useNow();
const timeFormat = useDateFormat(now, 'HH:mm');
const dayFormat = useDateFormat(now, 'MMMM Do YYYY');
</script>
