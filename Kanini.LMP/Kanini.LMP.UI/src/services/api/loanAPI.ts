import axiosInstance from './axiosInstance';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';
import type { LoanCategory } from '../../types/loan';

// Fallback data matching the design mockup
const FALLBACK_LOAN_CATEGORIES: LoanCategory[] = [
    {
        loanProductId: 1,
        loanProductName: 'Medical Loan',
        loanProductDescription: 'Healthcare financing for medical emergencies',
        isActive: true
    },
    {
        loanProductId: 2,
        loanProductName: 'Education Loan',
        loanProductDescription: 'Finance your education with competitive rates',
        isActive: true
    },
    {
        loanProductId: 3,
        loanProductName: 'Debt Consolidation Loan',
        loanProductDescription: 'Consolidate your debts into one easy payment',
        isActive: true
    },
    {
        loanProductId: 4,
        loanProductName: 'Small Business Loan',
        loanProductDescription: 'Grow your business with flexible funding',
        isActive: true
    },
    {
        loanProductId: 5,
        loanProductName: 'Creditcard Loan',
        loanProductDescription: 'Premium credit cards with exclusive benefits',
        isActive: true
    },
    {
        loanProductId: 6,
        loanProductName: 'Vehicle Loan',
        loanProductDescription: 'Drive your dream car or bike today',
        isActive: true
    },
    {
        loanProductId: 7,
        loanProductName: 'Consumer Loan',
        loanProductDescription: 'Quick personal loans for your immediate needs',
        isActive: true
    },
    {
        loanProductId: 8,
        loanProductName: 'Personal Loan',
        loanProductDescription: 'Quick personal loans for your immediate needs',
        isActive: true
    },
    {
        loanProductId: 9,
        loanProductName: 'Housing Loan',
        loanProductDescription: 'Finance your dream home with competitive rates',
        isActive: true
    }
];

export const loanAPI = {
    async getLoanCategories(): Promise<LoanCategory[]> {
        try {
            return await ApiService.execute(async () => {
                const response = await axiosInstance.get<ApiResponse<LoanCategory[]>>(
                    '/api/LoanProduct/active'
                );
                return response;
            });
        } catch (error) {
            // If backend endpoint is not available, use fallback data
            console.warn('Using fallback loan categories data:', error);
            return FALLBACK_LOAN_CATEGORIES;
        }
    }
};
