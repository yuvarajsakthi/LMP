import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { faqAPI, type FaqDTO } from '../../services/api/faqAPI';

interface FaqState {
  faqs: FaqDTO[];
  currentFaq: FaqDTO | null;
  loading: boolean;
  error: string | null;
}

const initialState: FaqState = {
  faqs: [],
  currentFaq: null,
  loading: false,
  error: null,
};

export const createFaq = createAsyncThunk(
  'faq/create',
  async (faq: FaqDTO) => {
    return await faqAPI.createFaq(faq);
  }
);

export const getAllFaqs = createAsyncThunk(
  'faq/getAll',
  async () => {
    return await faqAPI.getAllFaqs();
  }
);

export const getFaqById = createAsyncThunk(
  'faq/getById',
  async (id: number) => {
    return await faqAPI.getFaqById(id);
  }
);

export const getFaqsByCustomerId = createAsyncThunk(
  'faq/getByCustomer',
  async (customerId: number) => {
    return await faqAPI.getFaqsByCustomerId(customerId);
  }
);

export const updateFaq = createAsyncThunk(
  'faq/update',
  async ({ id, faq }: { id: number; faq: FaqDTO }) => {
    return await faqAPI.updateFaq(id, faq);
  }
);

export const deleteFaq = createAsyncThunk(
  'faq/delete',
  async (id: number) => {
    await faqAPI.deleteFaq(id);
    return id;
  }
);

const faqSlice = createSlice({
  name: 'faq',
  initialState,
  reducers: {
    clearFaqs: (state) => {
      state.faqs = [];
      state.currentFaq = null;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(createFaq.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createFaq.fulfilled, (state, action) => {
        state.loading = false;
        state.faqs.push(action.payload);
      })
      .addCase(createFaq.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create FAQ';
      })
      .addCase(getAllFaqs.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getAllFaqs.fulfilled, (state, action) => {
        state.loading = false;
        state.faqs = action.payload;
      })
      .addCase(getAllFaqs.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch FAQs';
      })
      .addCase(getFaqById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getFaqById.fulfilled, (state, action) => {
        state.loading = false;
        state.currentFaq = action.payload;
      })
      .addCase(getFaqById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch FAQ';
      })
      .addCase(getFaqsByCustomerId.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(getFaqsByCustomerId.fulfilled, (state, action) => {
        state.loading = false;
        state.faqs = action.payload;
      })
      .addCase(getFaqsByCustomerId.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch customer FAQs';
      })
      .addCase(updateFaq.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateFaq.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.faqs.findIndex(f => f.id === action.payload.id);
        if (index !== -1) state.faqs[index] = action.payload;
      })
      .addCase(updateFaq.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to update FAQ';
      })
      .addCase(deleteFaq.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteFaq.fulfilled, (state, action) => {
        state.loading = false;
        state.faqs = state.faqs.filter(f => f.id !== action.payload);
      })
      .addCase(deleteFaq.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to delete FAQ';
      });
  },
});

export const { clearFaqs } = faqSlice.actions;
export default faqSlice.reducer;
