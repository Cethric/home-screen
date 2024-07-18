import { defineAsyncComponent } from 'vue';
import LoadingSpinner from './LoadingSpinner.vue';

export const ResponsiveImageSuspenseAsync = defineAsyncComponent({
  loader: () => import('./ResponsiveImageSuspense.vue'),
  loadingComponent: LoadingSpinner,
  timeout: 0,
});
