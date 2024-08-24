import { defineAsyncComponent } from 'vue';
import LoadingSpinner from '../LoadingSpinner/LoadingSpinner.vue';

export const ResponsiveImageSuspenseAsync = defineAsyncComponent({
  loader: () => import('./ResponsiveImage.vue'),
  loadingComponent: LoadingSpinner,
  timeout: 0,
});
