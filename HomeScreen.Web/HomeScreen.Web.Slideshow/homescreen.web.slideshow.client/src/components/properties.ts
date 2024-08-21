import type { Image } from '@homescreen/web-common-components';

export enum RollingDirections {
  'forward' = 'forward',
  'backward' = 'backward',
}

export type RollingDirection = keyof typeof RollingDirections;

export enum DateTimeWeatherComboKinds {
  'header' = 'header',
  'footer' = 'footer',
}

export type DateTimeWeatherComboKind = keyof typeof DateTimeWeatherComboKinds;

export interface PolaroidImage {
  image: Image;
  top: number;
  left: number;
  rotation: number;
}
