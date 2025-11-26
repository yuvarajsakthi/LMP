import axios from "axios";
import { requestInterceptor, requestErrorHandler, responseInterceptor, responseErrorHandler } from ".";

const BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api";

const axiosInstance = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 10000,
});

axiosInstance.interceptors.request.use(requestInterceptor, requestErrorHandler);
axiosInstance.interceptors.response.use(responseInterceptor, responseErrorHandler);

export default axiosInstance;
