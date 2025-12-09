import { useState } from 'react';
import { loanApplicationAPI } from '../services/api/loanApplicationAPI';

export const useLoanSubmission = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);


  const submitPersonalLoan = async (customerId: number, data: any) => {
    setLoading(true);
    setError(null);
    try {
      const response = await loanApplicationAPI.createPersonalLoan(customerId, data);
      setLoading(false);
      return response;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit loan application');
      setLoading(false);
      throw err;
    }
  };

  const submitHomeLoan = async (customerId: number, data: any) => {
    setLoading(true);
    setError(null);
    try {
      const response = await loanApplicationAPI.createHomeLoan(customerId, data);
      setLoading(false);
      return response;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to submit loan application');
      setLoading(false);
      throw err;
    }
  };

  const submitVehicleLoan = async (customerId: number, data: any) => {
    setLoading(true);
    setError(null);
    try {
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
