const moment = require("moment");

module.exports = (date, format) => {
  const momentFormat = format
    .replaceAll("%A", "dddd")
    .replaceAll("%B", "MMMM")
    .replaceAll("%-d", "D")
    .replaceAll("%Y", "YYYY")
    .replaceAll("%-I", "h")
    .replaceAll("%M", "mm")
    .replaceAll("%P", "A")
    .replaceAll("%Z", "Z");
  return moment(date).format(momentFormat);
};
