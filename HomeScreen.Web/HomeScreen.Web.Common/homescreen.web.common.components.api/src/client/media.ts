import type { Client } from '@hey-api/client-fetch';
import {
  type AcceptedTransformMediaItem,
  type AcceptedTransformMediaLine,
  mediaAll,
  mediaGet,
  mediaGet2,
  mediaGet3,
  mediaGet4,
  mediaGet5,
  type MediaItem,
  mediaPatch,
  MediaTransformOptionsFormat,
  type PaginatedMediaItem,
} from '../generated';

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

type FileStreamHttpResult = {
  data: ReadableStream;
};

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
  public constructor(private readonly client: Client) {}

  public async *paginate(
    offset: number,
    length: number,
    signal?: AbortSignal,
  ): AsyncGenerator<PaginatedMediaItem> {
    const response = await mediaAll({
      client: this.client,
      query: { offset, length },
      parseAs: 'stream',
      signal,
      async responseTransformer(data) {
        return [];
      },
    });
    if (response.response.status === 200) {
      type Stream = MaybeAsyncIterable &
        ReadableStream<string> & {
          values(options: { preventCancel: boolean }): string[];
        };
      const stream: Stream | undefined = response.response.body?.pipeThrough(
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
    const response = await mediaGet({
      client: this.client,
      query: { count },
      parseAs: 'stream',
      signal,
      async responseTransformer(data) {
        return [];
      },
    });
    if (response.response.status === 200) {
      type Stream = MaybeAsyncIterable &
        ReadableStream<string> & {
          values(options: { preventCancel: boolean }): string[];
        };
      const stream: Stream | undefined = response.response.body?.pipeThrough(
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
    const response = await mediaPatch({
      client: this.client,
      path: { mediaId },
      query: { enabled },
      signal,
    });
    return response.data;
  }

  public async downloadLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined> {
    const response = await mediaGet3({
      client: this.client,
      path: { direction, size },
      signal,
    });
    if (response.response.status === 200)
      return response.data as FileStreamHttpResult;
  }

  public async transformLine(
    direction: number,
    size: number,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaLine | undefined> {
    const response = await mediaGet5({
      client: this.client,
      path: { direction, size },
      signal,
    });
    if (response.response.status === 202) return response.data;
  }

  public async downloadItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<FileStreamHttpResult | undefined> {
    const response = await mediaGet2({
      client: this.client,
      path: { mediaId, width, height },
      query: { blur, format },
      parseAs: 'blob',
      signal,
    });
    if (response.response.status === 200)
      return response.data as FileStreamHttpResult;
  }

  public async transformItem(
    mediaId: string,
    width: number,
    height: number,
    blur: boolean,
    format: MediaTransformOptionsFormat,
    signal?: AbortSignal,
  ): Promise<AcceptedTransformMediaItem | undefined> {
    const response = await mediaGet4({
      client: this.client,
      path: { mediaId, width, height },
      query: { blur, format },
      signal,
    });
    if (response.response.status === 202) return response.data;
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
