export function random(): number {
  return Math.random();
}

export function rangeRNG(min: number, max: number, random: () => number): number {
  return Math.trunc(random() * (max - min) + min);
}

export function range(min: number, max: number): number {
  return rangeRNG(min, max, random);
}

export function choiceRNG<T, TArr extends T[] = T[]>(options: TArr, random: () => number): TArr[number] {
  return options[rangeRNG(0, options.length, random)] as TArr[number];
}

export function choice<T>(options: T[]): T {
  return choiceRNG(options, random);
}
