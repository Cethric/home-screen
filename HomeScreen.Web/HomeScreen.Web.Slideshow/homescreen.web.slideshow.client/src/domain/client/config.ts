import { ConfigClient } from '@/domain/generated/homescreen-slideshow-api';

export async function loadConfig() {
  const config = new ConfigClient(undefined, window);
  const response = await config.config();
  if (response.status !== 200) {
    throw new Error('Unable to load config');
  }
  return response.result;
}
