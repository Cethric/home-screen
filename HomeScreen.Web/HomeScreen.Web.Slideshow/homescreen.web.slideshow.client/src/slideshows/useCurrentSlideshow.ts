import { type Slideshow, Slideshows } from '@/slideshows/properties';
import {
    type Component,
    computed,
    type ComputedRef,
    defineAsyncComponent,
} from 'vue';
import FullscreenMainLoader from '@/slideshows/fullscreen/FullscreenMainLoader.vue';
import { choice } from '@/helpers/random';
import { openobserveRum } from '@homescreen/web-common-components';

export const useCurrentSlideshow = (props: {
    activeSlideshow: Slideshow;
}): {
    currentSlideshow: ComputedRef<Component>;
    currentCount: ComputedRef<number>;
    currentTotal: ComputedRef<number>;
} => {
    const slideshows = {
        [Slideshows.rolling_slideshow]: defineAsyncComponent({
            loader: () => import('@/slideshows/rolling/RollingSlideshow.vue'),
            timeout: 10,
            loadingComponent: FullscreenMainLoader,
            onError(error) {
                openobserveRum.addError(error);
            },
        }),
        [Slideshows.polaroid_slideshow]: defineAsyncComponent({
            loader: () => import('@/slideshows/polaroid/PolaroidSlideshow.vue'),
            timeout: 10,
            loadingComponent: FullscreenMainLoader,
            onError(error) {
                openobserveRum.addError(error);
            },
        }),
        [Slideshows.fullscreen_slideshow]: defineAsyncComponent({
            loader: () =>
                import('@/slideshows/fullscreen/FullscreenSlideshow.vue'),
            timeout: 10,
            loadingComponent: FullscreenMainLoader,
            onError(error) {
                openobserveRum.addError(error);
            },
        }),
        [Slideshows.grid_slideshow]: defineAsyncComponent({
            loader: () => import('@/slideshows/grid/GridSlideshow.vue'),
            timeout: 10,
            loadingComponent: FullscreenMainLoader,
            onError(error) {
                openobserveRum.addError(error);
            },
        }),
    };
    const count = {
        [Slideshows.rolling_slideshow]: choice([2, 3, 5]),
        [Slideshows.polaroid_slideshow]: 40,
        [Slideshows.fullscreen_slideshow]: 1,
        [Slideshows.grid_slideshow]: 4,
    };

    const currentSlideshow = computed(() => slideshows[props.activeSlideshow]);
    const currentCount = computed(() => count[props.activeSlideshow]);

    const total = computed(() => ({
        [Slideshows.rolling_slideshow]: 100 * currentCount.value,
        [Slideshows.polaroid_slideshow]: 200,
        [Slideshows.fullscreen_slideshow]: 50,
        [Slideshows.grid_slideshow]: 100,
    }));

    const currentTotal = computed(() => total.value[props.activeSlideshow]);

    return { currentSlideshow, currentCount, currentTotal };
};
