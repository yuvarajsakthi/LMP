import React from 'react';
import { motion } from 'framer-motion';
import Layout from '../../../layout/Layout';
import ApplicationStatus from '../../../components/dashboard/applicationStatus/ApplicationStatus';

const ViewStatus: React.FC = () => {
  return (
    <Layout>
      <motion.div
        initial={{ opacity: 0, x: -20 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.4 }}
      >
        <ApplicationStatus />
      </motion.div>
    </Layout>
  );
};

export default ViewStatus;