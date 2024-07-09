import type { DateTime } from 'luxon';

export enum Variants {
  primary = 'primary',
  secondary = 'secondary',
}

export type Variant = keyof typeof Variants;

export enum Directions {
  horizontal = 'horizontal',
  vertical = 'vertical',
}

export type Direction = keyof typeof Directions;

export interface Image {
  id: string;
  dateTime: DateTime;
  enabled: boolean;
  location?: {
    name: string;
    latitude: string;
    longitude: string;
  };
}
