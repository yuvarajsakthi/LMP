import { configureStore } from '@reduxjs/toolkit';
import authReducer, { setToken } from './slices/authSlice';
import { authMiddleware } from '../middleware';

export const store = configureStore({
  reducer: {
    auth: authReducer,
  },
});

// Initialize store with persisted auth state
const initializeStore = () => {
  const token = authMiddleware.getToken();
  const decodedToken = authMiddleware.getDecodedToken();
  
  if (token && decodedToken && authMiddleware.isAuthenticated()) {
    store.dispatch(setToken({ token, user: decodedToken }));
  }
};

// Initialize on store creation
initializeStore();

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;