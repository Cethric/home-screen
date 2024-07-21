import { defineAsyncComponent } from 'vue';
import { LoadingSpinner } from '@homescreen/web-components-client/src/index';

export const DateTimeWeatherComboAsync = defineAsyncComponent({
  loader: () => import('@/components/DateTimeWeatherCombo.vue'),
  loadingComponent: LoadingSpinner,
});
