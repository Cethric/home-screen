import { inject } from 'vue';
import { type ApiClient, ApiError } from '@/domain/client/api.ts';
import type { components } from '@/domain/generated/schema';

export type PaginatedMediaItem = components['schemas']['PaginatedMediaItem'];

export type MediaItem = components['schemas']['MediaItem'];

export type FileStreamHttpResult = Blob;

export type MediaTransformOptionsFormat =
  components['schemas']['MediaTransformOptionsFormat'];

export type AcceptedTransformMediaLine =
  components['schemas']['AcceptedTransformMediaLine'];

export type AcceptedTransformMediaItem =
  components['schemas']['AcceptedTransformMediaItem'];

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

export interface IMediaClient {
  paginate(
    offset: number,
    length: number,
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem>;

  random(
    count: number,
    signal?: AbortSignal,
  ): AsyncGenerator<MediaItem | undefined>;

  toggle(
    mediaId: string,
    enabled: boolean,
    signal?: AbortSignal,
  ): Promise<MediaItem | undefined>;

  downloadLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined>;

  transformLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaLine | undefined>;

  downloadItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined>;

  transformItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaItem | undefined>;
}

export class MediaClient implements IMediaClient {
  public constructor(private readonly client: ApiClient) {}

  public async *paginate(
    offset: number,
    length: number,
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem> {
    const response = await this.client.GET('/api/media/paginate', {
      params: { query: { offset, length } },
      parseAs: 'stream',
      signal,
    });
    if (response.error) {
      // @ts-expect-error
      throw new ApiError(response.error);
    }
    if (response.response.status === 200) {
      type Stream = MaybeAsyncIterable &
        ReadableStream<string> & {
          values(options: { preventCancel: boolean }): string[];
        };
      const stream: Stream | undefined = response.data?.pipeThrough(
        new TextDecoderStream('UTF-8', { fatal: false, ignoreBOM: true }),
        {
          preventAbort: false,
        },
      ) as ReadableStream<string> & {
        values(options: { preventCancel: boolean }): string[];
      };
      if (stream != undefined) {
        if (isAsyncIterable(stream)) {
          return yield* this.iteratePaginateStream(stream, signal);
        }

        return yield* this.whilePaginateStreamHasValues(stream, signal);
      }
    }
  }

  public async *random(
    count: number,
    signal?: AbortSignal,
  ): AsyncGenerator<MediaItem | undefined> {
    const response = await this.client.GET('/api/media/random', {
      params: { query: { count } },
      parseAs: 'stream',
      signal,
    });
    if (response.error) {
      // @ts-expect-error
      throw new ApiError(response.error);
    }
    if (response.response.status === 200) {
      type Stream = MaybeAsyncIterable &
        ReadableStream<string> & {
          values(options: { preventCancel: boolean }): string[];
        };
      const stream: Stream | undefined = response.data?.pipeThrough(
        new TextDecoderStream('UTF-8', { fatal: false, ignoreBOM: true }),
        {
          preventAbort: false,
        },
      ) as ReadableStream<string> & {
        values(options: { preventCancel: boolean }): string[];
      };
      if (stream != undefined) {
        if (isAsyncIterable(stream)) {
          return yield* this.iterateRandomStream(stream, signal);
        }

        return yield* this.whileRandomStreamHasValues(stream, signal);
      }
    }
    return undefined;
  }

  public async toggle(
    mediaId: string,
    enabled: boolean,
    signal?: AbortSignal,
  ): Promise<MediaItem | undefined> {
    const response = await this.client.PATCH(
      '/api/media/item/{mediaId}/toggle',
      {
        params: {
          path: { mediaId },
          query: { enabled },
        },
        signal,
      },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }

  public async downloadLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined> {
    const response = await this.client.GET(
      '/api/media/download/line/{direction}/{size}',
      {
        params: { path: { direction, size } },
        parseAs: 'blob',
        signal,
      },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }

  public async transformLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaLine | undefined> {
    const response = await this.client.GET(
      '/api/media/transform/line/{direction}/{size}',
      {
        params: { path: { direction, size } },
        signal,
      },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    if (response.response.status === 202) return response.data;
    return undefined;
  }

  public async downloadItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined> {
    const response = await this.client.GET(
      '/api/media/download/item/{mediaId}/{width}/{height}',
      {
        params: { path: { mediaId, width, height }, query: { blur, format } },
        parseAs: 'blob',
        signal,
      },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    return response.data;
  }

  public async transformItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaItem | undefined> {
    const response = await this.client.GET(
      '/api/media/transform/item/{mediaId}/{width}/{height}',
      {
        params: { path: { mediaId, width, height }, query: { blur, format } },
        signal,
      },
    );
    if (response.error) {
      throw new ApiError(response.error);
    }
    if (response.response.status === 202) return response.data;
    return undefined;
  }

  private async *iteratePaginateStream(
    stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    },
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem> {
    signal?.throwIfAborted();
    for await (const chunk of stream.values({ preventCancel: false })) {
      signal?.throwIfAborted();
      if (chunk === undefined || chunk === null || chunk.trim().length === 0) {
        continue;
      }

      yield* this.processGetPaginateMediaItemsChunk(chunk, signal);
    }
  }

  private async *whilePaginateStreamHasValues(
    stream: ReadableStream<string>,
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem> {
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
        yield* this.processGetPaginateMediaItemsChunk(value, signal);
      }
    } finally {
      reader.releaseLock();
    }
  }

  private async *processGetPaginateMediaItemsChunk(
    value: string,
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem> {
    signal?.throwIfAborted();
    const lines = value
      .trim()
      .split('\n')
      .filter(
        (line) =>
          line.trim().length > 0 && line.startsWith('{') && line.endsWith('}'),
      );
    for (const line of lines) {
      signal?.throwIfAborted();
      try {
        const data = JSON.parse(line, undefined);
        if (data === null) {
          continue;
        }
        yield data;
      } catch (error) {
        console.error('failed to parse line', line, error);
      }
    }
  }

  private async *iterateRandomStream(
    stream: ReadableStream<string> & {
      values(options: { preventCancel: boolean }): string[];
    },
    signal?: AbortSignal,
  ): AsyncGenerator<MediaItem> {
    signal?.throwIfAborted();
    for await (const chunk of stream.values({ preventCancel: false })) {
      signal?.throwIfAborted();
      if (chunk === undefined || chunk === null || chunk.trim().length === 0) {
        continue;
      }

      yield* this.processGetRandomMediaItemsChunk(chunk, signal);
    }
  }

  private async *whileRandomStreamHasValues(
    stream: ReadableStream<string>,
    signal?: AbortSignal,
  ): AsyncGenerator<MediaItem> {
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
        yield* this.processGetRandomMediaItemsChunk(value, signal);
      }
    } finally {
      reader.releaseLock();
    }
  }

  private async *processGetRandomMediaItemsChunk(
    value: string,
    signal?: AbortSignal,
  ): AsyncGenerator<MediaItem> {
    signal?.throwIfAborted();
    const lines = value
      .trim()
      .split('\n')
      .filter(
        (line) =>
          line.trim().length > 0 && line.startsWith('{') && line.endsWith('}'),
      );
    for (const line of lines) {
      signal?.throwIfAborted();
      try {
        const data = JSON.parse(line, undefined);
        if (data === null) {
          continue;
        }
        yield data;
      } catch (error) {
        console.error('failed to parse line', line, error);
      }
    }
  }
}

export function getMediaClient(client: ApiClient): MediaClient {
  return new MediaClient(client);
}

export const MediaApiProvider = Symbol('MediaApiProvider');

export function injectMediaApi(): IMediaClient {
  return inject<IMediaClient>(MediaApiProvider)!;
}
