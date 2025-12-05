import React from 'react';
import { useParams } from 'react-router-dom';
import Layout from '../../../layout/Layout';
import ApprovalManager from '../../../components/approvalManager/ApprovalManager';

const LoanDetails: React.FC = () => {
  const { applicationNumber } = useParams();

  return (
    <Layout>
      <ApprovalManager applicationNumber={applicationNumber} />
    </Layout>
  );
};

export default LoanDetails;
