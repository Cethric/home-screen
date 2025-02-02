import type { Plugin } from 'vue';
import { openobserveRum } from '@openobserve/browser-rum';
import { openobserveLogs } from '@openobserve/browser-logs';
import { v4, v6 } from 'uuid';

interface OpenObserverSettings {
  applicationId: string;
  clientToken: string;
  organizationIdentifier: string;
  service: string;
  env: string;
  version: string;
  endpoint: string;
  insecureHTTP: boolean;
}

export const openobserver = {
  install(
    app,
    {
      applicationId,
      clientToken,
      organizationIdentifier,
      service,
      env,
      version,
      endpoint,
      insecureHTTP,
    },
  ) {
    openobserveRum.init({
      applicationId: applicationId, // required, any string identifying your application
      clientToken: clientToken,
      site: endpoint,
      organizationIdentifier: organizationIdentifier,
      service: service,
      env: env,
      version: version,
      trackResources: true,
      trackLongTasks: true,
      trackUserInteractions: true,
      apiVersion: 'v1',
      insecureHTTP: insecureHTTP,
      defaultPrivacyLevel: 'allow', // 'allow' or 'mask-user-input' or 'mask'. Use one of the 3 values.
    });

    openobserveLogs.init({
      clientToken: clientToken,
      site: endpoint,
      organizationIdentifier: organizationIdentifier,
      service: service,
      env: env,
      version: version,
      forwardErrorsToLogs: true,
      forwardConsoleLogs: 'all',
      insecureHTTP: insecureHTTP,
      apiVersion: 'v1',
    });

    // You can set a user context
    // openobserveRum.setUser({
    //   id: '1',
    //   name: 'Captain Hook',
    //   email: 'captainhook@example.com',
    // });

    app.mixin({
      mounted() {
        const ID_KEY = 'rum-id';
        let key = localStorage.getItem(ID_KEY);
        if (key === null) {
          key = v4();
          localStorage.setItem(ID_KEY, key);
        }
        openobserveRum.setUser({
          id: key,
        });
        openobserveRum.startSessionReplayRecording();
      },
      errorCaptured(error, instance, info) {
        openobserveRum.addError(error, { info });
        return false;
      },
      beforeUnmount() {
        openobserveRum.stopSessionReplayRecording();
      },
    });
  },
} satisfies Plugin<OpenObserverSettings>;
