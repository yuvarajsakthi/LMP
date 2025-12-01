import React, { useState, useEffect } from 'react';
import { Table, Divider, Input, Dropdown, Checkbox, Button, Space } from 'antd';
import { SearchOutlined, FilterOutlined, SortAscendingOutlined } from '@ant-design/icons';
import type { MenuProps } from 'antd';
import styles from './AppliedLoanManager.module.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../../context';
import { ROUTES } from '../../config';

interface LoanData {
  customerid: string;
  fullname: string;
  loantype: string;
  applicationnumber: string;
  loanamount: number;
  applicationdate: string;
  status: string;
}

const AppliedLoanManager: React.FC = () => {
  const navigate = useNavigate();
  const { token } = useAuth();
  const [showSearch, setShowSearch] = useState(false);
  const [searchCustomerId, setSearchCustomerId] = useState('');

  const [showFilter, setShowFilter] = useState(false);
  const [selectedStatus, setSelectedStatus] = useState<string[]>([]);
  const [data, setData] = useState<LoanData[]>([]);

  const handleViewClick = (applicationNumber: string) => {
    navigate(`${ROUTES.APPLIED_LOAN}/${applicationNumber}`);
  };

    useEffect(() => {
        getData();
    }, []);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Accept': return '#52c41a';
      case 'Reject': return '#ff4d4f';
      case 'Pending': return '#faad14';
      case 'Closed': return '#8c8c8c';
      default: return '#d9d9d9';
    }
  };

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
      render: (status: string) => (
        <div 
          className={styles.statusBadge}
          style={{ backgroundColor: getStatusColor(status) }}
        >
          {status}
        </div>
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
      const formattedData = res.data.map((item: any) => {
        const appliedDate = new Date(item.appliedDate);
        const formattedAppliedDate = appliedDate.toLocaleDateString('en-CA');

        return {
          customerid: item.customerId,
          fullname: item.fullname,
          loantype: item.loanCategory,
          applicationnumber: item.loanId,
          loanamount: item.status === 'Pending' ? item.appliedAmount : item.approvedAmount,
          applicationdate: formattedAppliedDate,
          status: item.status
        };
      });
      setData(formattedData);
    } catch (error) {
      console.error('Failed to fetch loan data:', error);
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchCustomerId(e.target.value);
  };

  const handleStatusChange = (values: string[]) => {
    setSelectedStatus(values);
  };

  const sortMenuItems: MenuProps['items'] = [
    { key: 'asc', label: 'Sort Ascending' },
    { key: 'desc', label: 'Sort Descending' },
  ];

  const handleSort: MenuProps['onClick'] = ({ key }) => {
    // Sort functionality can be implemented here if needed
    console.log('Sort order:', key);
  };

  const filteredData = data
    .filter(item => 
      item.customerid?.toString().includes(searchCustomerId)
    )
    .filter(item => 
      selectedStatus.length === 0 || selectedStatus.includes(item.status)
    );

  return (
    <div className={styles.mainContainer}>
      <div className={styles.appliedloanContainer}>
        <div className={styles.header}>
          <h2>Applied Loans</h2>
          <p>Loan Accelerator | Applied Loans</p>
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
            <Checkbox.Group onChange={handleStatusChange} value={selectedStatus}>
              <Checkbox value="Pending">Pending</Checkbox>
              <Checkbox value="Accept">Approved</Checkbox>
              <Checkbox value="Reject">Rejected</Checkbox>
              <Checkbox value="Closed">Closed</Checkbox>
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
          dataSource={filteredData} 
          pagination={{ pageSize: 7 }}
          rowKey="applicationnumber"
        />
      </div>
    </div>
  );
};

export default AppliedLoanManager;
