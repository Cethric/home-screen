import { inject } from 'vue';
import type { components } from '@/domain/generated/schema';
import { type ApiClient, ApiError } from '@/domain/client/api.ts';

export type Config = components['schemas']['Config'];

export interface IConfigClient {
  config(): Promise<Config | undefined>;
}

export class ConfigClient implements IConfigClient {
  constructor(private readonly client: ApiClient) {}

  public async config(): Promise<Config | undefined> {
    const response = await this.client.GET('/api/config', {});
    if (response.error) {
      // @ts-expect-error
      throw new ApiError(response.error);
    }
    return response.data;
  }
}

export function injectConfigApi(): IConfigClient {
  return inject<IConfigClient>(ConfigApiProvider)!;
}

export function getConfigClient(client: ApiClient): IConfigClient {
  return new ConfigClient(client);
}

export const ConfigApiProvider = Symbol('CommonConfigApiProvider');
