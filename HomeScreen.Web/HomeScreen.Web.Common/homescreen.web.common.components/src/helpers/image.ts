import { DateTime } from 'luxon';
import { type MaybeRefOrGetter } from 'vue';
import type { MediaItem } from '@homescreen/web-common-components-api';

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
    aspectRatio: number;
    portrait: boolean;
    colour: ImageColour;
}

export interface MediaSize {
    width: number;
    height: number;
}

export type ComputedMediaSize = MaybeRefOrGetter<MediaSize>;

export const transformMediaItemToImage = (item: MediaItem): Image =>
    ({
        id: item.id!,
        dateTime: item.created ?? DateTime.invalid('Created Time Not Provided'),
        enabled: item.enabled!,
        location:
            item.location?.name &&
            item.location?.latitude &&
            item.location.latitude !== 360 &&
            item.location?.longitude &&
            item.location.longitude !== 360
                ? ({
                      name: item.location.name,
                      latitude: item.location.latitude,
                      longitude: item.location.longitude,
                  } satisfies ImageLocation)
                : undefined,
        aspectRatio: item.aspectRatio ?? 16.0 / 9.0,
        portrait: item.portrait ?? false,
        colour: {
            red: item.baseR ?? 128,
            green: item.baseG ?? 128,
            blue: item.baseB ?? 128,
        } satisfies ImageColour,
    }) satisfies Image;
