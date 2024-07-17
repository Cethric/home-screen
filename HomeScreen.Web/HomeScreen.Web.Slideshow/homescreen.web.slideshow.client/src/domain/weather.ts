import {
  WeatherForecast,
  WeatherForecastClient,
} from '@/domain/api/homescreen-slideshow-api';

const weatherApi = new WeatherForecastClient();

export const loadWeather = async (): Promise<WeatherForecast> => {
  try {
    const response = await weatherApi.getCurrentForecast(151.20732, -33.86785);
    return response.result;
  } catch (e) {
    console.log('Unable to load weather data', e);
    return new WeatherForecast({
      feelsLikeTemperature: 0,
      weatherCode: 'Unable to load weather data',
    });
  }
};
