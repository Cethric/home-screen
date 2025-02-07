import { defineAsyncComponent } from 'vue';
import { LoadingSpinner } from '@homescreen/web-common-components';

export const DateTimeWeatherComboAsync = defineAsyncComponent({
    loader: () => import('@/components/DateTimeWeatherCombo.vue'),
    loadingComponent: LoadingSpinner,
});
