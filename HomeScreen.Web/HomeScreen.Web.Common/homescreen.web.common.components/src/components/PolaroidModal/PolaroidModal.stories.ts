import type { Meta, StoryObj } from '@storybook/vue3';
import PolaroidModal from '@/components/PolaroidModal/PolaroidModal.vue';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof PolaroidModal> = {
  component: PolaroidModal,
  tags: ['autodocs'],
  args: {
    image: picsumImages()[0],
  },
  argTypes: {
    image: {
      name: 'object',
      value: {
        id: { name: 'string' },
        dateTime: { name: 'object', value: {} },
        location: {
          name: 'object',
          value: {
            name: { name: 'string' },
            latitude: { name: 'string' },
            longitude: { name: 'string' },
          },
        },
      },
    },
  },
};
export default meta;

type PolaroidModalStory = StoryObj<typeof PolaroidModal>;

export const Default: PolaroidModalStory = {
  render: (args) => ({
    template: '<Suspense><PolaroidModal v-bind="args"  /></Suspense>',
    components: { PolaroidModal },
    setup() {
      return { args };
    },
  }),
};
