import { render, screen, fireEvent } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';
import Login from '../components/loginComponent/LoginComponent';

vi.mock('../context', () => ({
  useAuth: () => ({ setToken: vi.fn() }),
}));

vi.mock('../utils', () => ({
  validateField: vi.fn().mockResolvedValue(''),
}));

vi.mock('../middleware', () => ({
  authMiddleware: { setToken: vi.fn(), getToken: vi.fn() },
}));

const mockStore = configureStore({
  reducer: {
    auth: () => ({ isLoading: false, error: null }),
  },
});

describe('Login Component', () => {
  it('renders login form', () => {
    render(
      <Provider store={mockStore}>
        <BrowserRouter>
          <Login />
        </BrowserRouter>
      </Provider>
    );
    expect(screen.getByText('Sign In')).toBeInTheDocument();
    expect(screen.getByPlaceholderText('name@email.com')).toBeInTheDocument();
  });

  it('toggles between password and OTP login', () => {
    render(
      <Provider store={mockStore}>
        <BrowserRouter>
          <Login />
        </BrowserRouter>
      </Provider>
    );
    fireEvent.click(screen.getByText('Use OTP'));
    expect(screen.getByPlaceholderText('6-digit OTP')).toBeInTheDocument();
  });
});
