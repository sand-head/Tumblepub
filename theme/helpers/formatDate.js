const moment = require("moment");

module.exports = (date, format) => {
  const momentFormat = format
    .replace("%A", "dddd")
    .replace("%B", "MMMM")
    .replace("%-d", "D")
    .replace("%Y", "YYYY")
    .replace("%-I", "h")
    .replace("%M", "mm")
    .replace("%p", "A")
    .replace("%Z", "Z");
  return moment(date).format(momentFormat);
};
