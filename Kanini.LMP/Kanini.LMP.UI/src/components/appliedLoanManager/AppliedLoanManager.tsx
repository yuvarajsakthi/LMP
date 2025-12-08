import React, { useState, useEffect } from 'react';
import { Table, Divider, Input, Dropdown, Checkbox, Button, Space, Select, message, Modal, List } from 'antd';
import { SearchOutlined, FilterOutlined, SortAscendingOutlined, FileTextOutlined, EyeOutlined, DownloadOutlined, LikeOutlined, CheckCircleOutlined, ExclamationCircleOutlined } from '@ant-design/icons';
import type { MenuProps } from 'antd';
import styles from './AppliedLoanManager.module.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../../context';
import { MANAGER_ROUTES } from '../../config';

interface LoanData {
  customerid: string;
  fullname: string;
  loantype: string;
  applicationnumber: string;
  loanamount: number;
  applicationdate: string;
  status: string;
  documentUrl?: string;
  emiStatus?: string;
}

interface Document {
  documentId: number;
  documentName: string;
  documentType: string;
  uploadDate: string;
  documentUrl: string;
}

const AppliedLoanManager: React.FC = () => {
  const navigate = useNavigate();
  const { token } = useAuth();
  const [showSearch, setShowSearch] = useState(false);
  const [searchCustomerId, setSearchCustomerId] = useState('');

  const [showFilter, setShowFilter] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState<string[]>([]);
  const [data, setData] = useState<LoanData[]>([]);
  const [sortedData, setSortedData] = useState<LoanData[]>([]);
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc' | null>(null);
  const [isDocumentModalOpen, setIsDocumentModalOpen] = useState(false);
  const [selectedLoanDocuments, setSelectedLoanDocuments] = useState<Document[]>([]);
  const [loadingDocuments, setLoadingDocuments] = useState(false);
  const [pendingStatusChange, setPendingStatusChange] = useState<{ loanId: string; status: string; oldStatus: string } | null>(null);

  const handleViewClick = (applicationNumber: string) => {
    navigate(`${MANAGER_ROUTES.APPLIED_LOAN}/${applicationNumber}`);
  };

  const handleStatusChange = (loanId: string, newStatus: string, oldStatus: string) => {
    if (newStatus === 'Approved' || newStatus === 'Disbursed') {
      setPendingStatusChange({ loanId, status: newStatus, oldStatus });
    } else {
      confirmStatusChange(loanId, newStatus);
    }
  };

  const confirmStatusChange = async (loanId: string, newStatus: string) => {
    try {
      const statusMap: { [key: string]: number } = {
        'Draft': 0,
        'Submitted': 1,
        'Withdrawn': 2,
        'Pending': 3,
        'Rejected': 4,
        'Approved': 5,
        'Disbursed': 6
      };
      
      await axios.put(`/api/LoanApplicationFlow/${loanId}/status`, statusMap[newStatus], {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (newStatus === 'Disbursed') {
        try {
          await axios.post(`/api/EMI/create-on-disbursement/${loanId}`, {}, {
            headers: { Authorization: `Bearer ${token}` }
          });
        } catch (emiError) {
          console.error('EMI creation error:', emiError);
        }
      }

      message.success('Status updated successfully');
      getData();
    } catch (error) {
      message.error('Failed to update status');
      console.error('Status update error:', error);
    }
  };

  const handleConfirmStatusChange = () => {
    if (pendingStatusChange) {
      confirmStatusChange(pendingStatusChange.loanId, pendingStatusChange.status);
      setPendingStatusChange(null);
    }
  };

  const handleCancelStatusChange = () => {
    setPendingStatusChange(null);
    getData();
  };

  const handleViewDocuments = async (loanId: string) => {
    try {
      setLoadingDocuments(true);
      const res = await axios.get(`/api/Document/loan/${loanId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      
      if (res.data && res.data.length > 0) {
        setSelectedLoanDocuments(res.data);
        setIsDocumentModalOpen(true);
      } else {
        message.info('No documents available for this loan');
      }
    } catch (error) {
      message.error('Failed to fetch documents');
      console.error('Document fetch error:', error);
    } finally {
      setLoadingDocuments(false);
    }
  };

  const handleViewSingleDocument = (documentUrl: string) => {
    window.open(documentUrl, '_blank');
  };

  const handleDownloadDocument = async (documentId: number, documentName: string) => {
    try {
      const res = await axios.get(`/api/Document/download/${documentId}`, {
        headers: { Authorization: `Bearer ${token}` },
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([res.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', documentName);
      document.body.appendChild(link);
      link.click();
      link.remove();
      message.success('Document downloaded successfully');
    } catch (error) {
      message.error('Failed to download document');
      console.error('Download error:', error);
    }
  };

    useEffect(() => {
        getData();
    }, []);


  const columns = [
    {
      title: 'Customer ID',
      dataIndex: 'customerid',
      key: 'customerid',
      align: 'center' as const,
    },
    {
      title: 'Full Name',
      dataIndex: 'fullname',
      key: 'fullname',
      align: 'center' as const,
      sorter: (a: LoanData, b: LoanData) => a.fullname.localeCompare(b.fullname),
    },
    {
      title: 'Loan Type',
      dataIndex: 'loantype',
      key: 'loantype',
      align: 'center' as const,
    },
    {
      title: 'Application Number',
      dataIndex: 'applicationnumber',
      key: 'applicationnumber',
      align: 'center' as const,
    },
    {
      title: 'Loan Amount',
      dataIndex: 'loanamount',
      key: 'loanamount',
      align: 'center' as const,
      render: (amount: number) => `₹${amount.toLocaleString()}`,
    },
    {
      title: 'Application Date',
      dataIndex: 'applicationdate',
      key: 'applicationdate',
      align: 'center' as const,
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      align: 'center' as const,
      render: (status: string, record: LoanData) => (
        <Select
          value={status}
          style={{ width: 120 }}
          onChange={(value) => handleStatusChange(record.applicationnumber, value, status)}
          options={[
            { value: 'Draft', label: 'Draft' },
            { value: 'Submitted', label: 'Submitted' },
            { value: 'Pending', label: 'Pending' },
            { value: 'Approved', label: 'Approved' },
            { value: 'Rejected', label: 'Rejected' },
            { value: 'Disbursed', label: 'Disbursed' },
            { value: 'Withdrawn', label: 'Withdrawn' }
          ]}
        />
      ),
    },
    {
      title: 'EMI Status',
      dataIndex: 'emiStatus',
      key: 'emiStatus',
      align: 'center' as const,
      render: (emiStatus: string) => {
        const getEMIStatusColor = (status: string) => {
          switch (status) {
            case 'Completed': return '#52c41a';
            case 'In Progress': return '#1890ff';
            case 'Not Started': return '#d9d9d9';
            default: return '#d9d9d9';
          }
        };
        return (
          <div 
            style={{ 
              backgroundColor: getEMIStatusColor(emiStatus),
              color: '#fff',
              padding: '4px 12px',
              borderRadius: '4px',
              display: 'inline-block',
              fontSize: '12px'
            }}
          >
            {emiStatus || 'Not Started'}
          </div>
        );
      },
    },
    {
      title: 'View Document',
      key: 'document',
      align: 'center' as const,
      render: (_: any, record: LoanData) => (
        <Button
          icon={<FileTextOutlined />}
          size="small"
          loading={loadingDocuments}
          onClick={() => handleViewDocuments(record.applicationnumber)}
        >
          Documents
        </Button>
      ),
    },
    {
      title: 'Action',
      key: 'action',
      align: 'center' as const,
      render: (_: any, record: LoanData) => (
        <Button
          type="primary"
          size="small"
          onClick={() => handleViewClick(record.applicationnumber)}
        >
          View
        </Button>
      ),
    },
  ];

  const getData = async () => {
    try {
      const res = await axios.get('/loans/AppliedLoans', {
        headers: { Authorization: `Bearer ${token}` }
      });
      const formattedData = await Promise.all(res.data.map(async (item: any) => {
        const appliedDate = new Date(item.appliedDate);
        const formattedAppliedDate = appliedDate.toLocaleDateString('en-CA');

        let emiStatus = 'Not Started';
        if (item.status === 'Disbursed') {
          try {
            const emiRes = await axios.get(`/api/EMI/dashboard/${item.customerId}`, {
              headers: { Authorization: `Bearer ${token}` }
            });
            if (emiRes.data?.data) {
              const emiData = emiRes.data.data;
              const totalInstallments = emiData.paidInstallments + emiData.remainingInstallments;
              if (emiData.paidInstallments >= totalInstallments) {
                emiStatus = 'Completed';
              } else if (emiData.paidInstallments > 0) {
                emiStatus = 'In Progress';
              }
            }
          } catch (emiError) {
            console.error('Failed to fetch EMI status:', emiError);
          }
        }

        return {
          customerid: item.customerId,
          fullname: item.fullname,
          loantype: item.loanCategory,
          applicationnumber: item.loanId,
          loanamount: item.status === 'Pending' ? item.appliedAmount : item.approvedAmount,
          applicationdate: formattedAppliedDate,
          status: item.status,
          emiStatus
        };
      }));
      setData(formattedData);
    } catch (error) {
      console.error('Failed to fetch loan data:', error);
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchCustomerId(e.target.value);
  };

  const handleFilterStatusChange = (values: string[]) => {
    setSelectedStatus(values);
  };

  const sortMenuItems: MenuProps['items'] = [
    { key: 'asc', label: 'Sort by Name (A-Z)' },
    { key: 'desc', label: 'Sort by Name (Z-A)' },
    { key: 'amountAsc', label: 'Sort by Amount (Low to High)' },
    { key: 'amountDesc', label: 'Sort by Amount (High to Low)' },
    { key: 'dateAsc', label: 'Sort by Date (Oldest First)' },
    { key: 'dateDesc', label: 'Sort by Date (Newest First)' },
  ];

  const handleSort: MenuProps['onClick'] = ({ key }) => {
    let sorted = [...filteredData];
    
    switch(key) {
      case 'asc':
        sorted.sort((a, b) => a.fullname.localeCompare(b.fullname));
        break;
      case 'desc':
        sorted.sort((a, b) => b.fullname.localeCompare(a.fullname));
        break;
      case 'amountAsc':
        sorted.sort((a, b) => a.loanamount - b.loanamount);
        break;
      case 'amountDesc':
        sorted.sort((a, b) => b.loanamount - a.loanamount);
        break;
      case 'dateAsc':
        sorted.sort((a, b) => new Date(a.applicationdate).getTime() - new Date(b.applicationdate).getTime());
        break;
      case 'dateDesc':
        sorted.sort((a, b) => new Date(b.applicationdate).getTime() - new Date(a.applicationdate).getTime());
        break;
    }
    
    setSortedData(sorted);
    setSortOrder(key as any);
  };

  const filteredData = data
    .filter(item => 
      searchCustomerId === '' || item.customerid?.toString().includes(searchCustomerId)
    )
    .filter(item => 
      selectedStatus.length === 0 || selectedStatus.includes(item.status)
    );

  const displayData = sortOrder ? sortedData : filteredData;

  useEffect(() => {
    if (sortOrder) {
      handleSort({ key: sortOrder } as any);
    }
  }, [filteredData]);

  return (
    <div className={styles.mainContainer}>
      <div className={styles.appliedloanContainer}>
        <div className={styles.header}>
          <h2>Applied Loans</h2>
          <p>LMP | Applied Loans</p>
        </div>
        
        <Divider />
        
        <div className={styles.controls}>
          <Space>
            <Button 
              icon={<FilterOutlined />} 
              onClick={() => setShowFilter(!showFilter)}
            >
              Filter
            </Button>
            
            <Button 
              icon={<SearchOutlined />} 
              onClick={() => setShowSearch(!showSearch)}
            >
              Search
            </Button>
            
            <Dropdown menu={{ items: sortMenuItems, onClick: handleSort }}>
              <Button icon={<SortAscendingOutlined />}>
                Sort
              </Button>
            </Dropdown>
          </Space>
        </div>

        {showFilter && (
          <div className={styles.filterContainer}>
            <Checkbox.Group onChange={handleFilterStatusChange} value={selectedStatus}>
              <Checkbox value="Draft">Draft</Checkbox>
              <Checkbox value="Submitted">Submitted</Checkbox>
              <Checkbox value="Pending">Pending</Checkbox>
              <Checkbox value="Approved">Approved</Checkbox>
              <Checkbox value="Rejected">Rejected</Checkbox>
              <Checkbox value="Disbursed">Disbursed</Checkbox>
              <Checkbox value="Withdrawn">Withdrawn</Checkbox>
            </Checkbox.Group>
          </div>
        )}

        {showSearch && (
          <div className={styles.searchContainer}>
            <Input
              placeholder="Search by Customer ID..."
              value={searchCustomerId}
              onChange={handleSearchChange}
              prefix={<SearchOutlined />}
            />
          </div>
        )}
        
        <Table 
          columns={columns} 
          dataSource={displayData} 
          pagination={{ 
            pageSize: 7,
            showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
            showSizeChanger: false
          }}
          rowKey="applicationnumber"
        />

        <Modal
          title={
            <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
              {pendingStatusChange?.status === 'Approved' ? (
                <LikeOutlined style={{ fontSize: '24px', color: '#52c41a' }} />
              ) : (
                <CheckCircleOutlined style={{ fontSize: '24px', color: '#1890ff' }} />
              )}
              <span>Confirm Status Change</span>
            </div>
          }
          open={!!pendingStatusChange}
          onCancel={handleCancelStatusChange}
          footer={[
            <Button key="cancel" onClick={handleCancelStatusChange}>
              Cancel
            </Button>,
            <Button 
              key="confirm" 
              type="primary" 
              icon={pendingStatusChange?.status === 'Approved' ? <LikeOutlined /> : <CheckCircleOutlined />}
              onClick={handleConfirmStatusChange}
            >
              Confirm
            </Button>
          ]}
        >
          <div style={{ padding: '20px 0' }}>
            <p style={{ fontSize: '16px', marginBottom: '10px' }}>
              <ExclamationCircleOutlined style={{ color: '#faad14', marginRight: '8px' }} />
              Are you sure you want to change the status to <strong>{pendingStatusChange?.status}</strong>?
            </p>
            <p style={{ color: '#666' }}>
              This action will update the loan application status and notify the customer.
            </p>
          </div>
        </Modal>

        <Modal
          title="Loan Documents"
          open={isDocumentModalOpen}
          onCancel={() => setIsDocumentModalOpen(false)}
          footer={[
            <Button key="close" onClick={() => setIsDocumentModalOpen(false)}>
              Close
            </Button>
          ]}
          width={700}
        >
          <List
            dataSource={selectedLoanDocuments}
            renderItem={(doc) => (
              <List.Item
                actions={[
                  <Button
                    key="view"
                    type="link"
                    icon={<EyeOutlined />}
                    onClick={() => handleViewSingleDocument(doc.documentUrl)}
                  >
                    View
                  </Button>,
                  <Button
                    key="download"
                    type="link"
                    icon={<DownloadOutlined />}
                    onClick={() => handleDownloadDocument(doc.documentId, doc.documentName)}
                  >
                    Download
                  </Button>
                ]}
              >
                <List.Item.Meta
                  avatar={<FileTextOutlined style={{ fontSize: '24px', color: '#1890ff' }} />}
                  title={doc.documentName}
                  description={`Type: ${doc.documentType} | Uploaded: ${new Date(doc.uploadDate).toLocaleDateString()}`}
                />
              </List.Item>
            )}
          />
        </Modal>
      </div>
    </div>
  );
};

export default AppliedLoanManager;
