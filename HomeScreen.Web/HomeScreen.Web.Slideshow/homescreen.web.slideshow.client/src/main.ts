import 'web-streams-polyfill/polyfill';

import '@/styles/tailwind.css';
import '@/styles/nprogress.css';
import '@homescreen/web-common-components/dist/web-common-components.css';

import { createApp } from 'vue';
import App from './App.vue';
import {
    ComponentApiPlugin,
    makeClient,
    openobserver,
    otel,
} from '@homescreen/web-common-components';
import { ConfigProvider, loadConfig } from '@/domain/client/config';

(async () => {
    const response = await loadConfig();
    const {
        client,
        config,
        response: commonConfig,
    } = await makeClient(response.commonUrl);

    const app = createApp(App)
        .use(ComponentApiPlugin, { client, config })
        .provide(ConfigProvider, response);

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
