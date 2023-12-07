//导入axios
import axios from 'axios'

//使用axios下面的create([config])方法创建axios实例，其中config参数为axios最基本的配置信息。
const axiosInstance = axios.create({
  baseURL: 'http://localhost:5057', //请求后端数据的基本地址，自定义
  timeout: 5000                     //请求超时设置，单位ms
})

// 添加请求拦截器
axiosInstance.interceptors.request.use(config => {
  const token = window.localStorage.getItem('token') // 从 localStorage 中获取 token
  if (token != '') {
    config.headers['token'] = token // 将 token 添加到请求头中
  }
  return config
}, error => {
  return Promise.reject(error)
})

export default axiosInstance
