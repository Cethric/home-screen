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
  type ApiClient,
  getCommonApi,
  injectCommonApi,
} from '@/domain/client/api';
import {
  getMediaClient,
  type IMediaClient,
  injectMediaApi,
  MediaApiProvider,
  MediaClient,
  type MediaItem,
  type MediaTransformOptionsFormat,
} from '@/domain/client/media';
import {
  getWeatherClient,
  injectWeatherApi,
  type IWeatherClient,
  WeatherApiProvider,
  WeatherClient,
  type WeatherForecast,
} from '@/domain/client/weather';
import {
  type Config,
  ConfigApiProvider,
  ConfigClient,
  getConfigClient,
  type IConfigClient,
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

import { WmoWeatherCode } from '@/domain/generated/schema';

import { otel } from '@/helpers/otel';

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
  getWeatherClient,
  injectWeatherApi,
  WeatherApiProvider,
  getConfigClient,
  injectConfigApi,
  ConfigApiProvider,
  type Config,
  ConfigClient,
  type IConfigClient,
  type IMediaClient,
  type MediaItem,
  type IWeatherClient,
  type WeatherForecast,
  MediaClient,
  type MediaTransformOptionsFormat,
  WeatherClient,
  WmoWeatherCode,
  getCommonApi,
  injectCommonApi,
  type ApiClient,
  otel,
};
