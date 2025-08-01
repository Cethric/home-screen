import type { Meta, StoryObj } from "@storybook/vue3";
import DateTimeWeatherCombo from "@/components/DateTimeWeatherCombo.vue";
import { DateTimeWeatherComboKinds } from "@/components/properties";

const meta: Meta<typeof DateTimeWeatherCombo> = {
    component: DateTimeWeatherCombo,
    tags: ["autodocs"],
    args: {
        weatherForecast: { feelsLikeTemperature: 0, weatherCode: "Sunny" },
    },
    argTypes: {
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
        kind: {
            type: {
                name: "enum",
                value: Object.values(DateTimeWeatherComboKinds),
            },
        },
    },
};
export default meta;

type DateTimeWeatherComboStory = StoryObj<typeof DateTimeWeatherCombo>;

export const Default: DateTimeWeatherComboStory = {
    render: (args) => ({
        template:
            '<main class="w-dvw h-dvh"><DateTimeWeatherCombo v-bind="args" /></main>',
        components: { DateTimeWeatherCombo },
        setup() {
            return { args };
        },
    }),
};

export const KindHeader: DateTimeWeatherComboStory = {
    render: (args) => ({
        template:
            '<main class="w-dvw h-dvh"><DateTimeWeatherCombo v-bind="args" /></main>',
        components: { DateTimeWeatherCombo },
        setup() {
            return { args };
        },
    }),
    args: { kind: DateTimeWeatherComboKinds.header },
};

export const KindFooter: DateTimeWeatherComboStory = {
    render: (args) => ({
        template:
            '<main class="w-dvw h-dvh"><DateTimeWeatherCombo v-bind="args" /></main>',
        components: { DateTimeWeatherCombo },
        setup() {
            return { args };
        },
    }),
    args: { kind: DateTimeWeatherComboKinds.footer },
};
