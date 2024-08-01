import {
  ApiException,
  type IMediaItem,
  MediaClient,
  MediaItem,
  SwaggerResponse,
} from '@/domain/api/homescreen-slideshow-api';

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

function isAsyncIterable<T>(input: T): boolean {
  if (input === null) {
    return false;
  }

  return typeof input[Symbol.asyncIterator] === 'function';
}

export class StreamingMediaApi extends MediaClient {
  public constructor() {
    super();
  }

  async *getRandomMediaItemsStream(
    count?: number | undefined,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<IMediaItem>> {
    // @ts-ignore
    let url_ = this.baseUrl + '/api/Media/GetRandomMediaItems?';
    if (count === null)
      throw new Error("The parameter 'count' cannot be null.");
    else if (count !== undefined)
      url_ += 'count=' + encodeURIComponent('' + count) + '&';
    url_ = url_.replace(/[?&]$/, '');

    const options_: RequestInit = {
      method: 'GET',
      signal,
      headers: {
        Accept: 'application/json',
      },
    };

    console.log('getRandomMediaItemsStream');
    // @ts-ignore
    const _response = await this.http.fetch(url_, options_);
    yield* this.processGetRandomMediaItemsStream(_response, signal);
  }

  protected async *processGetRandomMediaItemsStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<IMediaItem>> {
    const status = response.status;
    const headers: Record<string, any> = {};
    response.headers?.forEach((v, k) => (headers[k] = v));
    if (status === 200) {
      yield* this.processGetRandomMediaItemsStreamBody(
        response.body,
        status,
        headers,
        signal,
      );
    } else if (status !== 200 && status !== 204) {
      const _responseText = await response.text();
      throwException(
        'An unexpected server error occurred.',
        status,
        _responseText,
        headers,
      );
    }
    yield Promise.resolve<SwaggerResponse<IMediaItem>>(
      new SwaggerResponse(status, headers, null as any),
    );
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
      if (!isAsyncIterable(stream)) {
        console.log('processGetRandomMediaItemsStreamBody while');
        const reader = stream.getReader();
        try {
          while (true) {
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
      } else {
        console.log('processGetRandomMediaItemsStreamBody iterate');
        for await (const chunk of stream.values({})) {
          console.log('processGetRandomMediaItemsStreamBody chunk', chunk);
          signal?.throwIfAborted();
          yield* this.processGetRandomMediaItemsChunk(
            chunk,
            status,
            headers,
            signal,
          );
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
    const lines: string[] = chunk
      .split('\n')
      .filter((line: string) => line.trim().length > 0);
    for (const line of lines) {
      signal?.throwIfAborted();
      console.log('processGetRandomMediaItemsStreamBody line', line);
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
        yield new SwaggerResponse(status, headers, null as any);
      }
    }
  }
}
