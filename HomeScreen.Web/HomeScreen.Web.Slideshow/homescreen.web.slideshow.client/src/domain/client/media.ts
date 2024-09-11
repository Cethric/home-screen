import {
  type IMediaClientWithStreaming,
  injectMediaApi,
  type MediaItem,
} from '@homescreen/web-common-components';

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
