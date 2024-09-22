import {
  ConfigClient,
  type IConfigClient,
} from '@/domain/generated/homescreen-common-api';
import { inject } from 'vue';

export function getConfigClient(baseUrl: string): IConfigClient {
  return new ConfigClient(baseUrl, window);
}

export const ConfigApiProvider = Symbol('CommonConfigApiProvider');

export function injectConfigApi(): IConfigClient {
  return inject<IConfigClient>(ConfigApiProvider)!;
}
