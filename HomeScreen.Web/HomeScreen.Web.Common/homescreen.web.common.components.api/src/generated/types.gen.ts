// This file is auto-generated by @hey-api/openapi-ts

export type Config = {
  [key: string]: never;
};

export type MediaItem = {
  id?: string;
  created?: Date;
  notes?: string;
  enabled?: boolean;
  location?: MediaItemLocation;
  aspectRatio?: number;
  portrait?: boolean;
  baseB?: number;
  baseG?: number;
  baseR?: number;
};

export type MediaItemLocation = {
  name?: string;
  latitude?: number;
  longitude?: number;
};

export type PaginatedMediaItem = {
  mediaItem?: MediaItem;
  totalPages: number;
};

export enum MediaTransformOptionsFormat {
  /**
   * Jpeg
   */
  JPEG = 'Jpeg',
  /**
   * JpegXl
   */
  JPEG_XL = 'JpegXl',
  /**
   * Png
   */
  PNG = 'Png',
  /**
   * WebP
   */
  WEB_P = 'WebP',
  /**
   * Avif
   */
  AVIF = 'Avif',
}

export type AcceptedTransformMediaItem = {
  mediaId?: string;
  width?: number;
  height?: number;
  blur?: boolean;
  format?: MediaTransformOptionsFormat;
  url?: string;
};

export type AcceptedTransformMediaLine = {
  [key: string]: never;
};

export type WeatherForecast = {
  feelsLikeTemperature?: number;
  maxTemperature?: number;
  minTemperature?: number;
  chanceOfRain?: number;
  amountOfRain?: number;
  weatherCode?: string;
};

export type HourlyForecast = {
  time?: Date;
  apparentTemperature?: number;
  precipitation?: number;
  precipitationProbability?: number;
  windDirection?: number;
  windSpeed?: number;
  windGusts?: number;
  isDay?: boolean;
  cloudCover?: number;
};

export type DailyForecast = {
  time?: Date;
  apparentTemperatureMin?: number;
  apparentTemperatureMax?: number;
  daylightDuration?: number;
  sunrise?: Date;
  sunset?: Date;
  uvIndexClearSkyMax?: number;
  uvIndexMax?: number;
  weatherCode?: WmoWeatherCode;
  weatherCodeLabel?: string;
  precipitationSum?: number;
  precipitationProbabilityMax?: number;
  precipitationProbabilityMin?: number;
};

export enum WmoWeatherCode {
  /**
   * Clear
   */
  CLEAR = 'Clear',
  /**
   * MostlyClear
   */
  MOSTLY_CLEAR = 'MostlyClear',
  /**
   * PartlyClear
   */
  PARTLY_CLEAR = 'PartlyClear',
  /**
   * Overcast
   */
  OVERCAST = 'Overcast',
  /**
   * Fog
   */
  FOG = 'Fog',
  /**
   * RimeFog
   */
  RIME_FOG = 'RimeFog',
  /**
   * LightDrizzle
   */
  LIGHT_DRIZZLE = 'LightDrizzle',
  /**
   * MediumDrizzle
   */
  MEDIUM_DRIZZLE = 'MediumDrizzle',
  /**
   * HeavyDrizzle
   */
  HEAVY_DRIZZLE = 'HeavyDrizzle',
  /**
   * LightFreezingDrizzle
   */
  LIGHT_FREEZING_DRIZZLE = 'LightFreezingDrizzle',
  /**
   * HeavyFreezingDrizzle
   */
  HEAVY_FREEZING_DRIZZLE = 'HeavyFreezingDrizzle',
  /**
   * LightRain
   */
  LIGHT_RAIN = 'LightRain',
  /**
   * MediumRain
   */
  MEDIUM_RAIN = 'MediumRain',
  /**
   * HeavyRain
   */
  HEAVY_RAIN = 'HeavyRain',
  /**
   * LightFreezingRain
   */
  LIGHT_FREEZING_RAIN = 'LightFreezingRain',
  /**
   * HeavyFreezingRain
   */
  HEAVY_FREEZING_RAIN = 'HeavyFreezingRain',
  /**
   * LightSnow
   */
  LIGHT_SNOW = 'LightSnow',
  /**
   * MediumSnow
   */
  MEDIUM_SNOW = 'MediumSnow',
  /**
   * HeavySnow
   */
  HEAVY_SNOW = 'HeavySnow',
  /**
   * GrainySnow
   */
  GRAINY_SNOW = 'GrainySnow',
  /**
   * LightRainShower
   */
  LIGHT_RAIN_SHOWER = 'LightRainShower',
  /**
   * MediumRainShower
   */
  MEDIUM_RAIN_SHOWER = 'MediumRainShower',
  /**
   * HeavyRainShower
   */
  HEAVY_RAIN_SHOWER = 'HeavyRainShower',
  /**
   * LightSnowShower
   */
  LIGHT_SNOW_SHOWER = 'LightSnowShower',
  /**
   * HeavySnowShower
   */
  HEAVY_SNOW_SHOWER = 'HeavySnowShower',
  /**
   * Thunderstorm
   */
  THUNDERSTORM = 'Thunderstorm',
  /**
   * ThunderstormWithSomeRain
   */
  THUNDERSTORM_WITH_SOME_RAIN = 'ThunderstormWithSomeRain',
  /**
   * ThunderstormWithHeavyRain
   */
  THUNDERSTORM_WITH_HEAVY_RAIN = 'ThunderstormWithHeavyRain',
}

