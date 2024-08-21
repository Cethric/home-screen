import { computedAsync, useMemoize } from '@vueuse/core';
import { type Ref } from 'vue';
import {MediaTransformOptionsFormat} from "@/domain/generated/homescreen-common-api";

export interface ComputedMediaSize {
  width: number;
  height: number;
}

export type LoadImageCallback = (
  id: string,
  width: number,
  height: number,
  blur: boolean,
  format: MediaTransformOptionsFormat,
  signal?: AbortSignal,
) => Promise<string>;

export const responsiveImageLoader = (
  loadImage: LoadImageCallback,
  format: MediaTransformOptionsFormat,
  imageId: string,
  width: number,
  height: number,
  loading: Ref<string | null>,
) => {
  const imageSrc = useMemoize(async (width: number, height: number) => {
    return await loadImage(
      imageId,
      Math.max(width, 200),
      Math.max(height, 200),
      false,
      format,
    );
  });

  return computedAsync(async () => {
    if (loading.value === null || loading.value.trim().length === 0) {
      return '';
    }
    return await imageSrc(width, height);
  }, loading?.value ?? '');
};

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
      Math.max(width, 200),
      Math.max(height, 200),
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
