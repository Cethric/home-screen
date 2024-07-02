<template>
  <div ref="openLayersMapViewDiv" class="h-64 w-auto" />
</template>

<script lang="ts" setup>
import { onBeforeUnmount, onMounted, ref } from 'vue';
import { Feature, Map, View } from 'ol';
import { fromLonLat } from 'ol/proj';
import { Tile, Vector } from 'ol/layer';
import { BingMaps, TileDebug, Vector as SourceVector } from 'ol/source';
import { Icon, Style } from 'ol/style';
import pin from '../assets/baseline_place_white_48dp.png?url';
import { Point } from 'ol/geom';

const props = defineProps<{ latitude: number; longitude: number }>();

const openLayersMapViewDiv = ref<HTMLDivElement>();
const openLayersMap = ref<Map>();

onMounted(() => {
  if (openLayersMapViewDiv.value) {
    openLayersMap.value?.dispose();
    const location = fromLonLat([props.longitude, props.latitude]);
    openLayersMap.value = new Map({
      target: openLayersMapViewDiv.value,
      layers: [
        new Tile({ source: new TileDebug() }),
        // new Tile({
        //   source: new OSM(),
        // }),
        new Tile({
          source: new BingMaps({
            key: import.meta.env.VITE_BING_MAPS_API_KEY,
            imagerySet: 'AerialWithLabelsOnDemand',
          }),
        }),
        new Vector({
          source: new SourceVector({
            features: [new Feature({ geometry: new Point(location) })],
          }),
          style: new Style({
            image: new Icon({
              anchor: [0.5, 0],
              anchorOrigin: 'bottom-left',
              anchorXUnits: 'fraction',
              anchorYUnits: 'fraction',
              src: pin,
              scale: 0.5,
            }),
          }),
        }),
      ],
      view: new View({ center: location, zoom: 15 }),
    });
  }
});
onBeforeUnmount(() => {
  openLayersMap.value?.dispose();
  openLayersMap.value = undefined;
});
</script>

<style lang="scss" scoped></style>
