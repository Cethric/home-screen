import {
  Config,
  ConfigClient,
} from '@/domain/generated/homescreen-slideshow-api';
import { inject } from 'vue';

export const ConfigProvider: symbol = Symbol.for('SlideshowConfig');

export async function loadConfig() {
  const config = new ConfigClient(undefined, window);
  const response = await config.config();
  if (response.status !== 200) {
    throw new Error('Unable to load config');
  }
  return response.result;
}

export function injectConfig() {
  const config = inject<Config>(ConfigProvider);
  return config!;
}
