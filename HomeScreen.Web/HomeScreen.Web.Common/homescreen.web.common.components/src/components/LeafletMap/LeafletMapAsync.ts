import { defineAsyncComponent } from "vue";
import LoadingSpinner from "@/components/LoadingSpinner/LoadingSpinner.vue";

export const LeafletMapAsync = defineAsyncComponent({
	loader: () => import("./LeafletMap.vue"),
	loadingComponent: LoadingSpinner,
});
