export function notNull<T>(
  element: T | undefined | null,
  name?: string,
): NonNullable<T> {
  if (element === undefined || element === null) {
    if (name) {
      throw `${name} was expected to not be null.`
    } else {
      throw "Element was expected to not be null."
    }
  }
  return element
}

export function intersperseWith<T>(
  source: T[],
  newElementCreator: (index: number) => T,
) {
  return source.reduce((prev, curr, index) => {
    if (prev.length > 0) {
      prev.push(newElementCreator(index))
    }
    prev.push(curr)
    return prev
  }, [] as T[])
}
