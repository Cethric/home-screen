import type { Plugin } from 'vue';
import { openobserveRum } from '@openobserve/browser-rum';
import { openobserveLogs } from '@openobserve/browser-logs';
import { v4 } from 'uuid';

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

export { openobserveRum, openobserveLogs };

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
      trackFrustrations: true,
      trackViewsManually: true,
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
      forwardReports: 'all',
      trackLongTasks: true,
      trackResources: true,
      insecureHTTP: insecureHTTP,
      apiVersion: 'v1',
    });

    const ID_KEY = 'rum-id';
    let key = localStorage.getItem(ID_KEY);
    if (key === null) {
      key = v4();
      localStorage.setItem(ID_KEY, key);
    }
    openobserveRum.setUser({
      id: key,
      name: location.hostname,
    });

    app.mixin({
      beforeCreate() {
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
