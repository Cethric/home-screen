import {
  ConfigClient,
  type IConfigClient,
} from '@/domain/generated/homescreen-common-api';
import { inject } from 'vue';

export function getConfigClient(baseUrl: string): IConfigClient {
  return new ConfigClient(baseUrl, window);
}

export const ConfigApiProvider = Symbol('ConfigApiProvider');

export function injectConfigApi(): IConfigClient {
  return inject<IConfigClient>(ConfigApiProvider)!;
}
