import "@/styles/nprogress.css";
import "@/styles/tailwind.css";
import "@homescreen/web-common-components/dist/web-common-components.css";

import {
    ComponentApiPlugin,
    makeClient,
} from "@homescreen/web-common-components";
import { createApp } from "vue";
import { ConfigProvider, loadConfig } from "@/domain/client/config";
import { useRoutes } from "@/routes";
import App from "./App.vue";

(async () => {
    const response = await loadConfig();
    const {
        client,
        config,
        response: commonConfig,
    } = await makeClient(response.commonUrl);

    console.log("Config", commonConfig);

    const app = createApp(App)
        .use(ComponentApiPlugin, { client, config })
        .provide(ConfigProvider, response)
        .use(useRoutes());

    app.mount("#app");
})().catch(console.error);
