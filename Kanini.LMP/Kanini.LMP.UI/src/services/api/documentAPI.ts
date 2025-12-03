import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface DocumentUpload {
  documentId: number;
  documentName: string;
  documentType: string;
  filePath: string;
  uploadedAt: string;
  isVerified: boolean;
  loanApplicationBaseId: number;
}

export const documentAPI = {
  async uploadDocument(formData: FormData): Promise<DocumentUpload> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<DocumentUpload>>(
        API_ENDPOINTS.UPLOAD_DOCUMENT,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } }
      );
      return response;
    });
  },

  async downloadDocument(documentId: number): Promise<Blob> {
    const response = await axiosInstance.get(
      `${API_ENDPOINTS.DOWNLOAD_DOCUMENT}/${documentId}`,
      { responseType: 'blob' }
    );
    return response.data;
  },

  async viewDocument(documentId: number): Promise<Blob> {
    const response = await axiosInstance.get(
      `${API_ENDPOINTS.VIEW_DOCUMENT}/${documentId}`,
      { responseType: 'blob' }
    );
    return response.data;
  },

  async getDocumentsByApplication(loanApplicationBaseId: number): Promise<DocumentUpload[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<DocumentUpload[]>>(
        `${API_ENDPOINTS.GET_DOCUMENTS_BY_APPLICATION}/${loanApplicationBaseId}`
      );
      return response;
    });
  },

  async verifyDocument(documentId: number, isApproved: boolean, comments?: string): Promise<DocumentUpload> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<DocumentUpload>>(
        API_ENDPOINTS.VERIFY_DOCUMENT,
        { documentId, isApproved, comments }
      );
      return response;
    });
  },

  async deleteDocument(documentId: number): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.delete<ApiResponse<{ message: string }>>(
        `${API_ENDPOINTS.DELETE_DOCUMENT}/${documentId}`
      );
      return response;
    });
  },

  async getPendingDocuments(): Promise<DocumentUpload[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<DocumentUpload[]>>(
        API_ENDPOINTS.GET_PENDING_DOCUMENTS
      );
      return response;
    });
  }
};