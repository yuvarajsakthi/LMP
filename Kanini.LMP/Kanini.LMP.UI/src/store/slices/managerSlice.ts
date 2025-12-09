import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { managerDashboardAPI } from '../../services/api/managerDashboardAPI';
import type { LoanApplicationDetail } from '../../types/managerDashboard';

interface ManagerState {
  loans: LoanApplicationDetail[];
  selectedLoan: LoanApplicationDetail | null;
  loading: boolean;
  error: string | null;
}

const initialState: ManagerState = {
  loans: [],
  selectedLoan: null,
  loading: false,
  error: null,
};

export const fetchAllLoans = createAsyncThunk('manager/fetchAllLoans', async () => {
  return await managerDashboardAPI.getAllLoans();
});

export const fetchLoanById = createAsyncThunk('manager/fetchLoanById', async (id: number) => {
  return await managerDashboardAPI.getLoanById(id);
});

export const updateLoanStatus = createAsyncThunk(
  'manager/updateLoanStatus',
  async (data: { loanApplicationBaseId: number; status: string; rejectionReason?: string }) => {
    await managerDashboardAPI.updateLoanStatus(data);
    return data.loanApplicationBaseId;
  }
);

export const disburseLoan = createAsyncThunk('manager/disburseLoan', async (id: number) => {
  await managerDashboardAPI.disburseLoan(id);
  return id;
});

const managerSlice = createSlice({
  name: 'manager',
  initialState,
  reducers: {
    clearSelectedLoan: (state) => {
      state.selectedLoan = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllLoans.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllLoans.fulfilled, (state, action) => {
        state.loading = false;
        state.loans = action.payload;
      })
      .addCase(fetchAllLoans.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch loans';
      })
      .addCase(fetchLoanById.fulfilled, (state, action) => {
        state.selectedLoan = action.payload;
      })
      .addCase(updateLoanStatus.fulfilled, (state) => {
        state.selectedLoan = null;
      })
      .addCase(disburseLoan.fulfilled, (state) => {
        state.selectedLoan = null;
      });
  },
});

export const { clearSelectedLoan } = managerSlice.actions;
export default managerSlice.reducer;
