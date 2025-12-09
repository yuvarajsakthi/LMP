import React, { useEffect, useState } from 'react';
import { Table, Tag, Button, Modal, Form, Input, Select, message, Space, Popconfirm } from 'antd';
import { EyeOutlined, DollarOutlined } from '@ant-design/icons';
import { fetchAllLoans, fetchLoanById, updateManagerLoanStatus, disburseLoan, clearSelectedLoan } from '../../store';
import styles from './AppliedLoanManager.module.css';
import type { LoanApplicationDetail } from '../../types/managerDashboard';
import { useAppDispatch, useAppSelector } from '../../hooks';

const { Option } = Select;
const { TextArea } = Input;

const AppliedLoanManager: React.FC = () => {
  const dispatch = useAppDispatch();
  const { loans, selectedLoan, loading } = useAppSelector((state) => state.manager);
  const [detailsVisible, setDetailsVisible] = useState(false);
  const [statusVisible, setStatusVisible] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState<string>('');
  const [form] = Form.useForm();

  useEffect(() => {
    dispatch(fetchAllLoans());
  }, [dispatch]);

  const showDetails = async (id: number) => {
    try {
      await dispatch(fetchLoanById(id)).unwrap();
      setDetailsVisible(true);
    } catch (error) {
      message.error('Failed to load loan details');
    }
  };

  const showStatusModal = async (loan: LoanApplicationDetail) => {
    await dispatch(fetchLoanById(loan.loanApplicationBaseId)).unwrap();
    setSelectedStatus(loan.status);
    setStatusVisible(true);
    setTimeout(() => {
      form.setFieldsValue({
        status: loan.status,
        rejectionReason: loan.rejectionReason
      });
    }, 0);
  };

  const handleStatusChange = (value: string) => {
    setSelectedStatus(value);
  };

  const handleStatusUpdate = async (values: any) => {
    try {
      await dispatch(updateManagerLoanStatus({
        loanApplicationBaseId: selectedLoan!.loanApplicationBaseId,
        status: values.status,
        rejectionReason: values.status === 'Rejected' ? values.rejectionReason : undefined
      })).unwrap();
      message.success('Loan status updated successfully');
      setStatusVisible(false);
      form.resetFields();
      dispatch(fetchAllLoans());
    } catch (error) {
      message.error('Failed to update loan status');
    }
  };

  const handleDisburse = async (id: number) => {
    try {
      await dispatch(disburseLoan(id)).unwrap();
      message.success('Payment disbursed successfully');
      dispatch(fetchAllLoans());
    } catch (error) {
      message.error('Failed to disburse payment');
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
      width: 250,
      render: (_: any, record: LoanApplicationDetail) => (
        <Space>
          <Button size="small" icon={<EyeOutlined />} onClick={() => showDetails(record.loanApplicationBaseId)}>
            View
          </Button>
          {(record.status === 'Pending' || record.status === 'Submitted') && (
            <Button size="small" type="primary" onClick={() => showStatusModal(record)}>
              Update
            </Button>
          )}
          {record.status === 'Approved' && (
            <Popconfirm
              title="Disburse Payment"
              description="Send the payment to customer?"
              onConfirm={() => handleDisburse(record.loanApplicationBaseId)}
              okText="Yes"
              cancelText="No"
            >
              <Button size="small" type="primary" icon={<DollarOutlined />} style={{ background: '#52c41a' }}>
                Disburse
              </Button>
            </Popconfirm>
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
        onCancel={() => {
          setDetailsVisible(false);
          dispatch(clearSelectedLoan());
        }}
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
        onCancel={() => {
          setStatusVisible(false);
          form.resetFields();
          dispatch(clearSelectedLoan());
        }}
        footer={null}
        width={500}
      >
        <Form form={form} onFinish={handleStatusUpdate} layout="vertical">
          <Form.Item name="status" label="Status" rules={[{ required: true, message: 'Please select status' }]}>
            <Select onChange={handleStatusChange} placeholder="Select status">
              <Option value="Approved">Approved</Option>
              <Option value="Rejected">Rejected</Option>
            </Select>
          </Form.Item>

          {selectedStatus === 'Rejected' && (
            <Form.Item 
              name="rejectionReason" 
              label="Rejection Reason"
              rules={[{ required: true, message: 'Please provide rejection reason' }]}
            >
              <TextArea rows={4} placeholder="Enter reason for rejection" />
            </Form.Item>
          )}

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit">
                Update Status
              </Button>
              <Button onClick={() => {
                setStatusVisible(false);
                form.resetFields();
                dispatch(clearSelectedLoan());
              }}>
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
