import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context';
import { loanApplicationAPI } from '../services/api/loanApplicationAPI';
import type {
  PersonalLoanApplicationCreateDTO,
  HomeLoanApplicationCreateDTO,
  VehicleLoanApplicationCreateDTO
} from '../types/loanApplicationCreate';

export const useLoanSubmission = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { token } = useAuth();
  const navigate = useNavigate();

  const submitPersonalLoan = async (data: PersonalLoanApplicationCreateDTO) => {
    setLoading(true);
    setError(null);
    try {
      const customerId = data.customerId;
      const response = await loanApplicationAPI.createPersonalLoan(customerId, data);
      setLoading(false);
      return response;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit loan application');
      setLoading(false);
      throw err;
    }
  };

  const submitHomeLoan = async (data: HomeLoanApplicationCreateDTO) => {
    setLoading(true);
    setError(null);
    try {
      const customerId = data.customerId;
      const response = await loanApplicationAPI.createHomeLoan(customerId, data);
      setLoading(false);
      return response;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit loan application');
      setLoading(false);
      throw err;
    }
  };

  const submitVehicleLoan = async (data: VehicleLoanApplicationCreateDTO) => {
    setLoading(true);
    setError(null);
    try {
      const customerId = data.customerId;
      const response = await loanApplicationAPI.createVehicleLoan(customerId, data);
      setLoading(false);
      return response;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit loan application');
      setLoading(false);
      throw err;
    }
  };

  return {
    loading,
    error,
    submitPersonalLoan,
    submitHomeLoan,
    submitVehicleLoan
  };
};
