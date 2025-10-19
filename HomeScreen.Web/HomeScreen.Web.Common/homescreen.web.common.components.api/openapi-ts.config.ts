import { defineConfig, definePluginConfig, tsc } from '@hey-api/openapi-ts';

const dateFormatter = definePluginConfig({
  config: {},
  api: {
    getSelector(...args: ReadonlyArray<string | undefined>) {
      return ['luxon', ...args];
    },
  },
  // dependencies: ['@hey-api/transformers'],
  handler: ({ plugin }) => {
    const luxonImportSymbol = plugin.registerSymbol({
      external: 'luxon',
      meta: {
        importKind: 'named',
        // kind: 'type',
      },
      name: 'DateTime',
      selector: plugin.api.getSelector('DateTime'),
    });

    const statement = tsc.constVariable({
      exportConst: true,
      expression: tsc.arrowFunction({
        async: false,
        parameters: [{ isRequired: true, type: 'string', name: 'dateString' }],
        returnType: luxonImportSymbol.placeholder,
        statements: [
          tsc.returnStatement({
            expression: tsc.callExpression({
              functionName: tsc.propertyAccessExpression({
                name: 'fromISO',
                isOptional: false,
                expression: tsc.identifier({ text: 'DateTime' }),
              }),
              parameters: [tsc.identifier({ text: 'dateString' })],
            }),
          }),
        ],
      }),
      name: 'stringToDate',
    });

    const symbolQueryOptionsFn = plugin.registerSymbol({
      exported: true,
      name: 'stringToDate',
      selector: plugin.api.getSelector('stringToDate'),
    });
    plugin.setSymbolValue(symbolQueryOptionsFn, statement);

    console.assert(luxonImportSymbol.placeholder === '_heyapi_168_');
    console.assert(symbolQueryOptionsFn.placeholder === '_heyapi_169_');
  },
  name: 'date-formatter',
  tags: ['transformer'],
});

export default defineConfig({
  input: 'http://localhost:5298/swagger/v1/swagger.json',
  output: {
    format: 'biome',
    lint: 'biome',
    clean: true,
    path: './src/generated',
  },
  parser: {
    validate_EXPERIMENTAL: true,
    pagination: {
      keywords: ['after', 'before', 'cursor', 'offset', 'page', 'start', 'length'],
    },
  },
  plugins: [
    { name: '@hey-api/schemas', exportFromIndex: true, type: 'json' },
    {
      name: '@hey-api/sdk',
      asClass: false,
      auth: true,
      client: '@hey-api/client-fetch',
      operationId: true,
      exportFromIndex: true,
      responseStyle: 'fields',
      transformer: '@hey-api/transformers',
      validator: {
        request: 'zod',
        response: 'zod',
      },
      instance: false,
    },
    dateFormatter({ exportFromIndex: true }),
    {
      name: '@hey-api/typescript',
      enums: { enabled: true, mode: 'typescript' },
      exportFromIndex: true,
    },
    {
      name: '@hey-api/transformers',
      bigInt: true,
      dates: false,
      exportFromIndex: true,
      transformers: [
        ({ dataExpression, schema }) => {
          if (schema.type !== 'string' || !(schema.format === 'date' || schema.format === 'date-time')) {
            return;
          }

          if (typeof dataExpression === 'string') {
            return [
              tsc.callExpression({
                functionName: '_heyapi_169_',
                parameters: [tsc.identifier({ text: dataExpression })],
                // types: [identifierDate],
              }),
            ];
          }

          if (dataExpression) {
            return [
              tsc.assignment({
                left: dataExpression,
                right: tsc.callExpression({
                  functionName: '_heyapi_169_',
                  parameters: [dataExpression],
                }),
              }),
            ];
          }
        },
      ],
      typeTransformers: [
        ({ schema }) => {
          if (schema.type !== 'string' || !(schema.format === 'date' || schema.format === 'date-time')) {
            return;
          }
          return tsc.typeReferenceNode({ typeName: '_heyapi_168_' });
        },
      ],
    },
    {
      name: 'zod',
      comments: true,
      compatibilityVersion: 4,
      dates: { offset: true, local: false },
      definitions: { enabled: true, types: { infer: { enabled: true } } },
      exportFromIndex: true,
      metadata: true,
      requests: { enabled: true, types: { infer: { enabled: true } } },
      responses: { enabled: true, types: { infer: { enabled: true } } },
      types: { infer: { enabled: true } },
      webhooks: { enabled: true, types: { infer: { enabled: true } } },
    },
    { name: '@hey-api/client-fetch', baseUrl: false, bundle: true, exportFromIndex: true, throwOnError: true },
    {
      name: '@pinia/colada',
      comments: true,
      exportFromIndex: true,
      mutationOptions: { enabled: true },
      queryKeys: { enabled: true, tags: true },
      queryOptions: { enabled: true },
    },
  ],
});
