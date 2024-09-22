import { createRouter, createWebHistory } from 'vue-router';

export const useRoutes = () => {
  const router = createRouter({
    history: createWebHistory(),
    routes: [
      {
        name: 'Dashboard',
        path: '/',
        component: () => import('./DashboardRoute.vue'),
      },
      {
        name: 'Gallery',
        path: '/gallery',
        component: () => import('./GalleryRoute.vue'),
      },
    ],
  });

  return router;
};
