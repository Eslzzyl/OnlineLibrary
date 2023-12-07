import { createRouter, createWebHashHistory } from 'vue-router'

import Greeting from '@/components/Greeting.vue';
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
router.beforeEach((to, from, next) => {
  // 如果路由配置中没有定义标题，则使用默认标题
  const destTitle = to.meta.title as string
  document.title = destTitle || '网上图书馆'

  // if (to.path !== '/') {
  //   if (to.path === '/register') {
  //     next()
  //   } else {
  //     const token = localStorage.getItem('token')
  //     if (token === null || token === '') {
  //       next('/')
  //     } else {
  //       const userType = localStorage.getItem('type')
  //       if (userType === 'user') {
  //         if (to.path === '/manager' || to.path === '/system') {
  //           next('/user')
  //         } else {
  //           next()
  //         }
  //       } else {
  //         if (to.path === '/user') {
  //           next('/manager')
  //         } else {
  //           next()
  //         }
  //       }
  //     }
  //   }
  // }
  next()
});

export default router