module.exports = {
  mode: "jit",
  purge: ["./**/*.hbs"],
  darkMode: "media", // or false or 'class'
  theme: {
    extend: {
      boxShadow: {
        border: "0 0 0 4px #fff",
      },
      typography: {
        DEFAULT: {
          css: {
            maxWidth: "none",
          },
        },
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [require("@tailwindcss/typography")],
};
