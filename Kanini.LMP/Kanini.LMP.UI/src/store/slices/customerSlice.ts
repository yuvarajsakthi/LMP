import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customerAPI, type Customer } from '../../services/api/customerAPI';

interface CustomerState {
  currentCustomer: Customer | null;
  loading: boolean;
  error: string | null;
  lastFetched: number | null;
}

const CACHE_DURATION = 5 * 60 * 1000;

const initialState: CustomerState = {
  currentCustomer: null,
  loading: false,
  error: null,
  lastFetched: null,
};

export const fetchCustomerById = createAsyncThunk(
  'customer/fetchById',
  async (customerId: number, { getState }) => {
    const state = getState() as any;
    const { lastFetched, currentCustomer } = state.customer;
    
    if (lastFetched && Date.now() - lastFetched < CACHE_DURATION && currentCustomer?.customerId === customerId) {
      return currentCustomer;
    }
    
    return await customerAPI.getCustomerById(customerId);
  }
);

export const fetchCustomerByUserId = createAsyncThunk(
  'customer/fetchByUserId',
  async (userId: number, { getState }) => {
    console.log('📦 fetchCustomerByUserId called with userId:', userId);
    const state = getState() as any;
    const { lastFetched, currentCustomer } = state.customer;
    
    console.log('📊 Cache check - lastFetched:', lastFetched, 'currentCustomer:', currentCustomer);
    
    if (lastFetched && Date.now() - lastFetched < CACHE_DURATION && currentCustomer?.userId === userId) {
      console.log('✅ Returning cached customer data');
      return currentCustomer;
    }
    
    console.log('🌐 Fetching fresh customer data from API');
    return await customerAPI.getCustomerByUserId(userId);
  }
);

export const updateCustomerProfile = createAsyncThunk(
  'customer/updateProfile',
  async ({ customerId, data }: { customerId: number; data: Partial<Customer> }) => {
    return await customerAPI.updateCustomer(customerId, data);
  }
);

const customerSlice = createSlice({
  name: 'customer',
  initialState,
  reducers: {
    clearCustomer: (state) => {
      state.currentCustomer = null;
      state.error = null;
      state.lastFetched = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomerById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCustomerById.fulfilled, (state, action) => {
        state.loading = false;
        state.currentCustomer = action.payload;
        state.lastFetched = Date.now();
      })
      .addCase(fetchCustomerById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch customer';
      })
      .addCase(updateCustomerProfile.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateCustomerProfile.fulfilled, (state, action) => {
        state.loading = false;
        state.currentCustomer = action.payload;
      })
      .addCase(updateCustomerProfile.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to update customer';
      })
      .addCase(fetchCustomerByUserId.pending, (state) => {
        console.log('⏳ fetchCustomerByUserId - PENDING');
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCustomerByUserId.fulfilled, (state, action) => {
        console.log('✅ fetchCustomerByUserId - FULFILLED:', action.payload);
        state.loading = false;
        state.currentCustomer = action.payload;
        state.lastFetched = Date.now();
      })
      .addCase(fetchCustomerByUserId.rejected, (state, action) => {
        console.error('❌ fetchCustomerByUserId - REJECTED:', action.error);
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch customer';
      });
  },
});

export const { clearCustomer } = customerSlice.actions;
export default customerSlice.reducer;
