import type { Meta, StoryObj } from "@storybook/vue3";
import RollingSlideModal from "@/slideshows/rolling/RollingSlideModal.vue";
import { picsumImages } from "@/stories/helpers";

const meta: Meta<typeof RollingSlideModal> = {
    component: RollingSlideModal,
    tags: ["autodocs"],
    args: {
        image: picsumImages()[0],
    },
    argTypes: {
        image: {
            type: {
                name: "object",
                value: {
                    id: { name: "string" },
                    dateTime: { name: "object", value: {} },
                    location: {
                        name: "object",
                        value: {
                            name: { name: "string" },
                            latitude: { name: "string" },
                            longitude: { name: "string" },
                        },
                    },
                },
            },
        },
    },
};
export default meta;

type RollingSlideModalStory = StoryObj<typeof RollingSlideModal>;

export const Default: RollingSlideModalStory = {
    render: (args) => ({
        template: '<Suspense><RollingSlideModal v-bind="args"  /></Suspense>',
        components: { RollingSlideModal },
        setup() {
            return { args };
        },
    }),
};
