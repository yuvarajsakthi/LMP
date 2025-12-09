import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { loanApplicationAPI } from '../../services/api/loanApplicationAPI';

interface LoanApplicationState {
  applications: any[];
  currentApplication: any | null;
  loanCategories: any[];
  loading: boolean;
  error: string | null;
}

const initialState: LoanApplicationState = {
  applications: [],
  currentApplication: null,
  loanCategories: [],
  loading: false,
  error: null,
};

export const createPersonalLoan = createAsyncThunk(
  'loanApplication/createPersonal',
  async ({ customerId, data }: { customerId: number; data: any }) => {
    return await loanApplicationAPI.createPersonalLoan(customerId, data);
  }
);

export const createHomeLoan = createAsyncThunk(
  'loanApplication/createHome',
  async ({ customerId, data }: { customerId: number; data: any }) => {
    return await loanApplicationAPI.createHomeLoan(customerId, data);
  }
);

export const createVehicleLoan = createAsyncThunk(
  'loanApplication/createVehicle',
  async ({ customerId, data }: { customerId: number; data: any }) => {
    return await loanApplicationAPI.createVehicleLoan(customerId, data);
  }
);

const loanApplicationSlice = createSlice({
  name: 'loanApplication',
  initialState,
  reducers: {
    clearLoanApplications: (state) => {
      state.applications = [];
      state.currentApplication = null;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(createPersonalLoan.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createPersonalLoan.fulfilled, (state, action) => {
        state.loading = false;
        state.currentApplication = action.payload;
      })
      .addCase(createPersonalLoan.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create personal loan';
      })
      .addCase(createHomeLoan.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createHomeLoan.fulfilled, (state, action) => {
        state.loading = false;
        state.currentApplication = action.payload;
      })
      .addCase(createHomeLoan.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create home loan';
      })
      .addCase(createVehicleLoan.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createVehicleLoan.fulfilled, (state, action) => {
        state.loading = false;
        state.currentApplication = action.payload;
      })
      .addCase(createVehicleLoan.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create vehicle loan';
      });
  },
});

export const { clearLoanApplications } = loanApplicationSlice.actions;
export default loanApplicationSlice.reducer;
