import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { authAPI } from '../../services/api/authAPI';
import { secureStorage } from '../../utils/secureStorage';
import type { LoginCredentials, RegisterCredentials, DecodedToken } from '../../types';

interface AuthState {
  user: DecodedToken | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  user: null,
  token: null,
  isLoading: false,
  error: null,
};

export const loginUser = createAsyncThunk(
  'auth/login',
  async (credentials: LoginCredentials, { rejectWithValue }) => {
    try {
      const response = await authAPI.login(credentials);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || 'Login failed',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const registerUser = createAsyncThunk(
  'auth/register',
  async (credentials: RegisterCredentials, { rejectWithValue }) => {
    try {
      const response = await authAPI.register(credentials);
      return response;
    } catch (error: any) {
      // Error is already handled by ApiService and shown to user
      return rejectWithValue({
        message: error.message || 'Registration failed',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const sendOTP = createAsyncThunk(
  'auth/sendOTP',
  async (data: { email: string; phoneNumber?: string; purpose: 'LOGIN' | 'REGISTRATION' | 'PASSWORD_RESET' }, { rejectWithValue }) => {
    try {
      const response = await authAPI.sendOTP(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || 'Failed to send OTP',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const verifyOTP = createAsyncThunk(
  'auth/verifyOTP',
  async (data: { userId: number; otp: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.verifyOTP(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || 'OTP verification failed',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const verifyLoginOTP = createAsyncThunk(
  'auth/verifyLoginOTP',
  async (data: { userId: number; otp: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.verifyLoginOTP(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || 'Login OTP verification failed',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const resetPassword = createAsyncThunk(
  'auth/resetPassword',
  async (data: { userId: number; otp: string; newPassword: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.resetPassword(data);
      return response.message || 'Password reset successfully';
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || 'Failed to reset password',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.error = null;
      secureStorage.removeToken();
    },
    clearError: (state) => {
      state.error = null;
    },
    setToken: (state, action) => {
      state.token = action.payload.token;
      state.user = action.payload.user;
      // Store token in localStorage when setting via reducer
      if (action.payload.token) {
        secureStorage.setToken(action.payload.token);
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loginUser.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(loginUser.fulfilled, (state, action) => {
        state.isLoading = false;
        if (action.payload.requiresVerification) {
          // Don't set token/user for unverified accounts
          state.error = null;
        } else {
          state.token = action.payload.token;
          state.user = action.payload.user;
          state.error = null;
          // Store token in localStorage
          if (action.payload.token) {
            secureStorage.setToken(action.payload.token);
          }
        }
      })
      .addCase(loginUser.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Login failed';
      })
      .addCase(registerUser.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(registerUser.fulfilled, (state) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Registration failed';
      })
      .addCase(sendOTP.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(sendOTP.fulfilled, (state) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(sendOTP.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Failed to send OTP';
      })
      .addCase(verifyOTP.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(verifyOTP.fulfilled, (state, action) => {
        state.isLoading = false;
        if (action.payload.token && action.payload.user) {
          state.token = action.payload.token;
          state.user = action.payload.user;
          // Store token in localStorage
          secureStorage.setToken(action.payload.token);
        }
        state.error = null;
      })
      .addCase(verifyOTP.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'OTP verification failed';
      })
      .addCase(verifyLoginOTP.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(verifyLoginOTP.fulfilled, (state, action) => {
        state.isLoading = false;
        state.token = action.payload.token;
        state.user = action.payload.user;
        state.error = null;
        // Store token in localStorage
        if (action.payload.token) {
          secureStorage.setToken(action.payload.token);
        }
      })
      .addCase(verifyLoginOTP.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Login OTP verification failed';
      })
      .addCase(resetPassword.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(resetPassword.fulfilled, (state) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(resetPassword.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Failed to reset password';
      });
  },
});

export const { logout, clearError, setToken } = authSlice.actions;
export default authSlice.reducer;