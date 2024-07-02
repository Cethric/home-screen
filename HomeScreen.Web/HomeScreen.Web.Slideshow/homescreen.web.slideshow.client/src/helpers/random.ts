export function random(): number {
  return Math.random();
}

export function _range(min: number, max: number, random: () => number): number {
  return Math.trunc(random() * (max - min) + min);
}

export function range(min: number, max: number): number {
  return _range(min, max, random);
}

export function _choice<T>(options: T[], random: () => number): T {
  return options[_range(0, options.length, random)];
}

export function choice<T>(options: T[]): T {
  return _choice(options, random);
}
