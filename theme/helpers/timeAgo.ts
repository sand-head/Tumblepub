import { formatDistanceToNow } from 'date-fns';

export default (date: string | Date) => {
  return formatDistanceToNow(new Date(date));
};
