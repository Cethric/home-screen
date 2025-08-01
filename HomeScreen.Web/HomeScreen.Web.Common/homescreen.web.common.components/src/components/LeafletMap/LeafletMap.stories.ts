import type { Meta, StoryObj } from "@storybook/vue3";
import LeafletMap from "./LeafletMap.vue";

const meta: Meta<typeof LeafletMap> = {
    component: LeafletMap,
    tags: ["autodocs"],
    argTypes: {
        latitude: { type: "number" },
        longitude: { type: "number" },
        tooltip: { type: "string" },
    },
    args: {
        latitude: -33.86785,
        longitude: 151.20732,
        tooltip: "Hello, World!",
    },
};
export default meta;

type OpenLayersMapStory = StoryObj<typeof LeafletMap>;

export const Default: OpenLayersMapStory = {
    render: (args) => ({
        template: "<LeafletMap v-bind='args'/>",
        components: { LeafletMap },
        setup() {
            return { args };
        },
    }),
};
