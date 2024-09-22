import {
  ApiException,
  type IMediaClient,
  MediaClient,
  MediaItem,
  PaginatedMediaItem,
  SwaggerResponse,
} from '@/domain/generated/homescreen-common-api';
import { inject } from 'vue';

type Fetcher = {
  fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
};

function hasOwn(obj: object, key: string): boolean {
  if ('hasOwn' in Object.prototype) {
    // @ts-ignore
    return Object.hasOwn(obj, key);
  }
  return Object.prototype.hasOwnProperty.call(obj, key); // NOSONAR
}

function jsonParse(json: any, reviver?: any) {
  json = JSON.parse(json, reviver);

  const byid: Record<string, any> = {};
  const refs: any = [];

  function recurse(obj: any, prop?: any, parent?: any) {
    if (typeof obj !== 'object' || !obj) return obj;

    if ('$ref' in obj) {
      const ref = obj.$ref;
      if (ref in byid) return byid[ref];
      refs.push([parent, prop, ref]);
      return undefined;
    } else if ('$id' in obj) {
      const id = obj.$id;
      delete obj.$id;
      if ('$values' in obj) obj = obj.$values;
      byid[id] = obj;
    }

    if (Array.isArray(obj)) {
      obj = obj.map((v, i) => recurse(v, i, obj));
    } else {
      for (const p in obj) {
        if (hasOwn(obj, p) && obj[p] && typeof obj[p] === 'object')
          obj[p] = recurse(obj[p], p, obj);
      }
    }

    return obj;
  }

  json = recurse(json);

  for (const element of refs) {
    const ref = element;
    ref[0][ref[1]] = byid[ref[2]];
  }

  return json;
}

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

type MaybeAsyncIterable = { [Symbol.asyncIterator]?: unknown };

export function isAsyncIterable<T extends MaybeAsyncIterable>(
  input?: T,
): boolean {
  if (input === null) {
    return false;
  }
  if (input === undefined) {
    return false;
  }

  return typeof input[Symbol.asyncIterator] === 'function';
}

export interface IMediaClientWithStreaming extends IMediaClient {
  randomStream(
    count: number,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>>;

  paginateStream(
    offset: number,
    length: number,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>>;
}

class MediaClientWithStreaming
  extends MediaClient
  implements IMediaClientWithStreaming
{
  private get url(): string {
    // @ts-expect-error
    return this.baseUrl;
  }

  private get client(): Fetcher {
    // @ts-expect-error
    return this.http;
  }

  async *randomStream(
    count: number,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    let url_ = this.url + '/api/media/random?';
    if (count === undefined || count === null)
      throw new Error(
        "The parameter 'count' must be defined and cannot be null.",
      );
    else url_ += 'count=' + encodeURIComponent('' + count) + '&';
    url_ = url_.replace(/[?&]$/, '');

    const options_: RequestInit = {
      method: 'GET',
      signal,
      headers: {
        Accept: 'application/json',
      },
    };

    const _response = await this.client.fetch(url_, options_);
    return yield* this.processRandomStream(_response, signal);
  }

  async *paginateStream(
    offset: number,
    length: number,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    let url_ = this.url + '/api/media/paginate?';
    if (offset === undefined || offset === null)
      throw new Error(
        "The parameter 'offset' must be defined and cannot be null.",
      );
    else url_ += 'offset=' + encodeURIComponent('' + offset) + '&';
    if (length === undefined || length === null)
      throw new Error(
        "The parameter 'length' must be defined and cannot be null.",
      );
    else url_ += 'length=' + encodeURIComponent('' + length) + '&';
    url_ = url_.replace(/[?&]$/, '');

    const options_: RequestInit = {
      method: 'GET',
      signal,
      headers: {
        Accept: 'application/json',
      },
    };
    const _response = await this.client.fetch(url_, options_);
    return yield* this.processPaginateStream(_response, signal);
  }

  protected async *processPaginateStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    signal?.throwIfAborted();
    const status = response.status;
    const _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return yield* this.processPaginateStreamBody(
        response.body,
        status,
        _headers,
        signal,
      );
    } else if (status !== 200 && status !== 204) {
      const _responseText1 = await response.text();
      return throwException(
        'An unexpected server error occurred.',
        status,
        _responseText1,
        _headers,
      );
    }
    return yield new SwaggerResponse(status, _headers, null as any);
  }

