export function random(): number {
  return Math.random();
}

export function rangeRNG(min: number, max: number, random: () => number): number {
  return Math.trunc(random() * (max - min) + min);
}

export function range(min: number, max: number): number {
  return rangeRNG(min, max, random);
}

export function choiceRNG<T>(options: T[], random: () => number): T {
  return options[rangeRNG(0, options.length, random)];
}

export function choice<T>(options: T[]): T {
  return choiceRNG(options, random);
}
