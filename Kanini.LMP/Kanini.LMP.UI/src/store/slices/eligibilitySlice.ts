import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { eligibilityAPI, type EligibilityResponse } from '../../services/api/eligibilityAPI';

interface EligibilityState {
  score: EligibilityResponse | null;
  loading: boolean;
  error: string | null;
  lastFetched: number | null;
}

const initialState: EligibilityState = {
  score: null,
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



const eligibilitySlice = createSlice({
  name: 'eligibility',
  initialState,
  reducers: {
    clearEligibility: (state) => {
      state.score = null;
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
        // Don't clear score to prevent UI flicker
      })
      .addCase(calculateEligibility.fulfilled, (state, action) => {
        state.loading = false;
        state.score = action.payload;
      })
      .addCase(calculateEligibility.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to calculate eligibility';
      });
  },
});

export const { clearEligibility } = eligibilitySlice.actions;
export default eligibilitySlice.reducer;
