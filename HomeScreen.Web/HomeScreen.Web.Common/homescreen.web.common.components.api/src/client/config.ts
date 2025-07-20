import { type Config, config } from '../generated';
import type { Client } from '../generated/client';

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
