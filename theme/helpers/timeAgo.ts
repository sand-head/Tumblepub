import { formatDistanceToNow } from 'date-fns';

export default (date: string | Date) => {
  const timeAgo = formatDistanceToNow(new Date(date));
  return `${timeAgo} ago`;
};
