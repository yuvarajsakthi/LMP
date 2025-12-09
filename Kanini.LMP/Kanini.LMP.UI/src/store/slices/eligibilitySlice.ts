import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { eligibilityAPI, type EligibilityProfileRequest, type EligibilityScore } from '../../services/api/eligibilityAPI';

interface EligibilityState {
  score: EligibilityScore | null;
  checkResult: any | null;
  loading: boolean;
  error: string | null;
  lastFetched: number | null;
}

const initialState: EligibilityState = {
  score: null,
  checkResult: null,
  loading: false,
  error: null,
  lastFetched: null,
};

const CACHE_DURATION = 5 * 60 * 1000;

export const getEligibilityScore = createAsyncThunk(
  'eligibility/getScore',
  async (customerId: number, { getState }) => {
    const state = getState() as any;
    const { lastFetched, score } = state.eligibility;
    
    if (lastFetched && Date.now() - lastFetched < CACHE_DURATION && score) {
      return score;
    }
    
    return await eligibilityAPI.getEligibilityScore(customerId);
  }
);

export const calculateEligibility = createAsyncThunk(
  'eligibility/calculate',
  async (customerId: number) => {
    return await eligibilityAPI.calculateEligibility(customerId);
  }
);

export const updateEligibilityProfile = createAsyncThunk(
  'eligibility/updateProfile',
  async ({ customerId, request }: { customerId: number; request: EligibilityProfileRequest }) => {
    return await eligibilityAPI.updateEligibilityProfile(customerId, request);
  }
);

export const checkEligibility = createAsyncThunk(
  'eligibility/check',
  async (request: EligibilityProfileRequest) => {
    return await eligibilityAPI.checkEligibility(request);
  }
);

const eligibilitySlice = createSlice({
  name: 'eligibility',
  initialState,
  reducers: {
    clearEligibility: (state) => {
      state.score = null;
      state.checkResult = null;
      state.error = null;
      state.lastFetched = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getEligibilityScore.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getEligibilityScore.fulfilled, (state, action) => {
        state.loading = false;
        state.score = action.payload;
        state.lastFetched = Date.now();
      })
      .addCase(getEligibilityScore.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to get eligibility score';
      })
      .addCase(calculateEligibility.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(calculateEligibility.fulfilled, (state, action) => {
        state.loading = false;
        state.score = action.payload;
      })
      .addCase(calculateEligibility.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to calculate eligibility';
      })
      .addCase(checkEligibility.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(checkEligibility.fulfilled, (state, action) => {
        state.loading = false;
        state.checkResult = action.payload;
      })
      .addCase(checkEligibility.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to check eligibility';
      });
  },
});

export const { clearEligibility } = eligibilitySlice.actions;
export default eligibilitySlice.reducer;
