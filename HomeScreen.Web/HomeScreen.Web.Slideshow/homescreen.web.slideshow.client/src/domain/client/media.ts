import {
  injectMediaApi,
  type MediaItem,
} from '@homescreen/web-common-components';

export const toggleMedia = async (
  imageId: string,
  state: boolean,
): Promise<MediaItem | undefined> => {
  const mediaApi = injectMediaApi();
  console.log('toggleMedia start', imageId, state);
  const response = await mediaApi.toggle(imageId, state);
  console.log('toggleMedia end', response?.id, response?.enabled);
  return response;
};
