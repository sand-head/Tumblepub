<template>
  <button
    class="px-2 py-1 rounded transition duration-50"
    :class="classes[props.color]"
    @click="onClick"
  >
    {{ props.label }}
  </button>
</template>

<script setup lang="ts">
  import { defineEmit, defineProps } from 'vue';

  const props = defineProps({
    label: {
      type: String,
      required: true,
    },
    color: {
      type: String,
      default: 'primary',
      validator: (value) => {
        return typeof value === 'string' && value in ['primary', 'secondary'];
      },
    },
  });
  const emit = defineEmit(['click']);

  const classes: { [type: string]: string } = {
    primary: 'bg-purple-500 text-white hover:bg-purple-600',
    secondary: 'bg-gray-200 text-black hover:bg-gray-300',
  };
  function onClick() {
    emit('click');
  }
</script>
