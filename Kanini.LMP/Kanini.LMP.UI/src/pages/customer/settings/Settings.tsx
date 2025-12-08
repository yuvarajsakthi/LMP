import React, { useState, useEffect } from 'react';
import { Card, Button, message } from 'antd';
import { UserOutlined, UploadOutlined } from '@ant-design/icons';
import { useForm } from 'react-hook-form';
import { useDropzone } from 'react-dropzone';
import { motion } from 'framer-motion';
import Layout from '../../../layout/Layout';
import { useAuth } from '../../../context';
import { customerAPI } from '../../../services/api/customerAPI';
import styles from './Settings.module.css';

interface FormData {
  fullName: string;
  email: string;
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
  homeOwnershipStatus?: string;
  aadhaarNumber?: string;
  panNumber?: string;
}

const Settings: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [profileImage, setProfileImage] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>();
  const { token } = useAuth();

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: { 'image/*': ['.png', '.jpg', '.jpeg'] },
    maxFiles: 1,
    onDrop: (acceptedFiles) => {
      const file = acceptedFiles[0];
      if (file) {
        setProfileImage(file);
        setPreview(URL.createObjectURL(file));
      }
    }
  });

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
      setValue('email', data.email);
      setValue('phoneNumber', data.phoneNumber);
      setValue('occupation', data.occupation);
      setValue('annualIncome', data.annualIncome);
      if (data.homeOwnershipStatus) setValue('homeOwnershipStatus', data.homeOwnershipStatus);
      if (data.aadhaarNumber) setValue('aadhaarNumber', data.aadhaarNumber);
      if (data.panNumber) setValue('panNumber', data.panNumber);
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

      await customerAPI.updateCustomerSettings(
        parseInt(userIdClaim), 
        { 
          phoneNumber: data.phoneNumber, 
          occupation: data.occupation, 
          annualIncome: data.annualIncome,
          homeOwnershipStatus: data.homeOwnershipStatus,
          aadhaarNumber: data.aadhaarNumber,
          panNumber: data.panNumber
        },
        profileImage || undefined
      );
      message.success('Profile updated successfully');
      await fetchUserData();
    } catch (error: any) {
      message.error(error.message || 'Failed to update profile');
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
                <label>Profile Image</label>
                <div {...getRootProps()} className={`${styles.dropzone} ${isDragActive ? styles.dropzoneActive : ''}`}>
                  <input {...getInputProps()} />
                  {preview ? (
                    <div className={styles.preview}>
                      <img src={preview} alt="Preview" className={styles.previewImage} />
                      <p>Drop a new image or click to change</p>
                    </div>
                  ) : (
                    <div className={styles.dropzoneContent}>
                      <UploadOutlined style={{ fontSize: 48, color: '#999' }} />
                      <p>{isDragActive ? 'Drop the image here' : 'Drag & drop an image, or click to select'}</p>
                    </div>
                  )}
                </div>
              </div>

              <div className={styles.formGroup}>
                <label>Full Name</label>
                <input
                  {...register('fullName')}
                  placeholder="Enter your full name"
                  className={styles.input}
                  disabled
                />
              </div>

              <div className={styles.formGroup}>
                <label>Email</label>
                <input
                  {...register('email')}
                  placeholder="Enter your email"
                  className={styles.input}
                  disabled
                />
              </div>

              <div className={styles.formGroup}>
                <label>Phone Number</label>
                <input
                  {...register('phoneNumber', { 
                    required: 'Phone number is required',
                    pattern: {
                      value: /^[6-9]\d{9}$/,
                      message: 'Invalid phone number (10 digits starting with 6-9)'
                    }
                  })}
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
                <label>Annual Income (₹)</label>
                <input
                  {...register('annualIncome', { 
                    required: 'Annual income is required',
                    min: { value: 0, message: 'Income must be positive' },
                    valueAsNumber: true
                  })}
                  type="number"
                  placeholder="Enter your annual income"
                  className={styles.input}
                />
                {errors.annualIncome && <span className={styles.error}>{errors.annualIncome.message}</span>}
              </div>

              <div className={styles.formGroup}>
                <label>Home Ownership Status (Optional)</label>
                <select
                  {...register('homeOwnershipStatus')}
                  className={styles.input}
                >
                  <option value="">Select status</option>
                  <option value="Rented">Rented</option>
                  <option value="Owned">Owned</option>
                  <option value="Mortage">Mortage</option>
                </select>
              </div>

              <div className={styles.formGroup}>
                <label>Aadhaar Number (Optional)</label>
                <input
                  {...register('aadhaarNumber', {
                    pattern: {
                      value: /^\d{12}$/,
                      message: 'Aadhaar must be 12 digits'
                    }
                  })}
                  placeholder="Enter your Aadhaar number"
                  className={styles.input}
                />
                {errors.aadhaarNumber && <span className={styles.error}>{errors.aadhaarNumber.message}</span>}
              </div>

              <div className={styles.formGroup}>
                <label>PAN Number (Optional)</label>
                <input
                  {...register('panNumber', {
                    pattern: {
                      value: /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/,
                      message: 'Invalid PAN format (e.g., ABCDE1234F)'
                    }
                  })}
                  placeholder="Enter your PAN number"
                  className={styles.input}
                  style={{ textTransform: 'uppercase' }}
                />
                {errors.panNumber && <span className={styles.error}>{errors.panNumber.message}</span>}
              </div>
              
              <Button type="primary" htmlType="submit" loading={loading} size="large" block>
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
