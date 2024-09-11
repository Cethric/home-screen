import {
  type IWeatherClient,
  WeatherClient,
} from '@/domain/generated/homescreen-common-api';
import { inject } from 'vue';

export function getWeatherClient(baseUrl: string): IWeatherClient {
  return new WeatherClient(baseUrl, window);
}

export const WeatherApiProvider = Symbol('WeatherApiProvider');

export function injectWeatherApi(): IWeatherClient {
  return inject<IWeatherClient>(WeatherApiProvider)!;
}
