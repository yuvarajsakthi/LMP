import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { notificationAPI, type Notification } from '../../services/api/notificationAPI';

interface NotificationState {
  notifications: Notification[];
  unreadCount: number;
  loading: boolean;
  error: string | null;
  lastFetched: number | null;
}

const CACHE_DURATION = 2 * 60 * 1000;

const initialState: NotificationState = {
  notifications: [],
  unreadCount: 0,
  loading: false,
  error: null,
  lastFetched: null,
};

export const getAllNotifications = createAsyncThunk(
  'notification/getAll',
  async (_, { getState }) => {
    const state = getState() as any;
    const { lastFetched, notifications } = state.notification;
    
    if (lastFetched && Date.now() - lastFetched < CACHE_DURATION && notifications.length > 0) {
      return notifications;
    }
    
    return await notificationAPI.getAllNotifications();
  }
);

export const deleteNotification = createAsyncThunk(
  'notification/delete',
  async (notificationId: number) => {
    await notificationAPI.deleteNotification(notificationId);
    return notificationId;
  }
);

const notificationSlice = createSlice({
  name: 'notification',
  initialState,
  reducers: {
    clearNotifications: (state) => {
      state.notifications = [];
      state.unreadCount = 0;
      state.error = null;
      state.lastFetched = null;
    },
    markAsRead: (state, action) => {
      const notification = state.notifications.find(n => n.notificationId === action.payload);
      if (notification && !notification.isRead) {
        notification.isRead = true;
        state.unreadCount = Math.max(0, state.unreadCount - 1);
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getAllNotifications.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getAllNotifications.fulfilled, (state, action) => {
        state.loading = false;
        state.notifications = action.payload;
        state.unreadCount = action.payload.filter(n => !n.isRead).length;
        state.lastFetched = Date.now();
      })
      .addCase(getAllNotifications.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch notifications';
      })
      .addCase(deleteNotification.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteNotification.fulfilled, (state, action) => {
        state.loading = false;
        const notification = state.notifications.find(n => n.notificationId === action.payload);
        if (notification && !notification.isRead) {
          state.unreadCount = Math.max(0, state.unreadCount - 1);
        }
        state.notifications = state.notifications.filter(n => n.notificationId !== action.payload);
      })
      .addCase(deleteNotification.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to delete notification';
      });
  },
});

export const { clearNotifications, markAsRead } = notificationSlice.actions;
export default notificationSlice.reducer;
