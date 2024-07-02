import type { Meta, StoryObj } from '@storybook/vue3';
import OpenLayersMap from './OpenLayersMap.vue';

const meta: Meta<typeof OpenLayersMap> = {
  component: OpenLayersMap,
  tags: ['autodocs'],
  argTypes: {
    latitude: { type: 'number' },
    longitude: { type: 'number' },
  },
  args: {
    latitude: -33.86785,
    longitude: 151.20732,
  },
};
export default meta;

type OpenLayersMapStory = StoryObj<typeof OpenLayersMap>;

export const Default: OpenLayersMapStory = {
  render: (args) => ({
    template: "<OpenLayersMap class='h-dvh w-dvw' v-bind='args'/>",
    components: { OpenLayersMap },
    setup() {
      return { args };
    },
  }),
};
