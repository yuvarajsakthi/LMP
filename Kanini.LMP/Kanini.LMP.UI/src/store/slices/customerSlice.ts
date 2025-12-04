import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { customerAPI, type Customer } from '../../services/api/customerAPI';

interface CustomerState {
  currentCustomer: Customer | null;
  loading: boolean;
  error: string | null;
}

const initialState: CustomerState = {
  currentCustomer: null,
  loading: false,
  error: null,
};

export const fetchCustomerByUserId = createAsyncThunk(
  'customer/fetchByUserId',
  async (userId: number) => {
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
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomerByUserId.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCustomerByUserId.fulfilled, (state, action) => {
        state.loading = false;
        state.currentCustomer = action.payload;
      })
      .addCase(fetchCustomerByUserId.rejected, (state, action) => {
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
      });
  },
});

export const { clearCustomer } = customerSlice.actions;
export default customerSlice.reducer;
