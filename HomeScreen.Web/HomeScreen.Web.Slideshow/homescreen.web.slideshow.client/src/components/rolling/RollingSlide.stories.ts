import type { Meta, StoryObj } from '@storybook/vue3';
import RollingSlide from '@/components/rolling/RollingSlide.vue';
import { Directions } from '@/helpers/component_properties';
import { loadPicsumImage, picsumImages } from '@/stories/helpers';

const meta: Meta<typeof RollingSlide> = {
  component: RollingSlide,
  tags: ['autodocs'],
  args: {
    images: picsumImages(10),
    direction: Directions.horizontal,
    loadImage: loadPicsumImage,
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
    direction: { type: { name: 'enum', value: Object.values(Directions) } },
    loadImage: { type: { name: 'function' } },
  },
};
export default meta;

type RollingSlideStory = StoryObj<typeof RollingSlide>;

export const Default: RollingSlideStory = {
  render: (args) => ({
    template: '<RollingSlide v-bind="args" />',
    components: { RollingSlide },
    setup() {
      return { args };
    },
  }),
};

export const Horizontal: RollingSlideStory = {
  render: (args) => ({
    template: '<RollingSlide v-bind="args" />',
    components: { RollingSlide },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.horizontal,
  },
};

export const Vertical: RollingSlideStory = {
  render: (args) => ({
    template: '<RollingSlide v-bind="args" />',
    components: { RollingSlide },
    setup() {
      return { args };
    },
  }),
  args: {
    direction: Directions.vertical,
  },
};
