import { Directions } from '@homescreen/web-common-components';
import type { Meta, StoryObj } from '@storybook/vue3';
import { RollingDirections } from '@/components/properties';
import RollingSlider from '@/slideshows/rolling/RollingSlider.vue';
import { picsumImages } from '@/stories/helpers';

const meta: Meta<typeof RollingSlider> = {
  component: RollingSlider,
  tags: ['autodocs'],
  args: {
    images: picsumImages(),
    rolling: RollingDirections.forward,
  },
  argTypes: {
    images: {
      type: {
        name: 'array',
        value: {
          name: 'object',
          value: {
            id: { name: 'string' },
            src: {
              name: 'array',
              value: { name: 'string' },
            },
            loading: { name: 'string' },
            width: { name: 'number' },
            height: { name: 'number' },
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
    },
    durationSeconds: { type: 'number', min: 1, max: 60, value: 24 },
    direction: { type: { name: 'enum', value: Object.values(Directions) } },
    rolling: {
      type: { name: 'enum', value: Object.values(RollingDirections) },
    },
  },
};
export default meta;

type RollingSliderStory = StoryObj<typeof RollingSlider>;

export const Default: RollingSliderStory = {
  render: (args) => ({
    template: '<main class="overflow-clip"><RollingSlider v-bind="args"  /></main>',
    components: { RollingSlider },
    setup() {
      return { args };
    },
  }),
};

export const Horizontal: RollingSliderStory = {
  render: (args) => ({
    template: '<main class="overflow-clip"><RollingSlider v-bind="args"  /></main>',
    components: { RollingSlider },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.horizontal,
  },
};

export const Vertical: RollingSliderStory = {
  render: (args) => ({
    template: '<main class="overflow-clip"><RollingSlider v-bind="args" /></main>',
    components: { RollingSlider },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.vertical,
  },
};
