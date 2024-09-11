import {
  injectWeatherApi,
  WeatherForecast,
} from '@homescreen/web-common-components';

export const loadWeather = async (): Promise<WeatherForecast> => {
  const weatherApi = injectWeatherApi();
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
