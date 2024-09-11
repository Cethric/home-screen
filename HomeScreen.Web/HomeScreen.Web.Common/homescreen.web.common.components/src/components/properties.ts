export enum Variants {
    primary = 'primary',
    secondary = 'secondary',
}

export type Variant = keyof typeof Variants;

export enum Directions {
    horizontal = 'horizontal',
    vertical = 'vertical',
    random = 'random',
}

export type Direction = keyof typeof Directions;