import { inject } from 'vue';
import { type ApiClient, ApiError } from '@/domain/client/api.ts';
import type { components } from '@/domain/generated/schema';

export type WeatherForecast = components['schemas']['WeatherForecast'];
export type DailyForecast = components['schemas']['DailyForecast'];
export type HourlyForecast = components['schemas']['HourlyForecast'];

export interface IWeatherClient {
  current(
    latitude: number,
    longitude: number,
  ): Promise<WeatherForecast | undefined>;

  daily(
    latitude: number,
    longitude: number,
  ): Promise<DailyForecast[] | undefined>;

  hourly(
    latitude: number,
    longitude: number,
  ): Promise<HourlyForecast[] | undefined>;
}

export class WeatherClient implements IWeatherClient {
  public constructor(private readonly client: ApiClient) {}

  public async current(
    longitude: number,
    latitude: number,
  ): Promise<WeatherForecast | undefined> {
    const response = await this.client.GET(
      '/api/weather/{longitude}/{latitude}/current',
      { params: { path: { latitude, longitude } } },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }

  public async daily(
    longitude: number,
    latitude: number,
  ): Promise<DailyForecast[] | undefined> {
    const response = await this.client.GET(
      '/api/weather/{longitude}/{latitude}/daily',
      { params: { path: { latitude, longitude } } },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }

  public async hourly(
    longitude: number,
    latitude: number,
  ): Promise<HourlyForecast[] | undefined> {
    const response = await this.client.GET(
      '/api/weather/{longitude}/{latitude}/hourly',
      { params: { path: { latitude, longitude } } },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }
}

export function getWeatherClient(client: ApiClient): IWeatherClient {
  return new WeatherClient(client);
}

export const WeatherApiProvider = Symbol('WeatherApiProvider');

export function injectWeatherApi(): IWeatherClient {
  return inject<IWeatherClient>(WeatherApiProvider)!;
}
