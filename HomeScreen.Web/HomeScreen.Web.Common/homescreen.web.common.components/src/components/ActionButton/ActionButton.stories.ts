import type { Meta, StoryObj } from "@storybook/vue3";
import { Variants } from "../properties";
import ActionButton from "./ActionButton.vue";

const meta: Meta<typeof ActionButton> = {
	component: ActionButton,
	tags: ["autodocs"],
	argTypes: {
		variant: {
			options: Object.values(Variants),
			type: { name: "enum", value: Object.values(Variants) },
		},
	},
};
export default meta;

type ActionButtonStory = StoryObj<typeof ActionButton>;

export const Default: ActionButtonStory = {
	render: (args) => ({
		template: '<ActionButton v-bind="args">Action Button</ActionButton>',
		components: { ActionButton },
		setup() {
			return { args };
		},
	}),
};

export const Primary: ActionButtonStory = {
	render: (args) => ({
		template: '<ActionButton v-bind="args">Action Button</ActionButton>',
		components: { ActionButton },
		setup() {
			return { args };
		},
	}),
	args: {
		variant: Variants.primary,
	},
};

export const Secondary: ActionButtonStory = {
	render: (args) => ({
		template: '<ActionButton v-bind="args">Action Button</ActionButton>',
		components: { ActionButton },
		setup() {
			return { args };
		},
	}),
	args: {
		variant: Variants.secondary,
	},
};
