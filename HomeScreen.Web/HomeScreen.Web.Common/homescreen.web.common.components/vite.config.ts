import {fileURLToPath, URL} from 'node:url';
import {defineConfig, UserConfig} from 'vite';
import vue from '@vitejs/plugin-vue';
import dts from 'vite-plugin-dts';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig(() => {
    return {
        plugins: [
            vue(),
            tailwindcss(),
            dts({
                tsconfigPath: fileURLToPath(
                    new URL('./tsconfig.app.json', import.meta.url),
                ),
                exclude: ['**/*.stories.*', '**/stories', '**/styles'],
                outDir: fileURLToPath(new URL('./dist', import.meta.url)),
                entryRoot: fileURLToPath(new URL('./src', import.meta.url)),
                declarationOnly: false,
                strictOutput: true,
                cleanVueFileName: true,
                staticImport: true,
                clearPureImport: true,
                insertTypesEntry: true,
            }),
        ],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url)),
            },
        },
        build: {
            target: 'modules',
            sourcemap: true,
            lib: {
                entry: fileURLToPath(new URL('./src/index.ts', import.meta.url)),
                formats: ['es'],
                name: 'web-common-components',
                cssFileName: 'web-common-components',
                fileName: (fmt, entryName) => `index.js`,
            },
            rollupOptions: {
                external: ['vue', /@vueuse\/.*/, 'luxon', /@fortawesome\/.*/, 'zod'],
                output: {
                    // Global variables for use in the UMD build
                    manualChunks: {
                        leaflet: [
                            'leaflet',
                            '@vue-leaflet/vue-leaflet',
                            '@turf/turf',
                            'geojson',
                        ],
                        otel: [
                            '@opentelemetry/api',
                            '@opentelemetry/sdk-trace-web',
                            '@opentelemetry/instrumentation',
                            '@opentelemetry/instrumentation-document-load',
                            '@opentelemetry/instrumentation-user-interaction',
                            '@opentelemetry/instrumentation-fetch',
                            '@opentelemetry/context-zone',
                            '@opentelemetry/exporter-trace-otlp-proto',
                            '@opentelemetry/exporter-metrics-otlp-http',
                            '@opentelemetry/instrumentation-xml-http-request',
                            '@opentelemetry/sdk-metrics',
                            '@opentelemetry/resources',
                            '@opentelemetry/semantic-conventions',
                            '@opentelemetry/otlp-exporter-base',
                        ],
                        openobserve: [
                            '@openobserve/browser-rum',
                            '@openobserve/browser-logs',
                        ],
                    },
                    globals: {
                        vue: 'Vue',
                    },
                },
            },
        },
    } satisfies UserConfig;
});
