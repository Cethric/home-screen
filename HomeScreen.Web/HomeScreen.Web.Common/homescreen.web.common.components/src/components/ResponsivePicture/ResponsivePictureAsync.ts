import { defineAsyncComponent } from 'vue';
import LoadingSpinner from '@/components/LoadingSpinner/LoadingSpinner.vue';

export const ResponsivePictureAsync = defineAsyncComponent({
  loader: () => import('@/components/ResponsivePicture/ResponsivePicture.vue'),
  loadingComponent: LoadingSpinner,
  timeout: 0,
});
