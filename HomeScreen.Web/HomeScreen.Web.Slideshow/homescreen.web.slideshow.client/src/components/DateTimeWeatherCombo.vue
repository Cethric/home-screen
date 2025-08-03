<template>
  <component
    :is="kind"
    :class="[
      'fixed z-50 w-98 max-w-110 bg-stone-400/40 p-4 text-ellipsis drop-shadow-md backdrop-blur',
      {
        'top-0 left-0 rounded-br-2xl':
          kind === DateTimeWeatherComboKinds.header,
        'right-0 bottom-0 rounded-tl-2xl':
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
        'w-full text-5xl font-extrabold text-neutral-50 tabular-nums',
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
import type { WeatherForecast } from '@homescreen/web-common-components';
import { useDateFormat, useNow } from '@vueuse/core';
import { type DateTimeWeatherComboKind, DateTimeWeatherComboKinds } from '@/components/properties';

defineProps<{
  weatherForecast: WeatherForecast;
  kind: DateTimeWeatherComboKind;
}>();

const now = useNow();
const timeFormat = useDateFormat(now, 'HH:mm');
const dayFormat = useDateFormat(now, 'MMMM Do YYYY');
</script>
