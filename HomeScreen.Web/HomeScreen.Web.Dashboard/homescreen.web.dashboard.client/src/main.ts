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
import { ConfigProvider, loadConfig } from '@/domain/client/config';
import { useRoutes } from '@/routes';

(async () => {
  const response = await loadConfig();
  const commonConfigApi = getConfigClient(response.commonUrl);
  const commonResponse = await commonConfigApi.config();

  const app = createApp(App)
    .provide(ConfigProvider, response)
    .provide(MediaApiProvider, getMediaClient(response.commonUrl))
    .provide(WeatherApiProvider, getWeatherClient(response.commonUrl))
    .provide(ConfigApiProvider, commonConfigApi)
    .use(useRoutes());
  configureSentry(app, commonResponse.result.sentryDsn);
  app.mount('#app');
})();
