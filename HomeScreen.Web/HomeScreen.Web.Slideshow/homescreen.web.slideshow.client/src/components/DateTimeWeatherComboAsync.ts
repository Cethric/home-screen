import { LoadingSpinner } from '@homescreen/web-common-components';
import { defineAsyncComponent } from 'vue';

export const DateTimeWeatherComboAsync = defineAsyncComponent({
  loader: () => import('@/components/DateTimeWeatherCombo.vue'),
  loadingComponent: LoadingSpinner,
});
