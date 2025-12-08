import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { authAPI } from '../../services/api/authAPI';
import { secureStorage } from '../../utils/secureStorage';
import { ERROR_MESSAGES, SUCCESS_MESSAGES } from '../../config/constants/messages';
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

// Endpoint: API_ENDPOINTS.USER_LOGIN
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
        message: error.message || 'Invalid credentials',
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

// Endpoint: API_ENDPOINTS.USER_REGISTER
export const registerUser = createAsyncThunk(
  'auth/register',
  async (credentials: RegisterCredentials, { rejectWithValue }) => {
    try {
      const response = await authAPI.register(credentials);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || ERROR_MESSAGES.REGISTRATION_FAILED,
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

// Endpoint: API_ENDPOINTS.SEND_OTP
export const sendOTP = createAsyncThunk(
  'auth/sendOTP',
  async (data: { email: string; purpose: 'LOGIN' | 'REGISTER' | 'FORGETPASSWORD' }, { rejectWithValue }) => {
    try {
      const response = await authAPI.sendOTP(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || ERROR_MESSAGES.OTP_SEND_FAILED,
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

// Endpoint: API_ENDPOINTS.VERIFY_OTP
export const verifyOTP = createAsyncThunk(
  'auth/verifyOTP',
  async (data: { email: string; otp: string; purpose: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.verifyOTP(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || ERROR_MESSAGES.OTP_VERIFY_FAILED,
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

// Endpoint: API_ENDPOINTS.FORGOT_PASSWORD
export const forgotPassword = createAsyncThunk(
  'auth/forgotPassword',
  async (data: { email: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.forgotPassword(data);
      return response;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || ERROR_MESSAGES.OTP_SEND_FAILED,
        statusCode: error.statusCode || 500,
        errors: error.errors || []
      });
    }
  }
);

// Endpoint: API_ENDPOINTS.RESET_PASSWORD
export const resetPassword = createAsyncThunk(
  'auth/resetPassword',
  async (data: { email: string; otp: string; newPassword: string }, { rejectWithValue }) => {
    try {
      const response = await authAPI.resetPassword(data);
      return response.message || SUCCESS_MESSAGES.PASSWORD_UPDATED;
    } catch (error: any) {
      return rejectWithValue({
        message: error.message || ERROR_MESSAGES.PASSWORD_UPDATE_FAILED,
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
        state.error = error?.message || 'Invalid credentials';
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
        state.error = error?.message || ERROR_MESSAGES.REGISTRATION_FAILED;
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
        state.error = error?.message || ERROR_MESSAGES.PASSWORD_UPDATE_FAILED;
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
        state.error = error?.message || ERROR_MESSAGES.OTP_SEND_FAILED;
      })
      .addCase(verifyOTP.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(verifyOTP.fulfilled, (state) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(verifyOTP.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || ERROR_MESSAGES.OTP_VERIFY_FAILED;
      })
      .addCase(forgotPassword.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(forgotPassword.fulfilled, (state) => {
        state.isLoading = false;
        state.error = null;
      })
      .addCase(forgotPassword.rejected, (state, action) => {
        state.isLoading = false;
        const error = action.payload as any;
        state.error = error?.message || ERROR_MESSAGES.OTP_SEND_FAILED;
      });
  },
});

export const { logout, clearError, setToken } = authSlice.actions;
export default authSlice.reducer;