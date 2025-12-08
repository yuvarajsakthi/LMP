import React, { useEffect, useState } from 'react';
import { Row, Col, Card, Statistic, Spin, message } from 'antd';
import { FileTextOutlined, CheckCircleOutlined, CloseCircleOutlined, ClockCircleOutlined, DollarOutlined, BankOutlined } from '@ant-design/icons';
import { Pie, Line } from '@ant-design/charts';
import Layout from '../../../layout/Layout';
import { managerDashboardAPI } from '../../../services/api/managerDashboardAPI';
import styles from './ManagerDashboard.module.css';
import type { DashboardStats } from '../../../types/managerDashboard';

const ManagerDashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      setLoading(true);
      const data = await managerDashboardAPI.getStats();
      setStats(data);
    } catch (error) {
      message.error('Failed to load dashboard stats');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div style={{ textAlign: 'center', padding: '50px' }}>
          <Spin size="large" />
        </div>
      </Layout>
    );
  }

  const pieConfig = {
    data: stats?.loanTypeDistribution || [],
    angleField: 'count',
    colorField: 'loanType',
    radius: 0.8,
    label: {
      text: 'loanType',
    },
    interactions: [{ type: 'element-active' }],
  };

  const lineConfig = {
    data: stats?.monthlyTrend || [],
    xField: 'month',
    yField: 'applicationCount',
    point: {
      size: 5,
      shape: 'diamond',
    },
  };

  return (
    <Layout>
      <div className={styles.container}>
        <h1 className={styles.title}>Manager Dashboard</h1>
        
        <Row gutter={[16, 16]}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Total Applications"
                value={stats?.totalApplications || 0}
                prefix={<FileTextOutlined />}
                styles={{ value: { color: '#1890ff' } }}
              />
            </Card>
          </Col>
          
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Pending"
                value={stats?.pendingApplications || 0}
                prefix={<ClockCircleOutlined />}
                styles={{ value: { color: '#faad14' } }}
              />
            </Card>
          </Col>
          
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Approved"
                value={stats?.approvedApplications || 0}
                prefix={<CheckCircleOutlined />}
                styles={{ value: { color: '#52c41a' } }}
              />
            </Card>
          </Col>
          
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Rejected"
                value={stats?.rejectedApplications || 0}
                prefix={<CloseCircleOutlined />}
                styles={{ value: { color: '#ff4d4f' } }}
              />
            </Card>
          </Col>

          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Disbursed"
                value={stats?.disbursedApplications || 0}
                prefix={<BankOutlined />}
                styles={{ value: { color: '#722ed1' } }}
              />
            </Card>
          </Col>

          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Total Disbursed Amount"
                value={stats?.totalDisbursedAmount || 0}
                prefix={<DollarOutlined />}
                precision={2}
                styles={{ value: { color: '#13c2c2' } }}
              />
            </Card>
          </Col>

          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Active Loans"
                value={stats?.activeLoans || 0}
                prefix={<BankOutlined />}
                styles={{ value: { color: '#eb2f96' } }}
              />
            </Card>
          </Col>
        </Row>

        <Row gutter={[16, 16]} style={{ marginTop: '24px' }}>
          <Col xs={24} lg={12}>
            <Card title="Loan Type Distribution">
              <Pie {...pieConfig} />
            </Card>
          </Col>

          <Col xs={24} lg={12}>
            <Card title="Monthly Application Trend">
              <Line {...lineConfig} />
            </Card>
          </Col>
        </Row>
      </div>
    </Layout>
  );
};

export default ManagerDashboard;
