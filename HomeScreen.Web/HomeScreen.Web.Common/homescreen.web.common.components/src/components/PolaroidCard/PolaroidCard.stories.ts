import type { Meta, StoryObj } from "@storybook/vue3";
import { picsumImage } from "@/stories/helpers";
import { Directions } from "../properties";
import PolaroidCard from "./PolaroidCard.vue";

const meta: Meta<typeof PolaroidCard> = {
	component: PolaroidCard,
	tags: ["autodocs"],
	args: {
		image: picsumImage(800, 600, false, 2),
		maxSize: 800,
	},
	argTypes: {
		direction: {
			options: Object.values(Directions),
			type: { name: "enum", value: Object.values(Directions) },
		},
		maxSize: {
			type: { name: "number" },
		},
		image: {
			type: {
				name: "object",
				value: {
					id: { name: "string" },
					dateTime: { name: "object", value: {} },
					enabled: { name: "boolean" },
					location: {
						name: "object",
						value: {
							name: { name: "string" },
							latitude: { name: "number" },
							longitude: { name: "number" },
						},
					},
				},
			},
		},
		flat: {
			type: "boolean",
		},
	},
};
export default meta;

type PolaroidCardStory = StoryObj<typeof PolaroidCard>;

export const Default: PolaroidCardStory = {
	render: (args) => ({
		template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
		components: { PolaroidCard },
		setup() {
			return { args };
		},
	}),
};

export const Portrait: PolaroidCardStory = {
	render: (args) => ({
		template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
		components: { PolaroidCard },
		setup() {
			return { args };
		},
	}),
	args: {
		image: picsumImage(600, 800, true, 2),
		maxSize: 600,
	},
};

export const Horizontal: PolaroidCardStory = {
	render: (args) => ({
		template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
		components: { PolaroidCard },
		setup() {
			return { args };
		},
	}),
	args: { direction: Directions.horizontal, maxSize: 800 },
};

export const Vertical: PolaroidCardStory = {
	render: (args) => ({
		template: '<Suspense><PolaroidCard v-bind="args" /></Suspense>',
		components: { PolaroidCard },
		setup() {
			return { args };
		},
	}),
	args: { direction: Directions.vertical, maxSize: 800 },
};
