export enum Slideshows {
  'polaroid_slideshow' = 'polaroid_slideshow',
  'fullscreen_slideshow' = 'fullscreen_slideshow',
  'rolling_slideshow' = 'rolling_slideshow',
  'grid_slideshow' = 'grid_slideshow',
}

export type Slideshow = keyof typeof Slideshows;
