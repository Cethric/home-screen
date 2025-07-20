import type { Image } from "@homescreen/web-common-components";
import { DateTime } from "luxon";

export const picsumImages = (count: number = 300) =>
    Array.from({ length: count }).map((_, i) => {
        return {
            id: `https://picsum.photos/400/500/${i}`,
            dateTime: DateTime.now(),
            enabled: true,
            location: {
                name: "Sydney",
                longitude: 151.20732,
                latitude: -33.86785,
            },
            aspectRatio: 500 / 400,
            portrait: true,
            colour: { red: 0, green: 0, blue: 0 },
        } satisfies Image;
    });
