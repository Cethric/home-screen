import 'web-streams-polyfill/polyfill';

import '@/styles/_root.scss';

import { createApp } from 'vue';
import App from './App.vue';
import {
  getConfigClient,
  getMediaClient,
  getWeatherClient,
} from '@homescreen/web-common-components';
import { loadConfig } from '@/domain/client/config';

(async () => {
  const response = await loadConfig();

  createApp(App)
    .provide('config', response)
    .provide('mediaApi', getMediaClient(response.commonUrl!))
    .provide('weatherApi', getWeatherClient(response.commonUrl!))
    .provide('configApi', getConfigClient(response.commonUrl!))
    .mount('#app');
})();
