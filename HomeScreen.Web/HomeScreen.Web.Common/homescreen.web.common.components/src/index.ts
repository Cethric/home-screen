import './styles/_root.scss';

import PolaroidCard from './components/PolaroidCard.vue';
import LeafletMap from './components/LeafletMap.vue';
import { LeafletMapAsync } from './components/LeafletMapAsync';
import ModalDialog from './components/ModalDialog.vue';
import LoadingSpinner from './components/LoadingSpinner.vue';
import ActionButton from './components/ActionButton.vue';
import ResponsiveImage from './components/ResponsiveImage.vue';
import ResponsiveImageSuspense from './components/ResponsiveImageSuspense.vue';
import { ResponsiveImageSuspenseAsync } from './components/ResponsiveImageSuspenseAsync';
import {
  type Direction,
  Directions,
  type Image,
  type Variant,
  Variants,
} from './components/properties';
import type {
  ComputedMediaSize,
  LoadImageCallback,
} from './helpers/computedMedia';

import {
  getMediaClient,
  type IMediaClientWithStreaming,
} from './domain/client/media';
import { getWeatherClient } from './domain/client/weather';
import { getConfigClient } from './domain/client/config';

import {
  Config,
  ConfigClient,
  type IConfig,
  type IConfigClient,
  type IMediaClient,
  type IMediaItem,
  type IWeatherClient,
  type IWeatherForecast,
  MediaClient,
  MediaItem,
  MediaTransformOptionsFormat,
  WeatherClient,
  WeatherForecast,
  WmoWeatherCode,
} from './domain/generated/homescreen-common-api';

export {
  PolaroidCard,
  LeafletMap,
  LeafletMapAsync,
  ModalDialog,
  LoadingSpinner,
  ActionButton,
  ResponsiveImage,
  ResponsiveImageSuspense,
  ResponsiveImageSuspenseAsync,
  type Direction,
  Directions,
  type Image,
  type Variant,
  Variants,
  type ComputedMediaSize,
  type LoadImageCallback,
  getMediaClient,
  type IMediaClientWithStreaming,
  getWeatherClient,
  getConfigClient,
  Config,
  ConfigClient,
  type IConfig,
  type IConfigClient,
  type IMediaClient,
  type IMediaItem,
  type IWeatherClient,
  type IWeatherForecast,
  MediaClient,
  MediaItem,
  MediaTransformOptionsFormat,
  WeatherClient,
  WeatherForecast,
  WmoWeatherCode,
};
