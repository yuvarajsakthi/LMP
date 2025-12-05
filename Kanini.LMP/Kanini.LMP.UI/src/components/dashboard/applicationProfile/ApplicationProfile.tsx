import React, { useEffect, useState } from 'react';
import styles from './ApplicationProfile.module.css';
import { Card } from 'antd';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';
import { customerDashboardAPI } from '../../../services/api/customerDashboardAPI';

interface LoanData {
  name: string;
  amount: number;
  percentage: number;
  remainingYears: number;
  color: string;
}

interface ApplicationProfileProps {
  loans?: LoanData[];
}

const ApplicationProfile: React.FC<ApplicationProfileProps> = ({ loans: providedLoans }) => {
  const [loans, setLoans] = useState<LoanData[]>(providedLoans || [
    { name: 'Car loan', amount: 25540, percentage: 25, remainingYears: 4, color: '#FBB851' },
    { name: 'Personal loan', amount: 9540, percentage: 60, remainingYears: 2, color: '#F37E20' }
  ]);

  useEffect(() => {
    if (providedLoans) return;
    const fetchRecentLoans = async () => {
      try {
        const recentLoans = await customerDashboardAPI.getRecentAppliedLoans();
        if (recentLoans && recentLoans.length > 0) {
          const colors = ['#FBB851', '#F37E20', '#4CAF50', '#2196F3'];
          const formattedLoans = recentLoans.slice(0, 4).map((loan: any, index: number) => ({
            name: loan.loanType || loan.loanProductName || 'Loan',
            amount: loan.loanAmount || loan.amount || 0,
            percentage: Math.round((loan.paidAmount || 0) / (loan.loanAmount || 1) * 100),
            remainingYears: Math.ceil((loan.tenure || 0) / 12),
            color: colors[index % colors.length]
          }));
          setLoans(formattedLoans);
        }
      } catch (error) {
        console.error('Failed to fetch recent loans:', error);
      }
    };
    fetchRecentLoans();
  }, [providedLoans]);

  return (
    <Card title="My Application Profile" style={{ height: '100%' }}>
        <p className={styles.loanrstatus}> Loan Repayment Status</p>


        <div className={styles.appcharts}>
          {loans.map((loan, index) => {
            const chartData = [{ value: loan.percentage }, { value: 100 - loan.percentage }];
            const colors = [loan.color, '#EDF1F5'];
            
            return (
              <div key={index} className={styles[`chart${index + 1}`] || styles.chart}>
                <h5 className={styles.chartpercentage}>{loan.percentage}%</h5>
                <ResponsiveContainer width="100%" height={100}>
                  <PieChart>
                    <Pie
                      data={chartData}
                      cx="50%"
                      cy="50%"
                      innerRadius={30}
                      outerRadius={40}
                      dataKey="value"
                    >
                      {chartData.map((_, cellIndex) => (
                        <Cell key={`cell-${cellIndex}`} fill={colors[cellIndex]} />
                      ))}
                    </Pie>
                  </PieChart>
                </ResponsiveContainer>
                <p className={styles.apploanname}>{loan.name}</p>
                <p className={styles.apploanamount}>₹{loan.amount.toLocaleString()}</p>
                <p className={styles.balancetenure}>{loan.remainingYears} years are remaining <br />to pay 100% loan</p>
              </div>
            );
          })}
        </div>
        <div className={styles.completereportlink}>
          <a href='#' onClick={(e) => e.preventDefault()}>Click here to view the complete Report</a>
        </div>

    </Card>
  );
};

export default ApplicationProfile;
