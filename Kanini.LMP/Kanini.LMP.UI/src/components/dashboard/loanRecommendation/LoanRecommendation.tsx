import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import styles from './LoanRecommendation.module.css';
import { Card, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { CUSTOMER_ROUTES } from '../../../config';
import EligibilityModal from '../../eligibilityModal/EligibilityModal';
import { fetchLoanProducts } from '../../../store';
import type { RootState, AppDispatch } from '../../../store';

interface LoanRecommendationProps {
  title?: string;
  description?: string;
  eligibilityText?: string;
  buttonText?: string;
  onApplyClick?: () => void;
}

const LoanRecommendation: React.FC<LoanRecommendationProps> = ({
  title = "What Loans are you looking for?",
  description = "Explore our wide range of loan products tailored to meet your financial needs. Get competitive interest rates, flexible repayment options, and quick approval process.",
  eligibilityText = "click here to check your loan eligibility status",
  buttonText = "APPLY NOW",
  onApplyClick
}) => {
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const loanProductsFromStore = useSelector((state: RootState) => state.dashboard.loanProducts);
  const [loanProducts, setLoanProducts] = useState<any[]>([]);
  const [showEligibilityModal, setShowEligibilityModal] = useState(false);

  useEffect(() => {
    if (loanProductsFromStore.length === 0) {
      dispatch(fetchLoanProducts());
    } else {
      setLoanProducts(loanProductsFromStore);
    }
  }, []);

  const handleApplyClick = () => {
    if (onApplyClick) {
      onApplyClick();
    } else {
      navigate(CUSTOMER_ROUTES.LOAN_TYPES);
    }
  };

  return (
    <Card title={title} style={{ height: '100%' }}>
      <p className={styles.para}>{description}</p>
      <p className={styles.para2}>
        <a href="#" onClick={(e) => { e.preventDefault(); setShowEligibilityModal(true); }}>{eligibilityText}</a>
      </p>
      <Button type="primary" className={styles.applynowbutton} onClick={handleApplyClick} block>
        {buttonText}
      </Button>
      <EligibilityModal visible={showEligibilityModal} onClose={() => setShowEligibilityModal(false)} />
    </Card>
  );
};

export default LoanRecommendation;