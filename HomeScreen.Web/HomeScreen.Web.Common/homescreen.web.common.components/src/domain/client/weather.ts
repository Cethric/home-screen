import {
  type IWeatherClient,
  WeatherClient,
} from '@/domain/generated/homescreen-common-api';

export function getWeatherClient(baseUrl: string): IWeatherClient {
  return new WeatherClient(baseUrl);
}
