import {
  type IWeatherClient,
  WeatherForecast,
} from '@homescreen/web-common-components';
import { inject } from 'vue';

export const loadWeather = async (): Promise<WeatherForecast> => {
  const weatherApi = inject<IWeatherClient>('weatherApi')!;
  console.log(weatherApi);
  try {
    const response = await weatherApi.current(151.20732, -33.86785);
    return response.result;
  } catch (e) {
    console.log('Unable to load weather data', e);
    return new WeatherForecast({
      feelsLikeTemperature: 0,
      weatherCode: 'Unable to load weather data',
    });
  }
};
