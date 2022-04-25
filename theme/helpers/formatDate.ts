import { format } from 'date-fns';

export default (date: string | Date, formatStr: string) => {
  const momentFormat = formatStr
    .replace("%A", "EEEE")
    .replace("%B", "MMMM")
    .replace("%-d", "d")
    .replace("%Y", "yyyy")
    .replace("%-I", "h")
    .replace("%M", "mm")
    .replace("%p", "a")
    .replace("%Z", "XXX");
  return format(new Date(date), momentFormat);
};
