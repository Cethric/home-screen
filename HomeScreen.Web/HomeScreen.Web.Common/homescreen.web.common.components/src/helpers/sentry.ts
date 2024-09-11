import {
    breadcrumbsIntegration,
    browserApiErrorsIntegration,
    browserProfilingIntegration,
    browserTracingIntegration,
    captureConsoleIntegration,
    contextLinesIntegration,
    feedbackAsyncIntegration,
    httpClientIntegration,
    init,
    moduleMetadataIntegration,
    replayIntegration,
    reportingObserverIntegration,
    sessionTimingIntegration,
} from '@sentry/vue';
import type {App} from 'vue';

export function configureSentry(app: App<Element>, dsn: string) {
    init({
        app: app,
        dsn: dsn,
        integrations: [
            replayIntegration(),
            captureConsoleIntegration(),
            browserTracingIntegration(),
            browserProfilingIntegration(),
            feedbackAsyncIntegration({colorScheme: 'system'}),
            breadcrumbsIntegration(),
            browserApiErrorsIntegration(),
            contextLinesIntegration(),
            httpClientIntegration(),
            moduleMetadataIntegration(),
            reportingObserverIntegration(),
            sessionTimingIntegration(),
        ],
        replaysOnErrorSampleRate: 0.1,
        replaysSessionSampleRate: 1.0,
        tracePropagationTargets: [
            'localhost',
            /^https:\/\/homescreen\.home-automation\.cloud/,
        ],
        trackComponents: true,
    });
}
