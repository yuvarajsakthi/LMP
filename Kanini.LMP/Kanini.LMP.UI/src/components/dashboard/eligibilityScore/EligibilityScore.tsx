import React, { useEffect, useState } from "react";
import styles from './EligibilityScore.module.css';
import { Card } from "antd";
import { PieChart, Pie, Cell, ResponsiveContainer } from "recharts";
import { useAuth } from "../../../context";
import { customerDashboardAPI } from "../../../services/api/customerDashboardAPI";

interface EligibilityScoreProps {
  score?: number;
}

const EligibilityScore: React.FC<EligibilityScoreProps> = ({ score }) => {
  const { token } = useAuth();
  const [eligibilityData, setEligibilityData] = useState<any>(null);

  useEffect(() => {
    const fetchEligibility = async () => {
      try {
        const data = await customerDashboardAPI.getEligibilityScore();
        setEligibilityData(data);
      } catch (error: any) {
        // Use default score if API fails
        setEligibilityData({ eligibilityScore: 750 });
      }
    };
    fetchEligibility();
  }, []);

  const eScore = score || eligibilityData?.eligibilityScore || token?.EligibilityScore || 250;
  const eligibilityPercentage = eScore / 1000;

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
          <div className={styles.scoreText}>{eScore}</div>
        </div>
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
