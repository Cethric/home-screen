import type { Meta, StoryObj } from "@storybook/vue3";
import { Variants } from "../properties";
import LoadingSpinner from "./LoadingSpinner.vue";

const meta: Meta<typeof LoadingSpinner> = {
	component: LoadingSpinner,
	tags: ["autodocs"],
	argTypes: {
		variant: {
			options: Object.values(Variants),
			type: { name: "enum", value: Object.values(Variants) },
		},
	},
};
export default meta;

type LoadingSpinnerStory = StoryObj<typeof LoadingSpinner>;

export const Default: LoadingSpinnerStory = {
	render: (args) => ({
		template: '<LoadingSpinner v-bind="args" />',
		components: { LoadingSpinner },
		setup() {
			return { args };
		},
	}),
};

export const Primary: LoadingSpinnerStory = {
	render: (args) => ({
		template: '<LoadingSpinner v-bind="args" />',
		components: { LoadingSpinner },
		setup() {
			return { args };
		},
	}),
	args: {
		variant: Variants.primary,
	},
};

export const Secondary: LoadingSpinnerStory = {
	render: (args) => ({
		template: '<LoadingSpinner v-bind="args" />',
		components: { LoadingSpinner },
		setup() {
			return { args };
		},
	}),
	args: {
		variant: Variants.secondary,
	},
};

export const Fullscreen: LoadingSpinnerStory = {
	render: (args) => ({
		template:
			'<div class="relative w-dvw h-dvh"><LoadingSpinner class="absolute size-full" v-bind="args" /></div>',
		components: { LoadingSpinner },
		setup() {
			return { args };
		},
	}),
};
