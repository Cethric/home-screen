export interface ComputedMediaSize {
  width: number;
  height: number;
}

export type LoadImageCallback = (
  id: string,
  width: number,
  height: number,
  blur: boolean,
  format: 'Jpeg' | 'WebP' | 'Avif' | 'JpegXL' | 'Png',
  signal?: AbortSignal,
) => Promise<string>;
