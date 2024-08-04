import { computedAsync, useMemoize } from '@vueuse/core';

export interface ComputedMediaSize {
  width: number;
  height: number;
}

export type ImageFormat = 'Jpeg' | 'WebP' | 'Avif' | 'JpegXL' | 'Png';

export type LoadImageCallback = (
  id: string,
  width: number,
  height: number,
  blur: boolean,
  format: ImageFormat,
  signal?: AbortSignal,
) => Promise<string>;

export const responsiveImageLoader = (
  loading: string,
  format: ImageFormat,
  imageId: string,
  width: number,
  height: number,
  loadImage: LoadImageCallback,
) => {
  const imageSrc = useMemoize(async () => {
    return await loadImage(
      imageId,
      Math.max(width, 200),
      Math.max(height, 200),
      false,
      format,
    );
  });

  return computedAsync(async () => {
    return await imageSrc();
  }, loading);
};
