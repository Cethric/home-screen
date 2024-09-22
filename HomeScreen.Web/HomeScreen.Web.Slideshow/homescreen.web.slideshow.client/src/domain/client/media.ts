import {
  injectMediaApi,
  type MediaItem,
} from '@homescreen/web-common-components';

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
