import '@/styles/_root.scss';

import { createApp } from 'vue';
import App from './App.vue';
import {
    ComponentApiPlugin,
    makeClient,
    openobserver,
    otel,
} from '@homescreen/web-common-components';
import { ConfigProvider, loadConfig } from '@/domain/client/config';
import { useRoutes } from '@/routes';

(async () => {
    const response = await loadConfig();
    const {
        client,
        config,
        response: commonConfig,
    } = await makeClient(response.commonUrl);

    const app = createApp(App)
        .use(ComponentApiPlugin, { client, config })
        .provide(ConfigProvider, response)
        .use(useRoutes());
    if (commonConfig?.otlpConfig) {
        app.use(otel, {
            serviceName: 'slideshow-web',
            endpoint: `${location.protocol}//${location.hostname}:${location.port}/otel`,
            headers: commonConfig.otlpConfig.headers,
            attributes: commonConfig.otlpConfig.attributes,
        });
    }

    if (commonConfig?.rumConfig) {
        const credentials = await fetch(
            `http://${commonConfig.rumConfig.endpoint}/api/${commonConfig.rumConfig.organizationIdentifier}/rumtoken`,
            {
                headers: {
                    Authorization: `Basic ${commonConfig.rumConfig.clientToken}`,
                },
            },
        ).then((res) => res.json());
        app.use(openobserver, {
            applicationId: 'homescreen.web.slideshow.client',
            clientToken: credentials['data']['rum_token'],
            organizationIdentifier:
                commonConfig.rumConfig.organizationIdentifier,
            service: 'homescreen.web.slideshow.client',
            env: 'development',
            version: '0.0.0',
            endpoint: commonConfig.rumConfig.endpoint,
            insecureHTTP: commonConfig.rumConfig.insecureHttp,
        });
    }

    app.mount('#app');
})();
