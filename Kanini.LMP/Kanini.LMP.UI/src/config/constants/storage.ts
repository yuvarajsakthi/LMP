export const STORAGE_KEYS = {
  ACCESS_TOKEN: import.meta.env.VITE_ACCESS_TOKEN_KEY || 'at_' + btoa('access').slice(0, 8),
  REFRESH_TOKEN: import.meta.env.VITE_REFRESH_TOKEN_KEY || 'rt_' + btoa('refresh').slice(0, 8),
} as const;