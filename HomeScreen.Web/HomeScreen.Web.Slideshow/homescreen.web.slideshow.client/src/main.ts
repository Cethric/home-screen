import 'web-streams-polyfill/polyfill';

import '@/styles/_root.scss';

import { createApp } from 'vue';
import App from './App.vue';
import { ConfigClient } from '@/domain/api/homescreen-slideshow-api';
import {
  getConfigClient,
  getMediaClient,
  getWeatherClient,
} from '@homescreen/web-common-components';

(async () => {
  const config = new ConfigClient(undefined, window);
  const response = await config.config();

  createApp(App)
    .provide('config', response.result)
    .provide('mediaApi', getMediaClient(response.result.commonUrl!))
    .provide('weatherApi', getWeatherClient(response.result.commonUrl!))
    .provide('configApi', getConfigClient(response.result.commonUrl!))
    .mount('#app');
})();
