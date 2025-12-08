import React, { useEffect, useState } from 'react';
import { Table, Tag, Button, Modal, Form, Input, InputNumber, Select, message, Space } from 'antd';
import { EyeOutlined } from '@ant-design/icons';
import { managerDashboardAPI } from '../../services/api/managerDashboardAPI';
import styles from './AppliedLoanManager.module.css';
import type { LoanApplicationDetail } from '../../types/managerDashboard';

const { Option } = Select;
const { TextArea } = Input;

const AppliedLoanManager: React.FC = () => {
  const [loans, setLoans] = useState<LoanApplicationDetail[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedLoan, setSelectedLoan] = useState<LoanApplicationDetail | null>(null);
  const [detailsVisible, setDetailsVisible] = useState(false);
  const [statusVisible, setStatusVisible] = useState(false);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchLoans();
  }, []);

  const fetchLoans = async () => {
    try {
      setLoading(true);
      const data = await managerDashboardAPI.getAllLoans();
      setLoans(data);
    } catch (error) {
      message.error('Failed to load loan applications');
    } finally {
      setLoading(false);
    }
  };

  const showDetails = async (id: number) => {
    try {
      const data = await managerDashboardAPI.getLoanById(id);
      setSelectedLoan(data);
      setDetailsVisible(true);
    } catch (error) {
      message.error('Failed to load loan details');
    }
  };

  const showStatusModal = (loan: LoanApplicationDetail) => {
    setSelectedLoan(loan);
    form.setFieldsValue({
      status: loan.status,
      interestRate: loan.interestRate,
      rejectionReason: loan.rejectionReason
    });
    setStatusVisible(true);
  };

  const handleStatusUpdate = async (values: any) => {
    try {
      await managerDashboardAPI.updateLoanStatus({
        loanApplicationBaseId: selectedLoan!.loanApplicationBaseId,
        ...values
      });
      message.success('Loan status updated successfully');
      setStatusVisible(false);
      fetchLoans();
    } catch (error) {
      message.error('Failed to update loan status');
    }
  };

  const getStatusColor = (status: string) => {
    const colors: Record<string, string> = {
      Pending: 'orange',
      Submitted: 'blue',
      Approved: 'green',
      Rejected: 'red',
      Disbursed: 'purple'
    };
    return colors[status] || 'default';
  };

  const columns = [
    {
      title: 'ID',
      dataIndex: 'loanApplicationBaseId',
      key: 'id',
      width: 80,
    },
    {
      title: 'Customer',
      dataIndex: 'customerName',
      key: 'customerName',
    },
    {
      title: 'Loan Type',
      dataIndex: 'loanType',
      key: 'loanType',
    },
    {
      title: 'Amount',
      dataIndex: 'requestedAmount',
      key: 'amount',
      render: (amount: number) => `₹${amount.toLocaleString()}`,
    },
    {
      title: 'Tenure',
      dataIndex: 'tenureMonths',
      key: 'tenure',
      render: (months: number) => `${months} months`,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => <Tag color={getStatusColor(status)}>{status}</Tag>,
    },
    {
      title: 'Submission Date',
      dataIndex: 'submissionDate',
      key: 'submissionDate',
      render: (date: string) => new Date(date).toLocaleDateString(),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: any, record: LoanApplicationDetail) => (
        <Space>
          <Button icon={<EyeOutlined />} onClick={() => showDetails(record.loanApplicationBaseId)}>
            View
          </Button>
          {(record.status === 'Pending' || record.status === 'Submitted') && (
            <Button type="primary" onClick={() => showStatusModal(record)}>
              Update Status
            </Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div className={styles.container}>
      <h1>Applied Loan Applications</h1>
      <Table
        columns={columns}
        dataSource={loans}
        loading={loading}
        rowKey="loanApplicationBaseId"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title="Loan Application Details"
        open={detailsVisible}
        onCancel={() => setDetailsVisible(false)}
        footer={null}
        width={800}
      >
        {selectedLoan && (
          <div className={styles.details}>
            <h3>Customer Information</h3>
            <p><strong>Name:</strong> {selectedLoan.customerName}</p>
            <p><strong>Email:</strong> {selectedLoan.customerEmail}</p>
            <p><strong>Phone:</strong> {selectedLoan.customerPhone}</p>

            <h3>Loan Details</h3>
            <p><strong>Type:</strong> {selectedLoan.loanType}</p>
            <p><strong>Amount:</strong> ₹{selectedLoan.requestedAmount.toLocaleString()}</p>
            <p><strong>Tenure:</strong> {selectedLoan.tenureMonths} months</p>
            <p><strong>Interest Rate:</strong> {selectedLoan.interestRate || 'N/A'}%</p>
            <p><strong>Status:</strong> <Tag color={getStatusColor(selectedLoan.status)}>{selectedLoan.status}</Tag></p>

            {selectedLoan.emiStatus && (
              <>
                <h3>EMI Details</h3>
                <p><strong>Monthly EMI:</strong> ₹{selectedLoan.emiStatus.monthlyEMI.toLocaleString()}</p>
                <p><strong>Total Repayment:</strong> ₹{selectedLoan.emiStatus.totalRepaymentAmount.toLocaleString()}</p>
                <p><strong>Total Interest:</strong> ₹{selectedLoan.emiStatus.totalInterestPaid.toLocaleString()}</p>
              </>
            )}

            {selectedLoan.documents.length > 0 && (
              <>
                <h3>Documents</h3>
                {selectedLoan.documents.map(doc => (
                  <p key={doc.documentId}><strong>{doc.documentType}:</strong> {doc.documentName}</p>
                ))}
              </>
            )}
          </div>
        )}
      </Modal>

      <Modal
        title="Update Loan Status"
        open={statusVisible}
        onCancel={() => setStatusVisible(false)}
        footer={null}
      >
        <Form form={form} onFinish={handleStatusUpdate} layout="vertical">
          <Form.Item name="status" label="Status" rules={[{ required: true }]}>
            <Select>
              <Option value="Pending">Pending</Option>
              <Option value="Approved">Approved</Option>
              <Option value="Rejected">Rejected</Option>
            </Select>
          </Form.Item>

          <Form.Item name="interestRate" label="Interest Rate (%)">
            <InputNumber min={0} max={100} step={0.1} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="rejectionReason" label="Rejection Reason">
            <TextArea rows={4} />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit">
                Update
              </Button>
              <Button onClick={() => setStatusVisible(false)}>
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default AppliedLoanManager;
