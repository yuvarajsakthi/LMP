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

export const updateLoanStatus = createAsyncThunk(
  'loanApplication/updateStatus',
  async ({ loanId, status }: { loanId: number; status: number }) => {
    return await loanApplicationAPI.updateLoanStatus(loanId, status);
  }
);

export const withdrawLoan = createAsyncThunk(
  'loanApplication/withdraw',
  async (loanId: number) => {
    await loanApplicationAPI.withdrawLoan(loanId);
    return loanId;
  }
);

export const fetchLoanCategories = createAsyncThunk(
  'loanApplication/fetchCategories',
  async () => {
    return await loanApplicationAPI.getLoanCategories();
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
      })
      .addCase(updateLoanStatus.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateLoanStatus.fulfilled, (state, action) => {
        state.loading = false;
        if (state.currentApplication?.loanId === action.payload.loanId) {
          state.currentApplication.status = action.payload.status;
        }
      })
      .addCase(updateLoanStatus.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to update loan status';
      })
      .addCase(withdrawLoan.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(withdrawLoan.fulfilled, (state, action) => {
        state.loading = false;
        state.applications = state.applications.filter(app => app.loanId !== action.payload);
      })
      .addCase(withdrawLoan.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to withdraw loan';
      })
      .addCase(fetchLoanCategories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchLoanCategories.fulfilled, (state, action) => {
        state.loading = false;
        state.loanCategories = action.payload;
      })
      .addCase(fetchLoanCategories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch loan categories';
      });
  },
});

export const { clearLoanApplications } = loanApplicationSlice.actions;
export default loanApplicationSlice.reducer;
