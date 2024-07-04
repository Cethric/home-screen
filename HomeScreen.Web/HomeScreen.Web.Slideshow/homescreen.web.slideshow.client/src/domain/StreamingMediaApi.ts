import {
  ApiException,
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

export class StreamingMediaApi extends MediaClient {
  public constructor() {
    super('/api');
  }

  async *getRandomMediaItemsStream(
    count?: number | undefined,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    // @ts-ignore
    let url_ = this.baseUrl + '/Media/GetRandomMediaItems?';
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

    // @ts-ignore
    const _response = await this.http.fetch(url_, options_);
    yield* this.processGetRandomMediaItemsStream(_response, signal);
  }

  protected async *processGetRandomMediaItemsStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    const status = response.status;
    const _headers: any = {};
    response.headers?.forEach((v, k) => (_headers[k] = v));
    if (status === 200) {
      const decoder = new TextDecoder('UTF-8', {});
      if (response.body === null) {
        yield new SwaggerResponse(status, _headers, null as any);
      }
      // @ts-ignore
      for await (const chunk of response.body) {
        signal?.throwIfAborted();
        const parsed = JSON.parse(decoder.decode(chunk), this.jsonParseReviver);
        const item = MediaItem.fromJS(parsed);
        yield new SwaggerResponse(status, _headers, item);
      }
    } else if (status !== 200 && status !== 204) {
      yield response.text().then((_responseText) => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers,
        );
      });
    }
    yield Promise.resolve<SwaggerResponse<MediaItem>>(
      new SwaggerResponse(status, _headers, null as any),
    );
  }
}
