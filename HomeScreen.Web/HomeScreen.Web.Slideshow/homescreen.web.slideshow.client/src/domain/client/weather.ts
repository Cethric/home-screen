import {
	injectComponentWeatherClient,
	type WeatherForecast,
} from "@homescreen/web-common-components";

export const loadWeather = async (): Promise<WeatherForecast | undefined> => {
	const weatherApi = injectComponentWeatherClient();
	try {
		return await weatherApi.current(151.20732, -33.86785);
	} catch (e) {
		console.error("Unable to load weather data", e);
		return {
			feelsLikeTemperature: 0,
			weatherCode: "Unable to load weather data",
		} satisfies WeatherForecast;
	}
};
