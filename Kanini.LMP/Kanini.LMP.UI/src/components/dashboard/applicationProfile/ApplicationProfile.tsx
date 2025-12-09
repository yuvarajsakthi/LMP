import React, { useEffect, useState } from 'react';
import styles from './ApplicationProfile.module.css';
import { Card, Button, message } from 'antd';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';
import axios from 'axios';
import { useAuth } from '../../../context';

interface EMIData {
  emiId: number;
  monthlyEMI: number;
  totalInterestPaid: number;
  remainingAmount: number;
  paidInstallments: number;
  remainingInstallments: number;
  nextPaymentDate: string;
  canPayNow: boolean;
}

const ApplicationProfile: React.FC = () => {
  const { token } = useAuth();
  const [emiData, setEmiData] = useState<EMIData | null>(null);
  const [loading, setLoading] = useState(true);
  const [paying, setPaying] = useState(false);

  useEffect(() => {
    fetchEMIData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchEMIData = async () => {
    try {
      const customerId = localStorage.getItem('customerId');
      if (!customerId) return;

      const res = await axios.get(`/api/EMI/dashboard/${customerId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      
      if (res.data?.data) {
        setEmiData(res.data.data);
      }
    } catch (error) {
      console.error('Failed to fetch EMI data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePayEMI = async () => {
    if (!emiData) return;
    
    setPaying(true);
    try {
      await axios.post(`/api/EMI/pay/${emiData.emiId}`, {}, {
        headers: { Authorization: `Bearer ${token}` }
      });
      message.success('EMI paid successfully!');
      fetchEMIData();
    } catch (error) {
      message.error('Failed to pay EMI');
    } finally {
      setPaying(false);
    }
  };

  if (loading) {
    return <Card title="My EMI Dashboard" loading={true} style={{ height: '100%' }} />;
  }

  if (!emiData) {
    return (
      <Card title="My EMI Dashboard" style={{ height: '100%' }}>
        <div style={{ textAlign: 'center', padding: '40px 20px', color: '#999' }}>
          <p>No active EMI plan found</p>
          <p style={{ fontSize: '12px' }}>Your EMI will appear here once your loan is disbursed</p>
        </div>
      </Card>
    );
  }

  const paidPercentage = Math.round((emiData.paidInstallments / (emiData.paidInstallments + emiData.remainingInstallments)) * 100);
  const remainingPercentage = 100 - paidPercentage;

  const totalPrincipal = emiData.monthlyEMI * (emiData.paidInstallments + emiData.remainingInstallments) - emiData.totalInterestPaid;
  const paidPrincipal = totalPrincipal * (paidPercentage / 100);
  const principalPercentage = Math.round((paidPrincipal / totalPrincipal) * 100);

  const paidInterest = emiData.totalInterestPaid * (paidPercentage / 100);
  const interestPercentage = Math.round((paidInterest / emiData.totalInterestPaid) * 100);

  return (
    <Card title="My EMI Dashboard" style={{ height: '100%' }}>
      <p className={styles.loanrstatus}>EMI Payment Status</p>
      <div className={styles.appcharts}>
        <div className={styles.chart1}>
          <h5 className={styles.chartpercentage}>{principalPercentage}%</h5>
          <ResponsiveContainer width="100%" height={100}>
            <PieChart>
              <Pie data={[{ value: principalPercentage }, { value: 100 - principalPercentage }]} cx="50%" cy="50%" innerRadius={30} outerRadius={40} dataKey="value">
                <Cell fill="#4CAF50" />
                <Cell fill="#EDF1F5" />
              </Pie>
            </PieChart>
          </ResponsiveContainer>
          <p className={styles.apploanname}>Principal Paid</p>
          <p className={styles.apploanamount}>₹{paidPrincipal.toLocaleString(undefined, {maximumFractionDigits: 0})}</p>
          <p className={styles.balancetenure}>₹{(totalPrincipal - paidPrincipal).toLocaleString(undefined, {maximumFractionDigits: 0})} remaining</p>
        </div>

        <div className={styles.chart2}>
          <h5 className={styles.chartpercentage}>{interestPercentage}%</h5>
          <ResponsiveContainer width="100%" height={100}>
            <PieChart>
              <Pie data={[{ value: interestPercentage }, { value: 100 - interestPercentage }]} cx="50%" cy="50%" innerRadius={30} outerRadius={40} dataKey="value">
                <Cell fill="#F37E20" />
                <Cell fill="#EDF1F5" />
              </Pie>
            </PieChart>
          </ResponsiveContainer>
          <p className={styles.apploanname}>Interest Paid</p>
          <p className={styles.apploanamount}>₹{paidInterest.toLocaleString(undefined, {maximumFractionDigits: 0})}</p>
          <p className={styles.balancetenure}>₹{(emiData.totalInterestPaid - paidInterest).toLocaleString(undefined, {maximumFractionDigits: 0})} remaining</p>
        </div>

        <div className={styles.chart2}>
          <h5 className={styles.chartpercentage}>{paidPercentage}%</h5>
          <ResponsiveContainer width="100%" height={100}>
            <PieChart>
              <Pie data={[{ value: paidPercentage }, { value: remainingPercentage }]} cx="50%" cy="50%" innerRadius={30} outerRadius={40} dataKey="value">
                <Cell fill="#2196F3" />
                <Cell fill="#EDF1F5" />
              </Pie>
            </PieChart>
          </ResponsiveContainer>
          <p className={styles.apploanname}>Remaining Amount</p>
          <p className={styles.apploanamount}>₹{emiData.remainingAmount.toLocaleString()}</p>
          <p className={styles.balancetenure}>{emiData.remainingInstallments} installments remaining</p>
        </div>
      </div>

      <div className={styles.paymentSection}>
        <Button 
          type="primary" 
          size="large"
          disabled={!emiData.canPayNow || paying}
          loading={paying}
          onClick={handlePayEMI}
          style={{ width: '100%', marginTop: '20px' }}
        >
          {emiData.canPayNow ? 'Pay This Month EMI' : 'Payment Not Due Yet'}
        </Button>
        <p style={{ textAlign: 'center', marginTop: '10px', color: '#626D8A', fontSize: '12px' }}>
          {emiData.paidInstallments} of {emiData.paidInstallments + emiData.remainingInstallments} EMIs paid
        </p>
      </div>
    </Card>
  );
};

export default ApplicationProfile;
