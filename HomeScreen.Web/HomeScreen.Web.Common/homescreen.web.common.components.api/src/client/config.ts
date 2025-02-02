import type { Client } from '@hey-api/client-fetch';
import { config, type Config } from '../generated';

export interface IConfigClient {
  config(): Promise<Config | undefined>;
}

export class ConfigClient implements IConfigClient {
  constructor(private readonly client: Client) {}

  public async config(): Promise<Config | undefined> {
    const response = await config({ client: this.client });
    return response.data;
  }
}
