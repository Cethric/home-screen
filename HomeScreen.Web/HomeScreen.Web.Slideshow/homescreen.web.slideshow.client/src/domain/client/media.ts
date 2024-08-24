import {
  type IMediaClientWithStreaming,
  type LoadImageCallback,
  type MediaItem,
  MediaTransformOptionsFormat,
} from '../../../../../HomeScreen.Web.Common/homescreen.web.common.components';
import { inject } from 'vue';

export function injectMediaApi(): IMediaClientWithStreaming {
  return inject<IMediaClientWithStreaming>('mediaApi')!;
}

export async function* loadMedia(
  mediaApi: IMediaClientWithStreaming,
  total: number,
  signal?: AbortSignal,
): AsyncGenerator<MediaItem> {
  const request = mediaApi.randomStream(total, signal);
  for await (const response of request) {
    signal?.throwIfAborted();
    yield response.result;
  }
}

export const loadImage = async (
  imageId: string,
  width: number,
  height: number,
  blur: boolean,
  format: MediaTransformOptionsFormat,
  signal?: AbortSignal,
): Promise<string> => {
  const mediaApi = injectMediaApi();
  const response = await mediaApi.transform(
    imageId,
    width,
    height,
    blur,
    format,
    // signal,
  );
  return (
    response.result.url ||
    response.headers['location'] ||
    response.headers['Location']
  );
};
export const loadImageCallback: LoadImageCallback =
  loadImage as LoadImageCallback;

export const toggleMedia = async (
  imageId: string,
  state: boolean,
): Promise<MediaItem> => {
  const mediaApi = injectMediaApi();
  console.log('toggleMedia start', imageId, state);
  const response = await mediaApi.toggle(imageId, state);
  console.log('toggleMedia end', response.result.id, response.result.enabled);
  return response.result;
};
