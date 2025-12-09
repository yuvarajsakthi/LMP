import axiosInstance from './axiosInstance';

export const managerDashboardAPI = {
  getStats: async () => {
    const response = await axiosInstance.get('/api/ManagerDashboard/stats');
    return response.data;
  },

  getAllLoans: async () => {
    const response = await axiosInstance.get('/api/ManagerDashboard/loans');
    return response.data;
  },

  getLoanById: async (id: number) => {
    const response = await axiosInstance.get(`/api/ManagerDashboard/loans/${id}`);
    return response.data;
  },

  getLoansByStatus: async (status: string) => {
    const response = await axiosInstance.get(`/api/ManagerDashboard/loans/status/${status}`);
    return response.data;
  },

  updateLoanStatus: async (data: { loanApplicationBaseId: number; status: string; interestRate?: number; rejectionReason?: string }) => {
    const response = await axiosInstance.put('/api/ManagerDashboard/loans/status', data);
    return response.data;
  },

  disburseLoan: async (id: number) => {
    const response = await axiosInstance.post(`/api/ManagerDashboard/loans/${id}/disburse`);
    return response.data;
  }
};
