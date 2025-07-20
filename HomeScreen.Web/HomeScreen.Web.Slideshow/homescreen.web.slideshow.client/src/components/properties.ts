export enum RollingDirections {
  forward = 'forward',
  backward = 'backward',
}

export type RollingDirection = keyof typeof RollingDirections;

export enum DateTimeWeatherComboKinds {
  header = 'header',
  footer = 'footer',
}

export type DateTimeWeatherComboKind = keyof typeof DateTimeWeatherComboKinds;
