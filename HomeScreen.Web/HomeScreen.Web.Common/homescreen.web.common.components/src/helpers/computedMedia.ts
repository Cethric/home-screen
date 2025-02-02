import { computedAsync, useMemoize } from '@vueuse/core';
import { type Ref } from 'vue';
import {
  injectComponentMediaClient,
  MediaTransformOptionsFormat,
} from '@homescreen/web-common-components-api';

export const loadImage = async (
  imageId: string,
  width: number,
  height: number,
  blur: boolean,
  format: MediaTransformOptionsFormat,
  signal?: AbortSignal,
): Promise<string> => {
  const mediaApi = injectComponentMediaClient();
  const response = await mediaApi.transformItem(
    imageId,
    Math.floor(width),
    Math.floor(height),
    blur,
    format,
    signal,
  );

  return response?.url ?? 'invalid-url';
};

export type LoadImageCallback = typeof loadImage;

export const asyncImage = (
  loadImage: LoadImageCallback,
  format: MediaTransformOptionsFormat,
  imageId: string,
  width: number,
  height: number,
  blur?: boolean,
): Ref<string | null> => {
  const imageSrc = useMemoize(async (width: number, height: number) => {
    return await loadImage(
      imageId,
      Math.max(width, 250),
      Math.max(height, 250),
      blur ?? false,
      format,
    );
  });

  return computedAsync(async () => {
    const result = await imageSrc(width, height);
    const image = window.document.createElement('img');
    image.loading = 'eager';
    image.src = result;
    return new Promise((resolve) =>
      image.addEventListener('load', () => {
        resolve(result);
        image.remove();
      }),
    );
  }, null);
};
