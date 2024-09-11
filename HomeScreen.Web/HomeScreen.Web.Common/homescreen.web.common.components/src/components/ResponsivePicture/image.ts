import type { DateTime } from 'luxon';
import {
  computed,
  type ComputedRef,
  type MaybeRefOrGetter,
  toValue,
} from 'vue';
import type { MediaItem } from '@/domain/generated/homescreen-common-api';

export interface ImageLocation {
  name: string;
  latitude: number;
  longitude: number;
}

export interface ImageColour {
  red: number;
  green: number;
  blue: number;
}

export interface Image {
  id: string;
  dateTime: DateTime;
  enabled: boolean;
  location?: ImageLocation;
  aspectWidth: number;
  aspectHeight: number;
  colour: ImageColour;
}

export interface MediaSize {
  width: number;
  height: number;
}

export type ComputedMediaSize = MaybeRefOrGetter<MediaSize>;

export interface ResponsivePictureProps {
  image: Image;
  imageSize: ComputedMediaSize;
  minImageSize?: ComputedMediaSize;
  maxImageSize?: ComputedMediaSize;
}

interface UseImageAspectSizeProps {
  image: Image;
  size: ComputedMediaSize;
  minSize?: ComputedMediaSize;
  maxSize?: ComputedMediaSize;
}

type UseImageAspectSize = ComputedRef<MediaSize>;

export const useImageAspectSize = ({
  image,
  size,
  minSize = { width: 250, height: 250 },
  maxSize,
}: UseImageAspectSizeProps): UseImageAspectSize =>
  computed(() => {
    const value = toValue(size);
    const min = toValue(minSize);
    const max = toValue(maxSize);
    const resolve = () => {
      if (value.height > value.width) {
        return {
          width: Math.floor(
            Math.max(
              value.width * image.aspectWidth,
              min.width * image.aspectWidth,
            ),
          ),
          height: Math.floor(Math.max(value.width, min.width)),
        };
      }
      return {
        width: Math.floor(Math.max(value.height, min.height)),
        height: Math.floor(
          Math.max(
            value.height * image.aspectHeight,
            min.height * image.aspectHeight,
          ),
        ),
      };
    };
    const resolved = resolve();
    return max === undefined
      ? resolved
      : {
          width: Math.min(resolved.width, max.width),
          height: Math.min(resolved.height, max.height),
        };
  });

export const transformMediaItemToImage = (item: MediaItem): Image =>
  ({
    id: item.id!,
    dateTime: item.created!,
    enabled: item.enabled!,
    location:
      item.location?.name && item.location?.latitude && item.location?.longitude
        ? ({
            name: item.location.name,
            latitude: item.location.latitude,
            longitude: item.location.longitude,
          } satisfies ImageLocation)
        : undefined,
    aspectWidth: item.aspectRatioWidth ?? 16.0 / 9.0,
    aspectHeight: item.aspectRatioHeight ?? 9.0 / 16.0,
    colour: {
      red: item.baseR ?? 128,
      green: item.baseG ?? 128,
      blue: item.baseB ?? 128,
    } satisfies ImageColour,
  }) satisfies Image;

export const imageToColour = (image: Image): ComputedRef<string> =>
  computed(
    () =>
      `rgb(${image.colour.red}, ${image.colour.green}, ${image.colour.blue})`,
  );
