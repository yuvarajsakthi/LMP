import React, { useState, useEffect } from 'react';
import { Card, Button, message } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { useForm } from 'react-hook-form';
import { motion } from 'framer-motion';
import Layout from '../../../layout/Layout';
import { useAuth } from '../../../context';
import { customerAPI } from '../../../services/api/customerAPI';
import styles from './Settings.module.css';

interface FormData {
  fullName: string;
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
}

const Settings: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>();
  const { token } = useAuth();

  useEffect(() => {
    fetchUserData();
  }, []);

  const fetchUserData = async () => {
    try {
      const userIdClaim = token?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || token?.nameid;
      
      if (!userIdClaim) {
        message.error('User not authenticated');
        return;
      }

      const data = await customerAPI.getCustomerSettings(parseInt(userIdClaim));
      setValue('fullName', data.fullName);
      setValue('phoneNumber', data.phoneNumber);
      setValue('occupation', data.occupation);
      setValue('annualIncome', data.annualIncome);
    } catch (error) {
      message.error('Failed to fetch user data');
    }
  };

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const userIdClaim = token?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || token?.nameid;
      
      if (!userIdClaim) {
        message.error('User not authenticated');
        return;
      }

      await customerAPI.updateCustomerSettings(parseInt(userIdClaim), data);
      message.success('Profile updated successfully');
    } catch (error) {
      message.error('Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <motion.div 
        className={styles.container}
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <h1 className={styles.title}>Settings</h1>
        
        <motion.div
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.3, delay: 0.2 }}
        >
          <Card title={<><UserOutlined /> Profile Settings</>} className={styles.card}>
            <form onSubmit={handleSubmit(onSubmit)}>
              <div className={styles.formGroup}>
                <label>Full Name</label>
                <input
                  {...register('fullName', { required: 'Full name is required' })}
                  placeholder="Enter your full name"
                  className={styles.input}
                />
                {errors.fullName && <span className={styles.error}>{errors.fullName.message}</span>}
              </div>
              
              <div className={styles.formGroup}>
                <label>Phone Number</label>
                <input
                  {...register('phoneNumber', { required: 'Phone number is required' })}
                  placeholder="Enter your phone number"
                  className={styles.input}
                />
                {errors.phoneNumber && <span className={styles.error}>{errors.phoneNumber.message}</span>}
              </div>
              
              <div className={styles.formGroup}>
                <label>Occupation</label>
                <input
                  {...register('occupation', { required: 'Occupation is required' })}
                  placeholder="Enter your occupation"
                  className={styles.input}
                />
                {errors.occupation && <span className={styles.error}>{errors.occupation.message}</span>}
              </div>
              
              <div className={styles.formGroup}>
                <label>Annual Income</label>
                <input
                  {...register('annualIncome', { required: 'Annual income is required', min: 0 })}
                  type="number"
                  placeholder="Enter your annual income"
                  className={styles.input}
                />
                {errors.annualIncome && <span className={styles.error}>{errors.annualIncome.message}</span>}
              </div>
              
              <Button type="primary" htmlType="submit" loading={loading}>
                Update Profile
              </Button>
            </form>
          </Card>
        </motion.div>
      </motion.div>
    </Layout>
  );
};

export default Settings;