import { StreamingMediaApi } from '@/domain/StreamingMediaApi';
import {
  type IMediaItem,
  type MediaItem,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';
import type { LoadImageCallback } from '@homescreen/web-components-client/src/index';

const mediaApi = new StreamingMediaApi();

export async function* loadMedia(
  total: number,
  signal?: AbortSignal,
): AsyncGenerator<IMediaItem> {
  const request = mediaApi.getRandomMediaItemsStream(total, signal);
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
  const response = await mediaApi.getTransformMediaItemUrl(
    imageId,
    width,
    height,
    blur,
    format,
    signal,
  );
  return response.headers['location'];
};
export const loadImageCallback: LoadImageCallback =
  loadImage as LoadImageCallback;

export const toggleMedia = async (
  imageId: string,
  state: boolean,
): Promise<MediaItem> => {
  console.log('toggleMedia start', imageId, state);
  const response = await mediaApi.toggleMediaItem(imageId, state);
  console.log('toggleMedia end', response.result.id, response.result.enabled);
  return response.result;
};
