import React, { useState, useEffect } from 'react';
import { Card, Button, message } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { useForm } from 'react-hook-form';
import { motion } from 'framer-motion';
import Layout from '../../../layout/Layout';
import { useAuth } from '../../../context';
import axios from 'axios';
import styles from './Settings.module.css';

interface FormData {
  fullName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  customerId?: number;
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
      const userIdClaim = token?.sub || token?.nameid;
      const response = await axios.get(`https://localhost:7297/api/Customer/user/${userIdClaim}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (response.data.success) {
        const data = response.data.data;
        setValue('fullName', data.fullName);
        setValue('email', data.email);
        setValue('phoneNumber', data.phoneNumber);
        setValue('dateOfBirth', data.dateOfBirth);
        setValue('customerId', data.customerId);
      }
    } catch (error) {
      message.error('Failed to fetch user data');
    }
  };

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const userIdClaim = token?.sub || token?.nameid;
      const customerData = { ...data, userId: userIdClaim };
      await axios.put(`https://localhost:7297/api/Customer/${data.customerId}`, customerData, {
        headers: { Authorization: `Bearer ${token}` }
      });
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
                <label>Email</label>
                <input
                  {...register('email')}
                  placeholder="Enter your email"
                  disabled
                  className={styles.input}
                />
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
                <label>Date of Birth</label>
                <input
                  {...register('dateOfBirth')}
                  type="date"
                  className={styles.input}
                />
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