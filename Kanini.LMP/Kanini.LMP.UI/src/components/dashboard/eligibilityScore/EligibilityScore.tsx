import React, { useEffect } from "react";
import styles from './EligibilityScore.module.css';
import { Card } from "antd";
import { PieChart, Pie, Cell, ResponsiveContainer } from "recharts";
import { useAuth } from "../../../context";
import { useAppDispatch, useAppSelector } from "../../../hooks";
import { getEligibilityScore } from "../../../store/slices/eligibilitySlice";

interface EligibilityScoreProps {
  score?: number;
}

const EligibilityScore: React.FC<EligibilityScoreProps> = ({ score }) => {
  const { token } = useAuth();
  const dispatch = useAppDispatch();
  const { score: eligibilityData, loading } = useAppSelector((state) => state.eligibility);

  useEffect(() => {
    const customerId = token?.CustomerId || token?.customerId;
    if (customerId && !eligibilityData) {
      dispatch(getEligibilityScore(Number(customerId)));
    }
  }, []);

  const eScore = score ?? eligibilityData?.EligibilityScore ?? eligibilityData?.eligibilityScore ?? eligibilityData?.creditScore ?? token?.EligibilityScore ?? 0;
  const eligibilityPercentage = eScore / 900;

  const gaugeData = [
    { value: eligibilityPercentage * 100 },
    { value: (1 - eligibilityPercentage) * 100 }
  ];
  
  const getScoreColor = () => {
    if (eligibilityPercentage > 0.7) return "#30BF78";
    if (eligibilityPercentage > 0.4) return "#FAAD14";
    return "#F4664A";
  };

  const getCreditScoreLabel = () => {
    if (eligibilityPercentage > 0.7) return "Excellent";
    if (eligibilityPercentage > 0.4) return "Good";
    return "Poor";
  };

  const getCreditScoreClass = () => {
    if (eligibilityPercentage > 0.7) return styles.excellentColor;
    if (eligibilityPercentage > 0.4) return styles.goodColor;
    return styles.poorColor;
  };

  const getStatusColor = () => {
    const status = ((eligibilityData as any)?.status || eligibilityData?.Status || eligibilityData?.eligibilityStatus || '').toLowerCase();
    if (status.includes('highly') || status.includes('excellent')) return '#30BF78';
    if (status.includes('eligible') || status.includes('good')) return '#FAAD14';
    if (status.includes('not') || status.includes('poor')) return '#F4664A';
    return '#666';
  };

  return (
    <Card title="Eligibility Score" extra={<a href="#">Generate Report</a>} style={{ height: '100%' }}>
        <div className={styles.gauge}>
          <ResponsiveContainer width="100%" height={200}>
            <PieChart>
              <Pie
                data={gaugeData}
                cx="50%"
                cy="50%"
                startAngle={180}
                endAngle={0}
                innerRadius={60}
                outerRadius={80}
                dataKey="value"
              >
                <Cell fill={getScoreColor()} />
                <Cell fill="#f0f0f0" />
              </Pie>
            </PieChart>
          </ResponsiveContainer>
          <div className={styles.scoreText}>{loading ? '...' : eScore}</div>
        </div>
        {!loading && eligibilityData && (
          <p className={styles.csp1} style={{ textAlign: 'center', marginTop: '10px' }}>
            Status: <strong style={{ color: getStatusColor() }}>{(eligibilityData as any)?.status || eligibilityData?.Status || eligibilityData?.eligibilityStatus || 'Not Available'}</strong>
          </p>
        )}
        <br />
        <br />
        <div className={styles.scoreInfo}>
          <p className={styles.csp1}>
            Your Credit Score is &nbsp;
            <span className={`${styles.csp} ${getCreditScoreClass()}`}>
              {getCreditScoreLabel()}!
            </span>
          </p>
        </div>
      </Card>
  );
};

export default EligibilityScore;
