import { defineAsyncComponent } from 'vue';
import LoadingSpinner from '@components/LoadingSpinner.vue';

export const DateTimeWeatherComboAsync = defineAsyncComponent({
  loader: () => import('@/components/DateTimeWeatherCombo.vue'),
  loadingComponent: LoadingSpinner,
});
