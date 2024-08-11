import {
  ApiException,
  type IMediaItem,
  MediaClient,
  MediaItem,
  SwaggerResponse,
} from '@/domain/api/homescreen-slideshow-api';
import { isAsyncIterable } from '@/helpers/isAsyncIterable';

function throwException(
  message: string,
  status: number,
  response: string,
  headers: {
    [key: string]: any;
  },
  result?: any,
): any {
  throw new ApiException(message, status, response, headers, result);
}

export class StreamingMediaApi extends MediaClient {
  public constructor() {
    super();
  }

  protected get url(): string {
    // @ts-expect-error
    return this.baseUrl;
  }

  protected get client(): {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
  } {
    // @ts-expect-error
    return this.http;
  }

  public async *randomStream(
    count: number,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<IMediaItem>> {
    let url = this.url + '/api/media/random?';
    if (count === undefined || count === null) {
      throw new Error(
        "The parameter 'count' must be defined and cannot be null.",
      );
    } else {
      url += 'count=' + encodeURIComponent('' + count) + '&';
    }
    url = url.replace(/[?&]$/, '');

    const options: RequestInit = {
      method: 'GET',
      signal,
      headers: {
        Accept: 'application/json',
      },
    };

    const response = await this.client.fetch(url, options);

    return yield* this.processRandomStream(response, signal);
  }

  protected async *processRandomStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<IMediaItem>> {
    const status = response.status;
    const _headers: any = {};
    response.headers?.forEach((v: any, k: any) => (_headers[k] = v));
    if (status === 200) {
      yield* this.processGetRandomMediaItemsStreamBody(
        response.body,
        status,
        _headers,
        signal,
      );
    } else if (status === 404) {
      const text = await response.text();
      throwException('A server side error occurred.', status, text, _headers);
    } else if (status !== 200 && status !== 204) {
      const text = await response.text();
      throwException(
        'An unexpected server error occurred.',
        status,
        text,
        _headers,
      );
    }
    yield new SwaggerResponse(status, _headers, null as any);
  }

  protected async *processGetRandomMediaItemsStreamBody(
    body: ReadableStream<Uint8Array> | null,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<IMediaItem>> {
    if (body === null) {
      yield new SwaggerResponse(status, headers, null as any);
    } else {
      const stream = body.pipeThrough(
        new TextDecoderStream('UTF-8', { fatal: false, ignoreBOM: true }),
        {
          preventAbort: false,
          signal,
        },
      );
      if (isAsyncIterable(stream)) {
        console.debug('processGetRandomMediaItemsStreamBody iterate');
        for await (const chunk of stream.values({})) {
          signal?.throwIfAborted();
          yield* this.processGetRandomMediaItemsChunk(
            chunk,
            status,
            headers,
            signal,
          );
        }
      } else {
        console.debug('processGetRandomMediaItemsStreamBody while');
        const reader = stream.getReader();
        try {
          while (true) {
            signal?.throwIfAborted();
            const { done, value } = await reader.read();
            if (done) {
              break;
            }
            yield* this.processGetRandomMediaItemsChunk(
              value,
              status,
              headers,
              signal,
            );
          }
        } finally {
          reader.releaseLock();
        }
      }
    }
  }

  protected *processGetRandomMediaItemsChunk(
    chunk: string,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ) {
    console.debug('processGetRandomMediaItemsStreamBody chunk', chunk);
    const lines: string[] = chunk
      .trim()
      .split('\n')
      .filter(
        (line: string) =>
          line.trim().length > 0 && line.startsWith('{') && line.endsWith('}'),
      );
    for (const line of lines) {
      signal?.throwIfAborted();
      console.debug('processGetRandomMediaItemsStreamBody line', line);
      try {
        const json = JSON.parse(line, this.jsonParseReviver);
        const media = MediaItem.fromJS(json);
        yield new SwaggerResponse(status, headers, media);
      } catch (error: unknown) {
        console.error(
          'processGetRandomMediaItemsStream invalid data',
          chunk,
          line,
          error,
        );
      }
    }
  }
}
