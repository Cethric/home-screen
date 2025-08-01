import { refDebounced, useWindowSize } from "@vueuse/core";
import { computed, type Ref, toValue } from "vue";
import type { Image } from "@/helpers/image";

export const useImageSize: (args: {
	image: Image;
	border?: number;
	maxSize?: number;
}) => {
	size: Readonly<Ref<number, number>>;
} = ({ image, border = 400, maxSize = 0 }) => {
	const { width, height } = useWindowSize({
		initialWidth: 500,
		initialHeight: 500,
		listenOrientation: true,
		type: "visual",
	});

	const computedRatio = computed(() => {
		const base =
			toValue(width) > toValue(height)
				? toValue(height) - border
				: toValue(width) - border;
		const ratio = base * image.aspectRatio;
		if (ratio > toValue(height)) {
			return ratio - (ratio - toValue(height)) - border;
		}
		if (ratio > toValue(width)) {
			return ratio - (ratio - toValue(width)) - border;
		}
		return ratio;
	});

	const computedSize = computed(() => {
		if (maxSize > 0 && maxSize < toValue(computedRatio)) {
			return Math.min(maxSize, toValue(computedRatio));
		}
		return toValue(computedRatio);
	});

	const debouncedSize = refDebounced(computedSize, 250);

	return { size: debouncedSize };
};
