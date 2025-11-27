import axios, { type AxiosInstance } from "axios";
import { requestInterceptor, requestErrorHandler } from "./Interceptors/requestInterceptor";
import { responseInterceptor, responseErrorHandler } from "./Interceptors/responseInterceptor";

const BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5156/";

let axiosInstance: AxiosInstance;
try {
  axiosInstance = axios.create({
    baseURL: BASE_URL,
    headers: {
      "Content-Type": "application/json",
    },
    timeout: 10000,
  });

  axiosInstance.interceptors.request.use(requestInterceptor, requestErrorHandler);
  axiosInstance.interceptors.response.use(responseInterceptor, responseErrorHandler);
} catch (error) {
  console.error('Failed to create axios instance:', error);
  axiosInstance = axios.create({ baseURL: BASE_URL });
}

export default axiosInstance;
