import { StreamingMediaApi } from '@/domain/StreamingMediaApi';
import {
  type MediaItem,
  MediaTransformOptionsFormat,
} from '@/domain/api/homescreen-slideshow-api';

const mediaApi = new StreamingMediaApi();

export async function* loadMedia(
  total: number,
  signal?: AbortSignal,
): AsyncGenerator<MediaItem> {
  console.debug('Loading random images');
  for await (const response of mediaApi.getRandomMediaItemsStream(
    total,
    signal,
  )) {
    signal?.throwIfAborted();
    console.debug('Received random image', response);
    yield response.result;
  }
  console.debug('Loaded random images');
}

export const loadImage = async (
  imageId: string,
  width: number,
  height: number,
  blur: boolean,
  format: MediaTransformOptionsFormat,
): Promise<string> => {
  const response = await mediaApi.getTransformMediaItemUrl(
    imageId,
    width,
    height,
    blur,
    format,
  );
  return response.headers['location'];
};

export const toggleMedia = async (
  imageId: string,
  state: boolean,
): Promise<MediaItem> => {
  console.log('toggleMedia start', imageId, state);
  const response = await mediaApi.toggleMediaItem(imageId, state);
  console.log('toggleMedia end', response.result.id, response.result.enabled);
  return response.result;
};
