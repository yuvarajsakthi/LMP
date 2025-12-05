import React, { useEffect, useState } from "react";
import { Table, Card, Button } from "antd";
import styles from './ApplicationStatus.module.css';
import { useAuth } from "../../../context";
import { useNavigate } from "react-router-dom";
import { CUSTOMER_ROUTES } from "../../../config";
import { loanAPI } from "../../../services";

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
  const navigate = useNavigate();

  const addMonthsToDate = (date: string, monthsToAdd: number): string => {
    if (date === "-") return date;
    const newDate = new Date(date);
    newDate.setMonth(newDate.getMonth() + monthsToAdd);
    return newDate.toLocaleDateString("en-IN", {
      day: "2-digit",
      month: "2-digit", 
      year: "numeric"
    }).replace(/\//g, "-");
  };
  const [applicationData, setApplicationData] = useState<ApplicationData[]>(data || []);
  const getApplicationStatus = async () => {
    if (data) return;
    
    try {
      const { customerDashboardAPI } = await import('../../../services/api/customerDashboardAPI');
      const loans = await customerDashboardAPI.getApplicationStatus();
      const formattedData = loans.map((item: any) => ({
        loanType: item.loanType || 'Personal Loan',
        applicationID: item.applicationId?.toString() || '-',
        loanAmount: item.amount || 0,
        interest: '-',
        startDate: item.appliedDate ? new Date(item.appliedDate).toLocaleDateString("en-IN").replace(/\//g, "-") : "-",
        endDate: "-",
        loanTenure: 0,
        status: item.status || 'Pending',
        stage: 'Application',
      }));
      setApplicationData(formattedData);
    } catch (error) {
      console.error('Failed to fetch application status:', error);
      setApplicationData([]);
    }
  };
  useEffect(() => {
    getApplicationStatus();
  }, []);

  const columns = [
    {
      title: "Loan Type",
      dataIndex: "loanType",
      key: "loanType",
    },
    {
      title: "Application ID",
      dataIndex: "applicationID",
      key: "applicationID",
    },
    {
      title: "Loan Amount",
      dataIndex: "loanAmount",
      key: "loanAmount",
    },
    {
      title: "Interest",
      dataIndex: "interest",
      key: "interest",
    },
    {
      title: "Start Date",
      dataIndex: "startDate",
      key: "startDate",
    },
    {
      title: "End Date",
      dataIndex: "endDate",
      key: "endDate",
    },
    {
      title: "Loan Tenure",
      dataIndex: "loanTenure",
      key: "loanTenure",
    },
    {
      title: "Stage",
      dataIndex: "stage",
      key: "stage",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (text: string, record: ApplicationData) => {
        let backgroundColor;
        let textColor;
        let borderRadius = "22px";

        if (record.status === "Accept") {
          backgroundColor = "#6DD7C5";
        } else if (record.status === "Reject") {
          backgroundColor = "#F87D7C";
        } else {
          backgroundColor = "#FFE171";
          textColor = "black";
        }

        if (record.status === "Draft") {
          return (
            <Button
              style={{
                backgroundColor,
                color: textColor,
                borderRadius,
                border: "none",
              }}
              onClick={() => navigate(CUSTOMER_ROUTES.INTEGRATION)}
            >
              {text}
            </Button>
          );
        } else {
          const cellStyle = {
            backgroundColor,
            color: textColor,
            padding: "5px",
            borderRadius,
          };
          return <div style={cellStyle}>{text}</div>;
        }
      },
    },
  ];

  return (
    <Card title="Application status" style={{ height: '100%' }}>
      <Table
          dataSource={applicationData}
          columns={columns}
          pagination={{ pageSize: 5 }}
          scroll={{ x: 'max-content' }}
          size="middle"
          rowKey={(record) => record.applicationID || Math.random().toString()}
        />
    </Card>
  );
};

export default ApplicationStatus;
