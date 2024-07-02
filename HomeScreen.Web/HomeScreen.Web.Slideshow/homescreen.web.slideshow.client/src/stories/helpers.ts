import { choice } from '@/helpers/random';
import { v4 as uuid } from 'uuid';
import { DateTime } from 'luxon';

export const picsumImages = () =>
  Array.from({ length: 300 }).map((_, i) => {
    const width = choice([400, 500, 600, 700]);
    const height = choice([400, 500, 600, 700]);
    return {
      id: uuid(),
      src: [`https://picsum.photos/id/${i}/${width}/${height}`],
      loading: `https://picsum.photos/id/${i}/${width}/${height}/?blur=2`,
      width: width,
      height: height,
      dateTime: DateTime.now(),
      location: { name: 'Sydney', longitude: 151.20732, latitude: -33.86785 },
    };
  });
