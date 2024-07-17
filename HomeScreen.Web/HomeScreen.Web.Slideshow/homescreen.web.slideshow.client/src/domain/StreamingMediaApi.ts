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
    super();
  }

  async *getRandomMediaItemsStream(
    count?: number | undefined,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
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

    // @ts-ignore
    const _response = await this.http.fetch(url_, options_);
    yield* this.processGetRandomMediaItemsStream(_response, signal);
  }

  protected async *processGetRandomMediaItemsStream(
    response: Response,
    signal?: AbortSignal,
  ): AsyncGenerator<SwaggerResponse<MediaItem>> {
    console.debug('processGetRandomMediaItemsStream start');
    const status = response.status;
    const _headers: any = {};
    response.headers?.forEach((v, k) => (_headers[k] = v));
    if (status === 200) {
      const decoder = new TextDecoder('UTF-8', {});
      if (response.body === null) {
        console.debug('processGetRandomMediaItemsStream invalid media');
        yield new SwaggerResponse(status, _headers, null as any);
      }
      // @ts-ignore
      for await (const chunk of response.body) {
        signal?.throwIfAborted();
        const decoded = decoder.decode(chunk);
        console.debug(
          'processGetRandomMediaItemsStream tick media',
          chunk,
          decoded,
        );
        for (const line of decoded
          .split('\n')
          .filter((line) => line.trim().length > 0)) {
          try {
            const parsed = JSON.parse(line, this.jsonParseReviver);
            const item = MediaItem.fromJS(parsed);
            yield new SwaggerResponse(status, _headers, item);
          } catch (error) {
            console.error(
              'processGetRandomMediaItemsStream invalid data',
              chunk,
              decoded,
              line,
              error,
            );
            yield new SwaggerResponse(status, _headers, null as any);
          }
        }
      }
    } else if (status !== 200 && status !== 204) {
      console.debug('processGetRandomMediaItemsStream invalid media');
      yield response.text().then((_responseText) => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers,
        );
      });
    }
    console.debug('processGetRandomMediaItemsStream end');
    yield Promise.resolve<SwaggerResponse<MediaItem>>(
      new SwaggerResponse(status, _headers, null as any),
    );
  }
}
