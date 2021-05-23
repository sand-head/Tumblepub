import { Story } from '@storybook/vue3';
import Button from '../components/Button.vue';

export default {
  title: 'Components/Button',
  component: Button,
  argTypes: {
    // backgroundColor: { control: 'color' },
    // size: {
    //   control: { type: 'select', options: ['small', 'medium', 'large'] },
    // },
    onClick: {},
  },
};

const Template: Story = (args) => ({
  // Components used in your story `template` are defined in the `components` object
  components: { Button },
  // The story's `args` need to be mapped into the template through the `setup()` method
  setup() {
    return { args };
  },
  // And then the `args` are bound to your component with `v-bind="args"`
  template: '<Button v-bind="args" />',
});

export const Primary = Template.bind({});
Primary.args = {
  label: 'Button',
};

export const Secondary = Template.bind({});
Secondary.args = {
  label: 'Button',
  color: 'secondary',
};
/*
export const Large = Template.bind({});
Large.args = {
  size: 'large',
  label: 'Button',
};

export const Small = Template.bind({});
Small.args = {
  size: 'small',
  label: 'Button',
};
 */
