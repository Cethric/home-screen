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
  images: string[];
  thumbnail: string;
  width: number;
  height: number;
  dateTime: DateTime;
  location: { longitude: number; latitude: number; name: string };
}
