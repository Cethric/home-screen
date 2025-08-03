type MaybeAsyncIterable = { [Symbol.asyncIterator]?: unknown };

export function isAsyncIterable<T extends MaybeAsyncIterable>(input?: T): boolean {
  if (input === null) {
    return false;
  }
  if (input === undefined) {
    return false;
  }

  return typeof input[Symbol.asyncIterator] === 'function';
}
