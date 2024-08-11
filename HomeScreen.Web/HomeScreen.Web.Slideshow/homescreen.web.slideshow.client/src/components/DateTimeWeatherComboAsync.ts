import { defineAsyncComponent } from 'vue';
import { LoadingSpinner } from '@homescreen/web-components-client';

export const DateTimeWeatherComboAsync = defineAsyncComponent({
  loader: () => import('@/components/DateTimeWeatherCombo.vue'),
  loadingComponent: LoadingSpinner,
});
