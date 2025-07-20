import type { Image } from "@/helpers/image";

export interface PolaroidImage {
    image: Image;
    top: number;
    left: number;
    rotation: number;
}
