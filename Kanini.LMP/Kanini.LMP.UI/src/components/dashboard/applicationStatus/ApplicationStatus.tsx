import React, { useEffect, useState } from "react";
import { Table, Card } from "antd";
import { useAuth } from "../../../context";
import { loanApplicationAPI } from "../../../services";
import styles from './ApplicationStatus.module.css';

interface ApplicationData {
  loanType: string;
  applicationID: string;
  loanAmount: number;
  interest: string | number;
  startDate: string;
  endDate: string;
  loanTenure: number;
  status: string;
  stage: string;
}

interface ApplicationStatusProps {
  data?: ApplicationData[];
}

const ApplicationStatus: React.FC<ApplicationStatusProps> = ({ data }) => {
  const { token } = useAuth();
  const [applicationData, setApplicationData] = useState<ApplicationData[]>(data || []);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (data) {
      setApplicationData(data);
      return;
    }

    const fetchApplications = async () => {
      if (!token?.customerId && !token?.CustomerId) return;
      
      setLoading(true);
      try {
        const customerId = token.customerId || token.CustomerId;
        const response = await loanApplicationAPI.getApplicationsByCustomerId(Number(customerId));
        
        if (response.success && response.data) {
          const formattedData = response.data.map((item: any) => ({
            loanType: item.loanType || 'Personal Loan',
            applicationID: item.loanApplicationBaseId?.toString() || '',
            loanAmount: item.requestedAmount || 0,
            interest: item.interestRate || '-',
            startDate: item.submissionDate ? new Date(item.submissionDate).toLocaleDateString("en-IN").replace(/\//g, "-") : "-",
            endDate: "-",
            loanTenure: item.tenureMonths || 0,
            status: item.status || 'Pending',
            stage: 'Application',
          }));
          setApplicationData(formattedData);
        }
      } catch (error) {
        console.error('Failed to fetch applications:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchApplications();
  }, [token, data]);

  const columns = [
    {
      title: "Application ID",
      dataIndex: "applicationID",
      key: "applicationID",
    },
    {
      title: "Loan Type",
      dataIndex: "loanType",
      key: "loanType",
    },
    {
      title: "Loan Amount",
      dataIndex: "loanAmount",
      key: "loanAmount",
      render: (amount: number) => amount > 0 ? `₹${amount.toLocaleString()}` : '-',
    },
    {
      title: "Tenure (Months)",
      dataIndex: "loanTenure",
      key: "loanTenure",
      render: (tenure: number) => tenure > 0 ? tenure : '-',
    },
    {
      title: "Submission Date",
      dataIndex: "startDate",
      key: "startDate",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (text: string) => {
        let backgroundColor;
        let textColor = "white";
        const borderRadius = "22px";

        switch (text) {
          case "Approved":
            backgroundColor = "#52c41a";
            break;
          case "Rejected":
            backgroundColor = "#ff4d4f";
            break;
          case "Disbursed":
            backgroundColor = "#722ed1";
            break;
          case "Submitted":
            backgroundColor = "#faad14";
            break;
          default:
            backgroundColor = "#1890ff";
        }

        return <div style={{ backgroundColor, color: textColor, padding: "5px 10px", borderRadius, textAlign: "center" }}>{text}</div>;
      },
    },
  ];

  return (
    <Card title="Application status" className={styles.box4}>
      <Table
        dataSource={applicationData}
        columns={columns}
        pagination={{ pageSize: 5 }}
        scroll={{ x: 'max-content' }}
        size="middle"
        loading={loading}
        rowKey={(record) => record.applicationID}
      />
    </Card>
  );
};

export default ApplicationStatus;
