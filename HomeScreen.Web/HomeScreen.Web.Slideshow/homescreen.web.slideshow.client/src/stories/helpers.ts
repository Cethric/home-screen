import { DateTime } from 'luxon';
import { type Image } from '@homescreen/web-components-client/src/index';

export const picsumImages = (count: number = 300) =>
  Array.from({ length: count }).map((_, i) => {
    return {
      id: `${i}`,
      dateTime: DateTime.now(),
      enabled: true,
      location: { name: 'Sydney', longitude: 151.20732, latitude: -33.86785 },
    } satisfies Image;
  });

export const loadPicsumImage = (
  id: string,
  width: number,
  height: number,
  blur: boolean,
  // format: MediaTransformOptionsFormat,
) => {
  return Promise.resolve(
    `https://picsum.photos/id/${id}/${width}/${height}?blur=${blur}`,
  );
};
