
import { useState, useEffect } from "react";
import { Row, Col, Card, Typography } from "antd";
import { motion } from "framer-motion";
import Layout from "../../../layout/Layout";
import EmiCalculator from "../../../components/dashboard/emiCalculator/EmiCalculator";
import LoanReco from "../../../components/dashboard/loanRecommendation/LoanRecommendation";
import EligibilityScore from "../../../components/dashboard/eligibilityScore/EligibilityScore";
import ApplicationStatus from "../../../components/dashboard/applicationStatus/ApplicationStatus";
import ApplicationProfile from "../../../components/dashboard/applicationProfile/ApplicationProfile";
import EligibilityModal from "../../../components/eligibilityModal/EligibilityModal";
import { useAuth } from "../../../context";
import styles from './CustomerDashboard.module.css';

const { Title, Text } = Typography;

const CustomerDashboard = () => {
  const { token } = useAuth();
  const [showEligibilityModal, setShowEligibilityModal] = useState(false);

  useEffect(() => {
    const hasShownModal = sessionStorage.getItem('eligibilityModalShown');
    if (!hasShownModal) {
      setShowEligibilityModal(true);
      sessionStorage.setItem('eligibilityModalShown', 'true');
    }
  }, []);

  return (
    <Layout>
      <div className={styles.dashboard}>
        <motion.div 
          className={styles.header}
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          <Title level={2} className={styles.welcome}>
            Hi {token?.FullName || token?.name || token?.username || token?.email || 'User'}!
          </Title>
          <Text className={styles.subtitle}>
            Manage your loans and explore new opportunities
          </Text>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.2 }}
        >
          <Row gutter={[20, 20]} className={styles.topRow}>
            <Col xs={24} sm={24} md={12} lg={8}>
              <Card className={styles.card} variant="borderless">
                <LoanReco />
              </Card>
            </Col>
            <Col xs={24} sm={24} md={12} lg={8}>
              <Card className={styles.card} variant="borderless">
                <EmiCalculator showChart={false} />
              </Card>
            </Col>
            <Col xs={24} sm={24} md={12} lg={8}>
              <Card className={styles.card} variant="borderless">
                <EligibilityScore />
              </Card>
            </Col>
          </Row>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.4 }}
        >
          <Row gutter={[20, 20]} className={styles.bottomRow}>
            <Col xs={24} lg={16}>
              <Card className={styles.card} variant="borderless">
                <ApplicationStatus />
              </Card>
            </Col>
            <Col xs={24} lg={8}>
              <Card className={styles.card} variant="borderless">
                <ApplicationProfile />
              </Card>
            </Col>
          </Row>
        </motion.div>
      </div>
      
      <EligibilityModal 
        visible={showEligibilityModal} 
        onClose={() => setShowEligibilityModal(false)} 
      />
    </Layout>
  );
};

export default CustomerDashboard;
