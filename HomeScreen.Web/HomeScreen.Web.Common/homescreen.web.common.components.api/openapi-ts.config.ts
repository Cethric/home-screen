import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    input: 'http://localhost:5298/swagger/v1/swagger.json',
    output: {
        format: 'prettier',
        lint: 'eslint',
        clean: true,
        path: './src/generated',
    },
    plugins: [
        { name: '@hey-api/schemas', type: 'json' },
        {
            name: '@hey-api/sdk',
            auth: false,
            client: '@hey-api/client-fetch',
            operationId: true,
            transformer: '@hey-api/transformers',
            validator: 'zod',
            asClass: false,
        },
        {
            name: '@hey-api/typescript',
            enums: 'typescript',
            exportInlineEnums: true,
            enumsCase: 'PascalCase',
        },
        {
            name: '@hey-api/transformers',
            dates: 'luxon',
            dateFormat: 'iso',
        },
        { name: 'zod' },
        // { name: '@hey-api/client-fetch', throwOnError: true, bundle: false },
    ],
});
