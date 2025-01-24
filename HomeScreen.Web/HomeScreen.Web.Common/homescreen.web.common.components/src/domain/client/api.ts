import createClient, { type Client } from 'openapi-fetch';
import type { paths } from '@/domain/generated/schema';
import { inject } from 'vue';
import {
  type ErrorResponse,
  type MediaType,
  type ResponseObjectMap,
} from 'openapi-typescript-helpers';

export const ApiProvider = Symbol('CommonApiProvider');

export type ApiClient = Client<paths>;

export function injectCommonApi(): ApiClient {
  return inject<ApiClient>(ApiProvider)!;
}

export function getCommonApi(baseUrl: string): ApiClient {
  return createClient<paths>({ baseUrl });
}

export class ApiError<
  T extends { responses: any },
  Media extends MediaType,
> extends Error {
  constructor(error: ErrorResponse<ResponseObjectMap<T>, Media>) {
    super();
  }
}
