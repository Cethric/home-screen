import { inject, Plugin } from 'vue';
import { Client, createClient, createConfig } from '@hey-api/client-fetch';
import {
  ConfigClient,
  IConfigClient,
  IMediaClient,
  IWeatherClient,
  MediaClient,
  WeatherClient,
} from '../client';

export const ComponentClient: symbol = Symbol.for('ComponentClient');
export const CommonConfigClient: symbol = Symbol.for('CommonConfigClient');
export const CommonMediaClient: symbol = Symbol.for('CommonMediaClient');
export const CommonWeatherClient: symbol = Symbol.for('CommonWeatherClient');

export const injectComponentClient = () => inject(ComponentClient) as Client;
export const injectComponentConfigClient = () =>
  inject(CommonConfigClient) as IConfigClient;
export const injectComponentMediaClient = () =>
  inject(CommonMediaClient) as IMediaClient;
export const injectComponentWeatherClient = () =>
  inject(CommonWeatherClient) as IWeatherClient;

export const makeClient = async (baseUrl: string) => {
  const client = createClient(
    createConfig({ throwOnError: true, baseUrl: baseUrl }),
  );
  const config = new ConfigClient(client);
  const response = await config.config();

  return { client, config, response };
};

interface PluginProps {
  client: Client;
  config: ConfigClient;
}

export const ComponentApiPlugin = {
  install(app, { client, config }) {
    app.provide(ComponentClient, client);
    app.provide(CommonConfigClient, config);
    app.provide(CommonMediaClient, new MediaClient(client));
    app.provide(CommonWeatherClient, new WeatherClient(client));
  },
} satisfies Plugin<PluginProps>;
