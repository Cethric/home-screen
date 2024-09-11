import 'web-streams-polyfill/polyfill';

import '@/styles/_root.scss';

import { createApp } from 'vue';
import App from './App.vue';
import {
  ConfigApiProvider,
  configureSentry,
  getConfigClient,
  getMediaClient,
  getWeatherClient,
  MediaApiProvider,
  WeatherApiProvider,
} from '@homescreen/web-common-components';
import { loadConfig } from '@/domain/client/config';

(async () => {
  const response = await loadConfig();
  const commonConfigApi = getConfigClient(response.commonUrl);
  const commonResponse = await commonConfigApi.config();

  const app = createApp(App)
    .provide('config', response)
    .provide(MediaApiProvider, getMediaClient(response.commonUrl))
    .provide(WeatherApiProvider, getWeatherClient(response.commonUrl))
    .provide(ConfigApiProvider, commonConfigApi);
  configureSentry(app, commonResponse.result.sentryDsn);
  app.mount('#app');
})();
