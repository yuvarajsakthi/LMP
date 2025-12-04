import React, { useEffect, useState } from 'react';
import styles from './LoanRecommendation.module.css';
import { Card, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { CUSTOMER_ROUTES } from '../../../config';
import { customerDashboardAPI } from '../../../services/api/customerDashboardAPI';

interface LoanRecommendationProps {
  title?: string;
  description?: string;
  eligibilityText?: string;
  buttonText?: string;
  onApplyClick?: () => void;
}

const LoanRecommendation: React.FC<LoanRecommendationProps> = ({
  title = "What Loans are you looking for?",
  description = "What's more, if you are eligible for a pre-approved offer, you can get instant loan sanction with no documentation. Added benefits include, attractive interest rate, low EMI and simplified loan application and disbursement process.",
  eligibilityText = "click here to check your loan eligibility status",
  buttonText = "APPLY NOW",
  onApplyClick
}) => {
  const navigate = useNavigate();
  const [loanProducts, setLoanProducts] = useState([]);

  useEffect(() => {
    const fetchLoanProducts = async () => {
      try {
        const products = await customerDashboardAPI.getLoanProducts();
        setLoanProducts(products);
      } catch (error: any) {
        // Silently handle error - component works without API data
      }
    };
    fetchLoanProducts();
  }, []);

  const handleApplyClick = () => {
    if (onApplyClick) {
      onApplyClick();
    } else {
      navigate(CUSTOMER_ROUTES.LOAN_TYPES);
    }
  };

  return (
    <div className={styles.box1}>
      <Card title={title}
        style={{
          width: '100%', height: '100%'
        }}
      >
        <p className={styles.para}>
          {description}
        </p>
        <div>
          <p className={styles.para2}>
            <a href="#" onClick={(e) => e.preventDefault()}>{eligibilityText}</a>
          </p>
        </div>
        <br />
        <div>
          <Button 
            type="primary" 
            className={styles.applynowbutton}
            onClick={handleApplyClick}
          >
            {buttonText}
          </Button>
        </div>
      </Card>
    </div>
  );
};

export default LoanRecommendation;