import {DateTime} from 'luxon';
import {type Image} from '@homescreen/web-common-components';

export const picsumImages = (count: number = 300) =>
    Array.from({length: count}).map((_, i) => {
        return {
            id: `https://picsum.photos/400/500/${i}`,
            dateTime: DateTime.now(),
            enabled: true,
            location: {name: 'Sydney', longitude: 151.20732, latitude: -33.86785},
            aspectWidth: 16.0 / 9.0,
            aspectHeight: 9.0 / 16.0,
            colour: {red: 0, green: 0, blue: 0},
        } satisfies Image;
    });
