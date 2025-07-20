import type { Image } from "@homescreen/web-common-components";
import { Directions } from "@homescreen/web-common-components";
import type { Meta, StoryObj } from "@storybook/vue3";
import RollingSlideshow from "@/slideshows/rolling/RollingSlideshow.vue";
import { picsumImages } from "@/stories/helpers";

const meta: Meta<typeof RollingSlideshow> = {
    component: RollingSlideshow,
    tags: ["autodocs"],
    args: {
        images: picsumImages().reduce<Record<Image["id"], Image>>((p, c) => {
            p[c.id] = c;
            return p;
        }, {}),
        weatherForecast: { feelsLikeTemperature: 0, weatherCode: "Sunny" },
    },
    argTypes: {
        images: {
            type: {
                name: "array",
                value: {
                    name: "object",
                    value: {
                        id: { name: "string" },
                        src: {
                            name: "array",
                            value: { name: "string" },
                        },
                        loading: { name: "string" },
                        width: { name: "number" },
                        height: { name: "number" },
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
        weatherForecast: {
            type: {
                name: "object",
                value: {
                    feelsLikeTemperature: { name: "number" },
                    maxTemperature: { name: "number" },
                    minTemperature: { name: "number" },
                    chanceOfRain: { name: "number" },
                    amountOfRain: { name: "number" },
                    weatherCode: { name: "string" },
                },
            },
        },
        direction: { type: { name: "enum", value: Object.values(Directions) } },
        durationSeconds: { type: "number", min: 1, max: 60, value: 8 },
        count: { type: "number", min: 1, max: 8, value: 2 },
    },
};
export default meta;

type RollingSlideshowStory = StoryObj<typeof RollingSlideshow>;

export const Default: RollingSlideshowStory = {
    render: (args) => ({
        template: '<RollingSlideshow v-bind="args" />',
        components: { RollingSlideshow },
        setup() {
            return { args };
        },
    }),
};

export const Horizontal: RollingSlideshowStory = {
    render: (args) => ({
        template: '<RollingSlideshow v-bind="args" />',
        components: { RollingSlideshow },
        setup() {
            return { args };
        },
    }),
    args: {
        direction: Directions.horizontal,
    },
};

export const Vertical: RollingSlideshowStory = {
    render: (args) => ({
        template: '<RollingSlideshow v-bind="args" />',
        components: { RollingSlideshow },
        setup() {
            return { args };
        },
    }),
    args: {
        direction: Directions.vertical,
    },
};
