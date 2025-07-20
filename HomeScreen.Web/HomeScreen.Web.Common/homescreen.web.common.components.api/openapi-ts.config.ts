import {defineConfig} from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'http://localhost:5298/swagger/v1/swagger.json',
  output: {
    format: 'biome',
    lint: 'biome',
    clean: true,
    path: './src/generated',
  },
  plugins: [
    { name: '@hey-api/schemas', type: 'json', exportFromIndex: true },
    {
      name: '@hey-api/sdk',
      auth: false,
      client: '@hey-api/client-fetch',
      operationId: true,
      transformer: '@hey-api/transformers',
      validator: 'zod',
      asClass: false,
        exportFromIndex: true,
        instance: false,
    },
    {
      name: '@hey-api/typescript',
      enums: 'typescript',
      exportFromIndex: true,
    },
    {
      name: '@hey-api/transformers',
        dates: true,
      // dates: 'luxon',
      // dateFormat: 'iso',
      exportFromIndex: true,
    },
    { name: 'zod', exportFromIndex: true, dates: {offset:true}, metadata: true, comments: true },
    // {name: '@tanstack/vue-query', exportFromIndex: false}
    // { name: '@hey-api/client-fetch', throwOnError: true, bundle: false },
  ],
});
