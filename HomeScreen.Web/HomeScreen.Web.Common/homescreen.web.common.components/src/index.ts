import '@/styles/_root.scss';

import PolaroidCard from '@/components/PolaroidCard/PolaroidCard.vue';
import PolaroidModal from '@/components/PolaroidModal/PolaroidModal.vue';
import TransformedPolaroidModal from '@/components/PolaroidModal/TransformedPolaroidModal.vue';
import { type PolaroidImage } from '@/components/PolaroidModal/types';
import LargeImage from '@/components/LargeImage/LargeImage.vue';
import LeafletMap from '@/components/LeafletMap/LeafletMap.vue';
import { LeafletMapAsync } from '@/components/LeafletMap/LeafletMapAsync';
import ModalDialog from '@/components/ModalDialog/ModalDialog.vue';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';
import ActionButton from '@/components/ActionButton/ActionButton.vue';
import ResponsivePicture from '@/components/ResponsivePicture/ResponsivePicture.vue';
import { ResponsivePictureAsync } from '@/components/ResponsivePicture/ResponsivePictureAsync';
import {
  type Direction,
  Directions,
  type Variant,
  Variants,
} from '@/components/properties';
import { type LoadImageCallback } from '@/helpers/computedMedia';

import {
  getMediaClient,
  type IMediaClientWithStreaming,
  injectMediaApi,
  MediaApiProvider,
} from '@/domain/client/media';
import {
  getWeatherClient,
  injectWeatherApi,
  WeatherApiProvider,
} from '@/domain/client/weather';
import {
  ConfigApiProvider,
  getConfigClient,
  injectConfigApi,
} from '@/domain/client/config';
import {
  type ComputedMediaSize,
  type Image,
  type ImageColour,
  type ImageLocation,
  imageToColour,
  type MediaSize,
  transformMediaItemToImage,
  useImageAspectSize,
} from '@/components/ResponsivePicture/image';

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
} from '@/domain/generated/homescreen-common-api';

import { configureSentry } from '@/helpers/sentry';

export {
  PolaroidCard,
  PolaroidModal,
  TransformedPolaroidModal,
  LargeImage,
  LeafletMap,
  LeafletMapAsync,
  ModalDialog,
  LoadingSpinner,
  ActionButton,
  ResponsivePicture,
  ResponsivePictureAsync,
  type Direction,
  Directions,
  type Image,
  type ImageColour,
  type ImageLocation,
  type PolaroidImage,
  type MediaSize,
  type Variant,
  Variants,
  injectMediaApi,
  MediaApiProvider,
  useImageAspectSize,
  transformMediaItemToImage,
  imageToColour,
  type ComputedMediaSize,
  type LoadImageCallback,
  getMediaClient,
  type IMediaClientWithStreaming,
  getWeatherClient,
  injectWeatherApi,
  WeatherApiProvider,
  getConfigClient,
  injectConfigApi,
  ConfigApiProvider,
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
  configureSentry,
};
