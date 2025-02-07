import type { Plugin } from 'vue';
import {
    BatchSpanProcessor,
    ConsoleSpanExporter,
    SimpleSpanProcessor,
    WebTracerProvider,
} from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { Resource } from '@opentelemetry/resources';
import {
    ATTR_SERVICE_NAME,
    ATTR_SERVICE_VERSION,
    ATTR_USER_AGENT_ORIGINAL,
} from '@opentelemetry/semantic-conventions';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto/build/src/platform/browser';
import { XMLHttpRequestInstrumentation } from '@opentelemetry/instrumentation-xml-http-request';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-http';
import {
    MeterProvider,
    PeriodicExportingMetricReader,
} from '@opentelemetry/sdk-metrics';

function parseDelimitedValues(value: string): Record<string, string> {
    const headers = value.split(','); // Split by comma
    const result: Record<string, string> = {};

    headers.forEach((header) => {
        const [key, value] = header.split('='); // Split by equal sign
        result[key.trim()] = value.trim(); // Add to the object, trimming spaces
    });

    return result;
}

interface OtelSettings {
    serviceName: string;
    endpoint: string;
    headers: string;
    attributes: string;
}

export const otel = {
    install: (app, { serviceName, endpoint, headers, attributes }) => {
        const parsedAttributes = parseDelimitedValues(attributes);
        const parsedHeaders = parseDelimitedValues(headers);

        const provider = new WebTracerProvider({
            spanProcessors: [
                new BatchSpanProcessor(
                    new OTLPTraceExporter({
                        url: `${endpoint}/v1/traces`,
                        headers: parsedHeaders,
                    }),
                ),
            ],
            resource: new Resource({
                [ATTR_SERVICE_NAME]: serviceName,
                [ATTR_SERVICE_VERSION]: '0.1.0',
                [ATTR_USER_AGENT_ORIGINAL]: navigator.userAgent,
                ...parsedAttributes,
            }),
        });

        const meterProvider = new MeterProvider({
            readers: [
                new PeriodicExportingMetricReader({
                    exporter: new OTLPMetricExporter({
                        url: `${endpoint}/v1/traces`,
                        headers: parsedHeaders,
                    }),
                    exportIntervalMillis: 1000,
                }),
            ],
        });

        provider.register({ contextManager: new ZoneContextManager() });
        registerInstrumentations({
            instrumentations: [
                new DocumentLoadInstrumentation(),
                new XMLHttpRequestInstrumentation(),
                new UserInteractionInstrumentation(),
                new FetchInstrumentation(),
            ],
        });
        app.provide('provider', provider);
        app.provide('meterProvider', meterProvider);
    },
} satisfies Plugin<OtelSettings>;
