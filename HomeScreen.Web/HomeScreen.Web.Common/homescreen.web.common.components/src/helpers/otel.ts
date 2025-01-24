import type { Plugin } from 'vue';
import {
  ConsoleSpanExporter,
  SimpleSpanProcessor,
  WebTracerProvider,
} from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto/build/src/platform/browser';
import { Resource } from '@opentelemetry/resources';
import {
  ATTR_SERVICE_NAME,
  ATTR_SERVICE_VERSION,
  ATTR_USER_AGENT_ORIGINAL,
} from '@opentelemetry/semantic-conventions';

function parseDelimitedValues(value: string): Record<string, string> {
  const headers = value.split(','); // Split by comma
  const result: Record<string, string> = {};

  headers.forEach((header) => {
    const [key, value] = header.split('='); // Split by equal sign
    result[key.trim()] = value.trim(); // Add to the object, trimming spaces
  });

  return result;
}

export const otel = {
  install: (app, { serviceName, endpoint, headers, attributes }) => {
    const parsedAttributes = parseDelimitedValues(attributes);
    const parsedHeaders = parseDelimitedValues(headers);

    const provider = new WebTracerProvider({
      spanProcessors: [
        new SimpleSpanProcessor(new ConsoleSpanExporter()),
        new SimpleSpanProcessor(
          new OTLPTraceExporter({
            url: `${endpoint}/v1/traces`,
            headers: parsedHeaders,
          }),
        ),
        // new SimpleSpanProcessor(new OTLPMetricExporter({})),
      ],
      resource: new Resource({
        [ATTR_SERVICE_NAME]: serviceName,
        [ATTR_SERVICE_VERSION]: '0.1.0',
        [ATTR_USER_AGENT_ORIGINAL]: navigator.userAgent,
        ...parsedAttributes,
      }),
    });
    provider.register({ contextManager: new ZoneContextManager() });
    registerInstrumentations({
      instrumentations: [
        new DocumentLoadInstrumentation(),
        new UserInteractionInstrumentation(),
        new FetchInstrumentation(),
      ],
    });
    app.provide('provider', provider);
  },
} satisfies Plugin<{
  serviceName: string;
  endpoint: string;
  headers: string;
  attributes: string;
}>;
