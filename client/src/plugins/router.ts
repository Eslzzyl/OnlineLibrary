import { createRouter, createWebHashHistory } from 'vue-router'

import Greeting from '@/components/Greeting/Greeting.vue';
import User from '@/components/User/User.vue';
import Admin from '@/components/Admin/Admin.vue';

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/',
      name: 'Greeting',
      component: Greeting,
      meta: {
        title: '登录',
      },
    },
    {
      path: '/user',
      component: User,
      meta: {
        title: '用户页',
      }
    },
    {
      path: '/admin',
      component: Admin,
      meta: {
        title: '管理页',
      }
    },
  ]
})

// 添加全局前置守卫
router.beforeEach((to) => {
  // 如果路由配置中没有定义标题，则使用默认标题
  const destTitle = to.meta.title as string;
  document.title = destTitle || '网上图书馆';

  const token = localStorage.getItem('token');
  if (to.name !== 'Greeting' && token === null) {
    return { name: 'Greeting' };
  }
});

export default router