<template>
  <button :class="classes[props.type]" @click="onClick">{{ props.label }}</button>
</template>

<script setup lang="ts">
  import { defineEmit, defineProps } from "vue";
  const props = defineProps({
    label: {
      type: String,
      required: true,
    },
    type: {
      type: String,
      default: 'primary',
      validator: (value) => {
        return typeof value === 'string' && value in ['primary', 'secondary'];
      },
    }
  });
  const emit = defineEmit(['click']);

  const classes: {[type: string]: string} = {
    'primary': 'bg-blue-500 text-white',
    'secondary': 'bg-gray-800 text-white'
  };
  function onClick() {
    emit('click');
  }
</script>
