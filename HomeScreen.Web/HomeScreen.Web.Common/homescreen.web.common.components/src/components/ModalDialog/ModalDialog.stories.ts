import type { Meta, StoryObj } from "@storybook/vue3";
import ModalDialog from "./ModalDialog.vue";

const meta: Meta<typeof ModalDialog> = {
	component: ModalDialog,
	tags: ["autodocs"],
	argTypes: {},
};
export default meta;

type ModalDialogStory = StoryObj<typeof ModalDialog>;

export const Default: ModalDialogStory = {
	render: (args) => ({
		template:
			'<ModalDialog v-bind="args"><template #header-center>Header Center</template><template #default><p>Hello, World!</p></template><template #activator="props"><button class="btn btn-neutral" v-bind="props">Activator</button></template></ModalDialog>',
		components: { ModalDialog },
		setup() {
			return { args };
		},
	}),
};