  protected async *processRandomStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    const status = response.status;
    const _headers: any = {};
    if (response.headers?.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return yield* this.processRandomStreamBody(
        response.body,
        status,
        _headers,
        signal,
      );
    } else if (status !== 200 && status !== 204) {
      const _responseText1 = await response.text();
      return throwException(
        'An unexpected server error occurred.',
        status,
        _responseText1,
        _headers,
      );
    }
    return yield new SwaggerResponse(status, _headers, null as any);
  }

  private async *processPaginateStreamBody(
    body: ReadableStream<Uint8Array> | null,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    signal?.throwIfAborted();
    if (body === null) {
      return yield new SwaggerResponse(status, headers, null as any);
    }
    // @ts-expect-error
    const stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    } & MaybeAsyncIterable = body.pipeThrough(
      new TextDecoderStream('UTF-8', { fatal: false, ignoreBOM: true }),
      {
        preventAbort: false,
        signal,
      },
    );

    if (isAsyncIterable(stream)) {
      return yield* this.iteratePaginateStream(stream, status, headers, signal);
    }

    return yield* this.whilePaginateStreamHasValues(
      stream,
      status,
      headers,
      signal,
    );
  }

  private async *iteratePaginateStream(
    stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    },
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    signal?.throwIfAborted();
    for await (const chunk of stream.values({ preventCancel: false })) {
      signal?.throwIfAborted();
      if (chunk === undefined || chunk === null || chunk.trim().length === 0) {
        continue;
      }

      yield* this.processGetPaginateMediaItemsChunk(
        chunk,
        status,
        headers,
        signal,
      );
    }
  }

  private async *whilePaginateStreamHasValues(
    stream: ReadableStream<string>,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    const reader = stream.getReader();
    try {
      while (true) {
        signal?.throwIfAborted();
        const { done, value } = await reader.read();
        if (done) {
          break;
        }
        if (
          value === undefined ||
          value === null ||
          value.trim().length === 0
        ) {
          continue;
        }
        yield* this.processGetPaginateMediaItemsChunk(
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

  private async *processGetPaginateMediaItemsChunk(
    value: string,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<PaginatedMediaItem>> {
    signal?.throwIfAborted();
    const _mappings: { source: any; target: any }[] = [];
    const lines = value
      .trim()
      .split('\n')
      .filter(
        (line) =>
          line.trim().length > 0 && line.startsWith('{') && line.endsWith('}'),
      );
    for (const line of lines) {
      signal?.throwIfAborted();
      const resultData200 = jsonParse(line, this.jsonParseReviver);
      const result200 = PaginatedMediaItem.fromJS(resultData200, _mappings);
      if (result200 === null) {
        continue;
      }
      yield new SwaggerResponse(status, headers, result200);
    }
  }

  private async *processRandomStreamBody(
    body: ReadableStream<Uint8Array> | null,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    signal?.throwIfAborted();
    if (body === null) {
      return yield new SwaggerResponse(status, headers, null as any);
    }
    // @ts-expect-error
    const stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    } & MaybeAsyncIterable = body.pipeThrough(
      new TextDecoderStream('UTF-8', { fatal: false, ignoreBOM: true }),
      {
        preventAbort: false,
        signal,
      },
    );

    if (isAsyncIterable(stream)) {
      return yield* this.iterateRandomStream(stream, status, headers, signal);
    }

    return yield* this.whileRandomStreamHasValues(
      stream,
      status,
      headers,
      signal,
    );
  }

  private async *iterateRandomStream(
    stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    },
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    signal?.throwIfAborted();
    console.debug('processGetRandomMediaItemsStreamBody iterate');
    for await (const chunk of stream.values({ preventCancel: false })) {
      signal?.throwIfAborted();
      if (chunk === undefined || chunk === null || chunk.trim().length === 0) {
        continue;
      }

      yield* this.processGetRandomMediaItemsChunk(
        chunk,
        status,
        headers,
        signal,
      );
    }
  }

  private async *whileRandomStreamHasValues(
    stream: ReadableStream<string>,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    console.debug('processGetRandomMediaItemsStreamBody while');
    const reader = stream.getReader();
    try {
      while (true) {
        signal?.throwIfAborted();
        const { done, value } = await reader.read();
        if (done) {
          break;
        }
        if (
          value === undefined ||
          value === null ||
          value.trim().length === 0
        ) {
          continue;
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

  private async *processGetRandomMediaItemsChunk(
    value: string,
    status: number,
    headers: Record<string, unknown>,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    signal?.throwIfAborted();
    const _mappings: { source: any; target: any }[] = [];
    const lines = value
      .trim()
      .split('\n')
      .filter(
        (line) =>
          line.trim().length > 0 && line.startsWith('{') && line.endsWith('}'),
      );
    for (const line of lines) {
      signal?.throwIfAborted();
      const resultData200 = jsonParse(line, this.jsonParseReviver);
      const result200 = MediaItem.fromJS(resultData200, _mappings);
      if (result200 === null) {
        continue;
      }
      yield new SwaggerResponse(status, headers, result200);
    }
  }
}

export function getMediaClient(baseUrl: string): MediaClientWithStreaming {
  return new MediaClientWithStreaming(baseUrl);
}

export const MediaApiProvider = Symbol('MediaApiProvider');

export function injectMediaApi(): IMediaClientWithStreaming {
  return inject<IMediaClientWithStreaming>(MediaApiProvider)!;
}
