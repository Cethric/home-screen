import {fileURLToPath, URL} from 'node:url';
import {defineConfig, UserConfig} from 'vite';
import vue from '@vitejs/plugin-vue';
import dts from 'vite-plugin-dts';

// https://vitejs.dev/config/
export default defineConfig(() => {
    return {
        plugins: [
            vue(),
            dts({
                tsconfigPath: fileURLToPath(
                    new URL('./tsconfig.app.json', import.meta.url),
                ),
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
                fileName: (fmt) => `web-common-components.${fmt}.js`,
            },
            rollupOptions: {
                external: ['vue', /@vueuse\/.*/, 'luxon', /@fortawesome\/.*/],
                output: {
                    // Global variables for use in the UMD build
                    manualChunks: {
                        leaflet: [
                            'leaflet',
                            '@vue-leaflet/vue-leaflet',
                            '@turf/turf',
                            'geojson',
                        ],
                        sentry: ['@sentry/vue'],
                    },
                    globals: {
                        vue: 'Vue',
                    },
                },
            },
        },
    } satisfies UserConfig;
});
