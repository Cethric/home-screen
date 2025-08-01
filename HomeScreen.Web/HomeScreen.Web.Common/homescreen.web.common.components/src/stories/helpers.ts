import { DateTime } from "luxon";
import type { Image } from "@/helpers/image";

export const picsumImage = (
	width: number,
	height: number,
	portrait: boolean,
	id: number,
) =>
	({
		id: `https://picsum.photos/${width}/${height}/${id}`,
		dateTime: DateTime.now(),
		enabled: true,
		location: { name: "Sydney", longitude: 151.20732, latitude: -33.86785 },
		aspectRatio: width / height,
		portrait: portrait,
		colour: { red: 0, green: 128, blue: 64 },
	}) satisfies Image;

export const picsumImages = (count: number = 300) =>
	Array.from({ length: count }).map((_, i) => picsumImage(800, 600, false, i));
