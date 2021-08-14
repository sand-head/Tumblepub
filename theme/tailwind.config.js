module.exports = {
  mode: "jit",
  purge: ["./**/*.hbs"],
  darkMode: "media", // or false or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [require("@tailwindcss/typography")],
};
