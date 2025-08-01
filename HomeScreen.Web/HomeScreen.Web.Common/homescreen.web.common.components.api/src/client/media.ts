import type { ZodType } from "zod";
import {
	type AcceptedTransformMediaItem,
	type AcceptedTransformMediaLine,
	type MediaItem,
	type MediaTransformOptionsFormat,
	mediaAll,
	mediaGet,
	mediaGet2,
	mediaGet3,
	mediaGet4,
	mediaGet5,
	mediaGetResponseTransformer,
	mediaPatch,
	type PaginatedMediaItem,
	zMediaItem,
	zPaginatedMediaItem,
} from "../generated";
import type { Client } from "../generated/client";

export function isAsyncIterable<
	TValue,
	TIterable extends Partial<AsyncIterable<TValue>>,
>(input?: TIterable): boolean {
	if (input === null) {
		return false;
	}
	if (input === undefined) {
		return false;
	}

	return typeof input[Symbol.asyncIterator] === "function";
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

type Stream = Partial<AsyncIterable<string>> &
	ReadableStream<string> & {
		values(options: { preventCancel: boolean }): string[];
	};

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
			parseAs: "stream",
			signal,
			async responseTransformer() {
				return [];
			},
		});
		if (response.response.status === 200) {
			return yield* this.processStream(
				response.response,
				zPaginatedMediaItem as unknown as ZodType<PaginatedMediaItem>,
				signal,
			);
		}
	}

	public async *random(
		count: number,
		signal?: AbortSignal,
	): AsyncGenerator<MediaItem | undefined> {
		const response = await mediaGet({
			client: this.client,
			query: { count },
			parseAs: "stream",
			signal,
			async responseTransformer() {
				return [];
			},
		});
		if (response.response.status === 200) {
			for await (const entry of this.processStream(
				response.response,
				zMediaItem,
				signal,
			)) {
				yield await mediaGetResponseTransformer(entry);
			}
		}
		return [];
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
		if (response.response.status === 200) return response.data;
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
			parseAs: "blob",
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
		if (response.response.status === 202)
			return response.data as AcceptedTransformMediaItem;
	}

	private async *processStream<TValue, TValidator extends ZodType<TValue>>(
		response: Response,
		responseValidator: TValidator,
		signal?: AbortSignal,
	): AsyncIterable<TValue> {
		const stream = response.body?.pipeThrough(
			new TextDecoderStream("UTF-8", { fatal: false, ignoreBOM: true }),
			{
				preventAbort: false,
			},
		) as Stream | undefined;
		if (stream !== undefined) {
			if (isAsyncIterable(stream)) {
				return yield* this.iterateStream<TValue, TValidator>(
					stream,
					responseValidator,
					signal,
				);
			}

			return yield* this.whileStreamHasValues<TValue, TValidator>(
				stream,
				responseValidator,
				signal,
			);
		}
	}

	private async *iterateStream<TValue, TValidator extends ZodType<TValue>>(
		stream: ReadableStream<string>,
		validator: TValidator,
		signal?: AbortSignal,
	): AsyncGenerator<TValue> {
		signal?.throwIfAborted();
		for await (const chunk of stream.values({ preventCancel: false })) {
			signal?.throwIfAborted();
			yield* this.processChunk<TValue, TValidator>(validator, chunk, signal);
		}
	}

	private async *whileStreamHasValues<
		TValue,
		TValidator extends ZodType<TValue>,
	>(
		stream: ReadableStream<string>,
		validator: TValidator,
		signal?: AbortSignal,
	): AsyncGenerator<TValue> {
		signal?.throwIfAborted();
		const reader = stream.getReader();
		try {
			while (true) {
				signal?.throwIfAborted();
				const { done, value } = await reader.read();
				if (done) {
					break;
				}
				yield* this.processChunk<TValue, TValidator>(validator, value, signal);
			}
		} finally {
			reader.releaseLock();
		}
	}

	private async *processChunk<TValue, TValidator extends ZodType<TValue>>(
		validator: TValidator,
		chunk?: string,
		signal?: AbortSignal,
	): AsyncGenerator<TValue> {
		signal?.throwIfAborted();

		if (chunk === undefined || chunk === null || chunk.trim().length === 0) {
			return;
		}

		const lines = chunk
			.trim()
			.split("\n")
			.filter(
				(line) =>
					line.trim().length > 0 && line.startsWith("{") && line.endsWith("}"),
			);
		for (const line of lines) {
			signal?.throwIfAborted();
			let parsed = {};
			try {
				parsed = JSON.parse(line, undefined);
			} catch (error) {
				console.error("failed to parse line", line, error);
			}
			const data = await validator.safeParseAsync(parsed);
			if (data.success && data.data) {
				yield data.data as TValue;
			} else {
				console.error("failed to parse line", line, data.error);
			}
		}
	}
}
