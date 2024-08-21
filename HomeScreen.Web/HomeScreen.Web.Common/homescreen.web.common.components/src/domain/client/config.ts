import {
  ConfigClient,
  type IConfigClient,
} from '@/domain/generated/homescreen-common-api';

export function getConfigClient(baseUrl: string): IConfigClient {
  return new ConfigClient(baseUrl);
}
