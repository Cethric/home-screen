import {
	type DailyForecast,
	type HourlyForecast,
	type WeatherForecast,
	weather,
	weatherAll,
	weatherAll2,
} from "../generated";
import type { Client } from "../generated/client";

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
	public constructor(private readonly client: Client) {}

	public async current(
		longitude: number,
		latitude: number,
	): Promise<WeatherForecast | undefined> {
		const response = await weather({
			client: this.client,
			path: { latitude, longitude },
		});
		return response.data as WeatherForecast;
	}

	public async daily(
		longitude: number,
		latitude: number,
	): Promise<DailyForecast[] | undefined> {
		const response = await weatherAll2({
			client: this.client,
			path: { latitude, longitude },
		});
		return response.data;
	}

	public async hourly(
		longitude: number,
		latitude: number,
	): Promise<HourlyForecast[] | undefined> {
		const response = await weatherAll({
			client: this.client,
			path: { latitude, longitude },
		});
		return response.data;
	}
}
