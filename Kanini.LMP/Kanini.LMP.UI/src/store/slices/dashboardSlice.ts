import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customerDashboardAPI } from '../../services/api/customerDashboardAPI';

interface DashboardState {
  loanProducts: any[];
  recentLoans: any[];
  eligibilityScore: any;
  applicationStatus: any[];
  isLoading: boolean;
  lastFetched: number | null;
  lastFetchedApplicationStatus: number | null;
}

const initialState: DashboardState = {
  loanProducts: [],
  recentLoans: [],
  eligibilityScore: null,
  applicationStatus: [],
  isLoading: false,
  lastFetched: null,
  lastFetchedApplicationStatus: null,
};

const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

export const fetchLoanProducts = createAsyncThunk(
  'dashboard/fetchLoanProducts',
  async (_, { getState }) => {
    const state = getState() as any;
    const { lastFetched, loanProducts } = state.dashboard;
    
    if (lastFetched && Date.now() - lastFetched < CACHE_DURATION && loanProducts.length > 0) {
      return loanProducts;
    }
    
    return await customerDashboardAPI.getLoanProducts();
  }
);

export const fetchRecentLoans = createAsyncThunk(
  'dashboard/fetchRecentLoans',
  async () => await customerDashboardAPI.getRecentAppliedLoans()
);

export const fetchEligibilityScore = createAsyncThunk(
  'dashboard/fetchEligibilityScore',
  async (customerId: number) => await customerDashboardAPI.getEligibilityScore(customerId)
);

export const fetchApplicationStatus = createAsyncThunk(
  'dashboard/fetchApplicationStatus',
  async (_, { getState }) => {
    const state = getState() as any;
    const { lastFetchedApplicationStatus, applicationStatus } = state.dashboard;
    
    if (lastFetchedApplicationStatus && Date.now() - lastFetchedApplicationStatus < CACHE_DURATION && applicationStatus.length > 0) {
      return applicationStatus;
    }
    
    return await customerDashboardAPI.getApplicationStatus();
  }
);

const dashboardSlice = createSlice({
  name: 'dashboard',
  initialState,
  reducers: {
    clearDashboardCache: (state) => {
      state.lastFetched = null;
      state.lastFetchedApplicationStatus = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchLoanProducts.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(fetchLoanProducts.fulfilled, (state, action) => {
        state.loanProducts = action.payload;
        state.lastFetched = Date.now();
        state.isLoading = false;
      })
      .addCase(fetchLoanProducts.rejected, (state) => {
        state.isLoading = false;
      })
      .addCase(fetchRecentLoans.fulfilled, (state, action) => {
        state.recentLoans = action.payload;
      })
      .addCase(fetchEligibilityScore.fulfilled, (state, action) => {
        state.eligibilityScore = action.payload;
      })
      .addCase(fetchApplicationStatus.fulfilled, (state, action) => {
        state.applicationStatus = action.payload;
        state.lastFetchedApplicationStatus = Date.now();
      });
  },
});

export const { clearDashboardCache } = dashboardSlice.actions;
export default dashboardSlice.reducer;
