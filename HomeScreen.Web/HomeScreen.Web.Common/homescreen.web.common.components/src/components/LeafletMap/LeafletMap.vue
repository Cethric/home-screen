<template>
  <div class="size-11/12">
    <l-map
      ref="map"
      v-model:zoom="zoom"
      :center="[centre.geometry.coordinates[1], centre.geometry.coordinates[0]]"
      :use-global-leaflet="false"
    >
      <l-tile-layer
        layer-type="base"
        name="OpenStreetMap"
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <l-geo-json :geojson="location">
        <l-tooltip v-if="tooltip">{{ tooltip }}</l-tooltip>
      </l-geo-json>
    </l-map>
  </div>
</template>

<script lang="ts" setup>
import { centroid, point } from '@turf/turf';
import { LGeoJson, LMap, LTileLayer, LTooltip } from '@vue-leaflet/vue-leaflet';
import { computed, toValue } from 'vue';

const zoom = defineModel({ default: 15 });
const props = defineProps<{
  latitude: number;
  longitude: number;
  tooltip?: string;
}>();

const location = computed(() => point([props.longitude, props.latitude], {}));
const centre = computed(() => centroid(toValue(location)));
</script>
