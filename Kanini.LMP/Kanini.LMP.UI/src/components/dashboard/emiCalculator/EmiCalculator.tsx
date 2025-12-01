import React, { useEffect, useState } from 'react';
import styles from './EmiCalculator.module.css';
import { PieChart, Pie, Cell, ResponsiveContainer } from 'recharts';
import { Card, Select, Input } from 'antd';

interface LoanTypeRates {
  [key: string]: number;
}

interface EmiCalculatorProps {
  defaultLoanAmount?: number;
  defaultTenure?: number;
  loanTypes?: LoanTypeRates;
}

const EmiCalculator: React.FC<EmiCalculatorProps> = ({
  defaultLoanAmount = 10000,
  defaultTenure = 3,
  loanTypes
}) => {
  const [loanAmount, setLoanAmount] = useState(defaultLoanAmount);
  const [interestRate, setInterestRate] = useState(3.5);
  const [tenure, setTenure] = useState(defaultTenure);
  const [emiAmount, setEmiAmount] = useState(0);
  const [totalRepayable, setTotalRepayable] = useState(0);
  const [totalInterest, setTotalInterest] = useState(0);
  const [selectedLoanType, setSelectedLoanType] = useState('Personal loan');

  const loanTypeInterestRates: LoanTypeRates = loanTypes || {
    'Personal loan': 3.5,
    'Consumer loan': 8,
    'Vehicle loan': 7,
    'House loan': 6,
    'Medical Loan': 12,
    'Debt consolidation Loan': 9,
    'Small business Loan': 10,
    'Credit card Loan': 12,
    'Education Loan': 7
  };

  const chartData = [
    { name: 'Interest', value: parseFloat(totalInterest.toString()) },
    { name: 'Principal', value: loanAmount }
  ];
  
  const colors = ['#e74c3c', '#f1c40f'];


  useEffect(() => {
    calculateEmi();
  }, [loanAmount, interestRate, tenure]);

  const calculateEmi = () => {
    const principle = loanAmount;
    const rate = interestRate / 100 / 12;
    const timeInMonths = tenure * 12;

    const emi = (principle * rate * Math.pow(1 + rate, timeInMonths)) / (Math.pow(1 + rate, timeInMonths) - 1);
    const totalAmountPayable = emi * timeInMonths;
    const totalInterestPayable = totalAmountPayable - principle;

    setEmiAmount(isNaN(emi) ? 0 : parseFloat(emi.toFixed(2)));
    setTotalRepayable(isNaN(totalAmountPayable) ? 0 : parseFloat(totalAmountPayable.toFixed(2)));
    setTotalInterest(isNaN(totalInterestPayable) ? 0 : parseFloat(totalInterestPayable.toFixed(2)));
  };

  const handleLoanTypeChange = (value: string) => {
    setSelectedLoanType(value);
    setInterestRate(loanTypeInterestRates[value] || 3.5);
  };


  return (
    <div className={styles.box2}>
      <Card
        title={
          <div className={styles.headcal}>
            <span className={styles.emititle}>EMI Calculator</span>
            <div className={styles.legend}>
              <div className={styles.legendItem}>
                <div className={styles.yellowDot}></div>
                <span>Principal</span>
              </div>
              <div className={styles.legendItem}>
                <div className={styles.redDot}></div>
                <span>Interest</span>
              </div>
            </div>
          </div>
        }
        style={{
          width: '100%', height: '100% '
        }} >
        <div className={styles.align}>
          <div className={styles.inputSection}>
            <Select
              value={selectedLoanType}
              onChange={handleLoanTypeChange}
              className={styles.select1}
              options={Object.keys(loanTypeInterestRates).map(type => ({
                label: type,
                value: type
              }))}
            />
            
            <Input
              type="number"
              value={loanAmount}
              placeholder="Enter Amount"
              onChange={(e) => setLoanAmount(Number(e.target.value))}
              className={styles.inputGroup}
            />
            
            <div className={styles.rateandtime}>
              <Input
                type="number"
                value={tenure}
                placeholder="Tenure in years"
                onChange={(e) => setTenure(Number(e.target.value))}
              />
              
              <Input
                type="number"
                value={interestRate}
                placeholder="Interest Rate"
                onChange={(e) => setInterestRate(Number(e.target.value))}
                suffix="%"
              />
            </div>
          </div>
          
          <div className={styles.chartSection}>
            <ResponsiveContainer width="100%" height={200}>
              <PieChart>
                <Pie
                  data={chartData}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={80}
                  dataKey="value"
                >
                  {chartData.map((_, index) => (
                    <Cell key={`cell-${index}`} fill={colors[index]} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
            <div className={styles.emiChartText}>
              <p className={styles.emiLabel}>EMI</p>
              <p className={styles.emiAmount}>₹{emiAmount} <span>/month</span></p>
            </div>
          </div>
        </div>
        <div className={styles.amountdisplay}>
          <div className={styles.emiLabel123}>
            <p>EMI Amount</p>
            <p>₹{emiAmount}</p>
          </div>
          <div className={styles.trp}>
            <p>Total Repayable</p>
            <p>₹{totalRepayable}</p>
          </div>
        </div>
      </Card>

    </div>
  );
};

export default EmiCalculator;

