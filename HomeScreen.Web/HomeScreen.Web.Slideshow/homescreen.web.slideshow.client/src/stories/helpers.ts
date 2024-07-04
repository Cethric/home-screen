import { DateTime } from 'luxon';
import { type Image } from '@/helpers/component_properties';

export const picsumImages = (count: number = 300) =>
  Array.from({ length: count }).map((_, i) => {
    return {
      id: `${i}`,
      dateTime: DateTime.now(),
      location: { name: 'Sydney', longitude: 151.20732, latitude: -33.86785 },
    } satisfies Image;
  });

export const loadPicsumImage = (
  id: string,
  width: number,
  height: number,
  blur: number,
  // format: MediaTransformOptionsFormat,
) => {
  return Promise.resolve(
    `https://picsum.photos/id/${id}/${width}/${height}?blur=${blur}`,
  );
};
