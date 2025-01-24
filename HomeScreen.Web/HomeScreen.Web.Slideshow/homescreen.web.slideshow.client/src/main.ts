import 'web-streams-polyfill/polyfill';

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
  otel,
  WeatherApiProvider,
} from '@homescreen/web-common-components';
import { ConfigProvider, loadConfig } from '@/domain/client/config';

(async () => {
  const response = await loadConfig();
  const commonApi = getCommonApi(response.commonUrl);
  const commonConfigApi = getConfigClient(commonApi);
  const config = await commonConfigApi.config();

  const app = createApp(App)
    .provide(ConfigProvider, response)
    .provide(MediaApiProvider, getMediaClient(commonApi))
    .provide(WeatherApiProvider, getWeatherClient(commonApi))
    .provide(ConfigApiProvider, commonConfigApi);

  if (config?.otlpConfig?.endpoint) {
    app.use(otel, {
      serviceName: 'slideshow-web',
      endpoint: config!.otlpConfig.endpoint,
      headers: config!.otlpConfig.headers,
      attributes: config!.otlpConfig.attributes,
    });
  }
  app.mount('#app');
})();
