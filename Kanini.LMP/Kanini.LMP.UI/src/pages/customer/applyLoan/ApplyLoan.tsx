import React, { useState, useEffect } from 'react';
import { Button, Typography, Spin } from 'antd';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import {
  MedicineBoxOutlined,
  ReadOutlined,
  DollarOutlined,
  ShopOutlined,
  CreditCardOutlined,
  CarOutlined,
  UserOutlined,
  HomeOutlined
} from '@ant-design/icons';
import { HiLockClosed } from 'react-icons/hi';
import { HiDocumentText } from 'react-icons/hi2';
import Layout from '../../../layout/Layout';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { fetchLoanCategories } from '../../../store/slices/loanApplicationSlice';
import styles from './ApplyLoan.module.css';

const { Title, Text } = Typography;

// Map loan category names to icons
const getIconForCategory = (categoryName: string): React.ReactNode => {
  const name = categoryName.toLowerCase();
  if (name.includes('medical')) return <MedicineBoxOutlined />;
  if (name.includes('education')) return <ReadOutlined />;
  if (name.includes('debt') || name.includes('consolidation')) return <DollarOutlined />;
  if (name.includes('business')) return <ShopOutlined />;
  if (name.includes('credit')) return <CreditCardOutlined />;
  if (name.includes('vehicle') || name.includes('car')) return <CarOutlined />;
  if (name.includes('consumer') || name.includes('personal')) return <UserOutlined />;
  if (name.includes('housing') || name.includes('home')) return <HomeOutlined />;
  return <DollarOutlined />;
};

const ApplyLoan: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { loanCategories, loading } = useAppSelector((state) => state.loanApplication);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);

  useEffect(() => {
    dispatch(fetchLoanCategories());
  }, [dispatch]);

  const handleCategorySelect = (categoryId: number) => {
    setSelectedCategory(categoryId);
  };

  const handleNext = () => {
    if (selectedCategory) {
      const category = loanCategories.find((c: any) => c.loanProductId === selectedCategory);
      navigate('/loan-application-form', { state: { selectedCategory: category } });
    }
  };

  return (
    <Layout>
      <div className={styles.container}>
        <div className={styles.contentWrapper}>
          {/* Left Sidebar - Intro */}
          <motion.div
            className={styles.sidebar}
            initial={{ opacity: 0, x: -50 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.6 }}
          >
            <div className={styles.sidebarContent}>
              <div className={styles.lockIcon}>
                <HiLockClosed />
              </div>

              <Title level={2} className={styles.sidebarTitle}>
                A few clicks away from applying your loan.
              </Title>

              <Text className={styles.sidebarSubtitle}>
                Avail quick & easy Personal Loans from the comfort of your home.
                Apply online & get instant approval.
              </Text>

              <div className={styles.documentIcon}>
                <HiDocumentText />
              </div>
            </div>
          </motion.div>

          {/* Right Section - Category Selection */}
          <motion.div
            className={styles.mainContent}
            initial={{ opacity: 0, x: 50 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.6 }}
          >
            <div className={styles.header}>
              <Title level={3} className={styles.mainTitle}>
                Select loan category
              </Title>
              <Text className={styles.subtitle}>
                Applying your loan is just a few steps away. Select any one of the loan type to continue.
              </Text>
            </div>

            {loading ? (
              <div className={styles.loadingContainer}>
                <Spin size="large" />
                <Text className={styles.loadingText}>Loading loan categories...</Text>
              </div>
            ) : (
              <>
                <div className={styles.categoryGrid}>
                  {loanCategories.map((category, index) => (
                    <motion.div
                      key={category.loanProductId}
                      initial={{ opacity: 0, y: 20 }}
                      animate={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.4, delay: index * 0.05 }}
                    >
                      <div
                        className={`${styles.categoryCard} ${selectedCategory === category.loanProductId ? styles.selected : ''
                          }`}
                        onClick={() => handleCategorySelect(category.loanProductId)}
                      >
                        <div className={styles.categoryIcon}>
                          {getIconForCategory(category.loanProductName)}
                        </div>
                        <Text className={styles.categoryName}>
                          {category.loanProductName}
                        </Text>
                      </div>
                    </motion.div>
                  ))}
                </div>

                <div className={styles.footer}>
                  <Button
                    type="primary"
                    size="large"
                    className={styles.nextButton}
                    onClick={handleNext}
                    disabled={!selectedCategory}
                  >
                    NEXT →
                  </Button>
                </div>
              </>
            )}
          </motion.div>
        </div>
      </div>
    </Layout>
  );
};



export default ApplyLoan;