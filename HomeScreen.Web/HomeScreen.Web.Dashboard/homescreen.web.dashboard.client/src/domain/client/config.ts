import { inject } from 'vue';
import {
  type Config,
  ConfigClient,
} from '@/domain/generated/homescreen-dashboard-api';

export const ConfigProvider: symbol = Symbol('Config');

export async function loadConfig() {
  const config = new ConfigClient(undefined, window);
  const response = await config.config();
  if (response.status !== 200) {
    throw new Error('Unable to load config');
  }
  return response.result;
}

export function injectConfig() {
  return inject<Config>(ConfigProvider);
}
