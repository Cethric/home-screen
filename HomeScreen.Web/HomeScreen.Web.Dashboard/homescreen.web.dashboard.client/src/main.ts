import '@/styles/_root.scss';

import { createApp } from 'vue';
import App from './App.vue';
import {
  ConfigApiProvider,
  getCommonApi,
  getConfigClient,
  getMediaClient,
  getWeatherClient,
  MediaApiProvider,
  WeatherApiProvider,
} from '@homescreen/web-common-components';
import { ConfigProvider, loadConfig } from '@/domain/client/config';
import { useRoutes } from '@/routes';

(async () => {
  const response = await loadConfig();
  const commonApi = getCommonApi(response.commonUrl);
  const commonConfigApi = getConfigClient(commonApi);

  const app = createApp(App)
    .provide(ConfigProvider, response)
    .provide(MediaApiProvider, getMediaClient(commonApi))
    .provide(WeatherApiProvider, getWeatherClient(commonApi))
    .provide(ConfigApiProvider, commonConfigApi)
    .use(useRoutes());
  app.mount('#app');
})();
