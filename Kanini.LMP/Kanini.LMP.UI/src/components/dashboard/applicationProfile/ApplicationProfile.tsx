import React from 'react';
import styles from './ApplicationProfile.module.css';
import { Card } from 'antd';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';

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

const ApplicationProfile: React.FC<ApplicationProfileProps> = ({ 
  loans = [
    { name: 'Car loan', amount: 25540, percentage: 25, remainingYears: 4, color: '#FBB851' },
    { name: 'Personal loan', amount: 9540, percentage: 60, remainingYears: 2, color: '#F37E20' }
  ]
}) => {

  return (
    <div className={styles.box5}>
      <Card
        title="My Application Profile"
        style={{
          width: '100%',
          height: '100%',
        }}
      >
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
    </div>
  );
};

export default ApplicationProfile;
