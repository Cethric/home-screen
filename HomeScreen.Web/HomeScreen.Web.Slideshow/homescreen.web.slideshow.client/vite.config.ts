import {fileURLToPath, URL} from 'node:url';
import {defineConfig, UserConfig} from 'vite';
import vue from '@vitejs/plugin-vue';
import fs from 'node:fs';
import {env} from 'node:process';
import path from 'node:path';
import child_process from 'node:child_process';

const makeServerConfig = (): UserConfig['server'] => {
    let keyFilePath: string | undefined = undefined;
    let certFilePath: string | undefined = undefined;
    const baseFolder =
        env.APPDATA !== undefined && env.APPDATA !== ''
            ? `${env.APPDATA}/ASP.NET/https`
            : `${env.HOME}/.aspnet/https`;

    const certificateName = 'homescreen.web.slideshow.client';
    certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (
            0 !==
            child_process.spawnSync(
                'dotnet',
                [
                    'dev-certs',
                    'https',
                    '--export-path',
                    certFilePath,
                    '--format',
                    'Pem',
                    '--no-password',
                ],
                {stdio: 'inherit'},
            ).status
        ) {
            throw new Error('Could not create certificate.');
        }
    }

    let target = env.ASPNETCORE_HTTPS_PORT
        ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
        : 'https://localhost:7163';
    if (env.ASPNETCORE_URLS) {
        target = env.ASPNETCORE_HTTPS_PORT
            ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
            : env.ASPNETCORE_URLS.split(';')[0];
    }

    return {
        proxy: {
            '/api': {
                target: target,
                changeOrigin: true,
                // rewrite: (path) => path.replace(/^\/api/, ''),
                secure: false,
            },
        },
        port: parseInt(process.env.PORT ?? '5173'),
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        },
        host: true,
    };
};

// https://vitejs.dev/config/
export default defineConfig(({command}) => {
    return {
        plugins: [vue()],
        build: {
            rollupOptions: {
                output: {
                    manualChunks: {
                        fontawesome: [
                            '@fortawesome/free-brands-svg-icons',
                            '@fortawesome/free-regular-svg-icons',
                            '@fortawesome/free-solid-svg-icons',
                            '@fortawesome/vue-fontawesome',
                        ],
                        'universal-cookie': ['universal-cookie'],
                        'jwt-decode': ['jwt-decode'],
                        qrcode: ['qrcode'],
                        sortablejs: ['sortablejs'],
                        nprogress: ['nprogress'],
                        vue: ['vue'],
                        luxon: ['luxon'],
                        components: ['@homescreen/web-common-components'],
                    },
                },
            },
        },
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url)),
            },
        },
        server: command === 'serve' ? makeServerConfig() : {},
    } satisfies UserConfig;
});
