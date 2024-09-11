import type { Image } from '@/components/ResponsivePicture/image';

export interface PolaroidImage {
  image: Image;
  top: number;
  left: number;
  rotation: number;
}
