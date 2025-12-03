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
    if (data) return; // Use provided data if available
    
    try {
      const loans = await loanAPI.getCustomerApplications();
      const formattedData = loans.map((item: any) => ({
        loanType: item.loanType || 'Personal Loan',
        applicationID: item.loanApplicationBaseId?.toString() || '-',
        loanAmount: item.loanAmount || 0,
        interest: item.interestRate || '-',
        startDate: item.createdAt ? new Date(item.createdAt).toLocaleDateString("en-IN").replace(/\//g, "-") : "-",
        endDate: item.expectedEndDate ? new Date(item.expectedEndDate).toLocaleDateString("en-IN").replace(/\//g, "-") : "-",
        loanTenure: item.tenure || 0,
        status: item.status || 'Pending',
        stage: item.stage || 'Application',
      }));
      setApplicationData(formattedData);
    } catch (error) {
      // Set empty data when API is unavailable
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
    <div className={styles.box4}>
      <Card
        title="Application status"
        style={{
          width: "100%",
          height: "100%",
        }}
      >
        {" "}
        <Table
          dataSource={applicationData}
          columns={columns}
          pagination={{ pageSize: 5 }}
          scroll={{ x: 'max-content' }}
          size="middle"
          rowKey={(record) => record.applicationID || Math.random().toString()}
        />
      </Card>
    </div>
  );
};

export default ApplicationStatus;
