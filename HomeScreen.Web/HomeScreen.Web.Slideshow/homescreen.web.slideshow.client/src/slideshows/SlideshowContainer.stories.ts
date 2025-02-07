import type { Meta, StoryObj } from '@storybook/vue3';
import SlideshowContainer from '@/slideshows/SlideshowContainer.vue';
import { Slideshows } from '@/slideshows/properties';

const meta: Meta<typeof SlideshowContainer> = {
    component: SlideshowContainer,
    tags: ['autodocs'],
    args: {},
    argTypes: {
        activeSlideshow: { name: 'enum', value: Object.values(Slideshows) },
    },
};
export default meta;

type SlideshowContainerStory = StoryObj<typeof SlideshowContainer>;

export const Default: SlideshowContainerStory = {
    render: (args) => ({
        template: '<Suspense><SlideshowContainer v-bind="args" /></Suspense>',
        components: { SlideshowContainer },
        setup() {
            return { args };
        },
    }),
};

export const FullscreenSlideshow: SlideshowContainerStory = {
    render: (args) => ({
        template: '<Suspense><SlideshowContainer v-bind="args" /></Suspense>',
        components: { SlideshowContainer },
        setup() {
            return { args };
        },
    }),
    args: { activeSlideshow: Slideshows.fullscreen_slideshow },
};

export const PolaroidSlideshow: SlideshowContainerStory = {
    render: (args) => ({
        template: '<Suspense><SlideshowContainer v-bind="args" /></Suspense>',
        components: { SlideshowContainer },
        setup() {
            return { args };
        },
    }),
    args: { activeSlideshow: Slideshows.polaroid_slideshow },
};

export const RollingSlideshow: SlideshowContainerStory = {
    render: (args) => ({
        template: '<Suspense><SlideshowContainer v-bind="args" /></Suspense>',
        components: { SlideshowContainer },
        setup() {
            return { args };
        },
    }),
    args: { activeSlideshow: Slideshows.rolling_slideshow },
};