export type ConfigData = {
  body?: never;
  path?: never;
  query?: never;
  url: '/api/config';
};

export type ConfigResponses = {
  200: Config;
};

export type ConfigResponse = ConfigResponses[keyof ConfigResponses];

export type MediaGetData = {
  body?: never;
  path?: never;
  query: {
    count: number;
  };
  url: '/api/media/random';
};

export type MediaGetResponses = {
  200: MediaItem;
};

export type MediaGetResponse = MediaGetResponses[keyof MediaGetResponses];

export type MediaAllData = {
  body?: never;
  path?: never;
  query: {
    offset: number;
    length: number;
  };
  url: '/api/media/paginate';
};

export type MediaAllResponses = {
  200: Array<PaginatedMediaItem>;
};

export type MediaAllResponse = MediaAllResponses[keyof MediaAllResponses];

export type MediaPatchData = {
  body?: never;
  path: {
    mediaId: string;
  };
  query: {
    enabled: boolean;
  };
  url: '/api/media/item/{mediaId}/toggle';
};

export type MediaPatchErrors = {
  404: unknown;
};

export type MediaPatchResponses = {
  200: MediaItem;
};

export type MediaPatchResponse = MediaPatchResponses[keyof MediaPatchResponses];

export type MediaGet2Data = {
  body?: never;
  path: {
    mediaId: string;
    width: number;
    height: number;
  };
  query: {
    blur: boolean;
    format: MediaTransformOptionsFormat;
  };
  url: '/api/media/download/item/{mediaId}/{width}/{height}';
};

export type MediaGet2Errors = {
  400: unknown;
  404: unknown;
};

export type MediaGet3Data = {
  body?: never;
  path: {
    direction: number;
    size: number;
  };
  query?: never;
  url: '/api/media/download/line/{direction}/{size}';
};

export type MediaGet3Errors = {
  400: unknown;
  404: unknown;
};

export type MediaGet4Data = {
  body?: never;
  path: {
    mediaId: string;
    width: number;
    height: number;
  };
  query: {
    blur: boolean;
    format: MediaTransformOptionsFormat;
  };
  url: '/api/media/transform/item/{mediaId}/{width}/{height}';
};

export type MediaGet4Errors = {
  404: unknown;
};

export type MediaGet4Responses = {
  202: AcceptedTransformMediaItem;
};

export type MediaGet4Response = MediaGet4Responses[keyof MediaGet4Responses];

export type MediaGet5Data = {
  body?: never;
  path: {
    direction: number;
    size: number;
  };
  query?: never;
  url: '/api/media/transform/line/{direction}/{size}';
};

export type MediaGet5Errors = {
  404: unknown;
};

export type MediaGet5Responses = {
  202: AcceptedTransformMediaLine;
};

export type MediaGet5Response = MediaGet5Responses[keyof MediaGet5Responses];

export type WeatherData = {
  body?: never;
  path: {
    longitude: number;
    latitude: number;
  };
  query?: never;
  url: '/api/weather/{longitude}/{latitude}/current';
};

export type WeatherErrors = {
  404: unknown;
};

export type WeatherResponses = {
  200: WeatherForecast;
};

export type WeatherResponse = WeatherResponses[keyof WeatherResponses];

export type WeatherAllData = {
  body?: never;
  path: {
    longitude: number;
    latitude: number;
  };
  query?: never;
  url: '/api/weather/{longitude}/{latitude}/hourly';
};

export type WeatherAllErrors = {
  404: unknown;
};

export type WeatherAllResponses = {
  200: Array<HourlyForecast>;
};

export type WeatherAllResponse = WeatherAllResponses[keyof WeatherAllResponses];

export type WeatherAll2Data = {
  body?: never;
  path: {
    longitude: number;
    latitude: number;
  };
  query?: never;
  url: '/api/weather/{longitude}/{latitude}/daily';
};

export type WeatherAll2Errors = {
  404: unknown;
};

export type WeatherAll2Responses = {
  200: Array<DailyForecast>;
};

export type WeatherAll2Response =
  WeatherAll2Responses[keyof WeatherAll2Responses];

export type ClientOptions = {
  baseUrl: 'http://localhost:5298' | (string & {});
};
