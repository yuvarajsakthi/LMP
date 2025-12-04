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
      if (error.requiresVerification) {
        return { requiresVerification: true, email: error.email, message: error.message };
      }
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
      return rejectWithValue({
        message: error.message || 'Registration failed',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

export const resetPassword = createAsyncThunk(
  'auth/resetPassword',
  async (data: { email: string; otp: string; newPassword: string }, { rejectWithValue }) => {
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
        if ('requiresVerification' in action.payload && action.payload.requiresVerification) {
          state.error = null;
          return;
        }
        if ('token' in action.payload) {
          state.token = action.payload.token;
          state.user = action.payload.user;
          state.error = null;
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
      .addCase(registerUser.fulfilled, (state, action) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(registerUser.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || 'Registration failed';
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