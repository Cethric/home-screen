import { fileURLToPath, URL } from "node:url";
import tailwindcss from "@tailwindcss/vite";
import vue from "@vitejs/plugin-vue";
import { defineConfig, type UserConfig } from "vite";
import dts from "vite-plugin-dts";

// https://vitejs.dev/config/
export default defineConfig(() => {
    return {
        plugins: [
            vue(),
            tailwindcss(),
            dts({
                tsconfigPath: fileURLToPath(
                    new URL("./tsconfig.app.json", import.meta.url),
                ),
                exclude: ["**/*.stories.*", "**/stories", "**/styles"],
                outDir: fileURLToPath(new URL("./dist", import.meta.url)),
                entryRoot: fileURLToPath(new URL("./src", import.meta.url)),
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
                "@": fileURLToPath(new URL("./src", import.meta.url)),
            },
        },
        build: {
            target: "esnext",
            sourcemap: true,
            lib: {
                entry: fileURLToPath(
                    new URL("./src/index.ts", import.meta.url),
                ),
                formats: ["es"],
                name: "web-common-components",
                cssFileName: "web-common-components",
                fileName: "index.js",
            },
            rollupOptions: {
                external: [
                    "vue",
                    /@vueuse\/.*/,
                    "luxon",
                    /@fortawesome\/.*/,
                    "zod",
                ],
                output: {
                    // Global variables for use in the UMD build
                    manualChunks: {
                        leaflet: [
                            "leaflet",
                            "@vue-leaflet/vue-leaflet",
                            "@turf/turf",
                            "geojson",
                        ],
                    },
                    globals: {
                        vue: "Vue",
                    },
                },
            },
        },
    } satisfies UserConfig;
});
