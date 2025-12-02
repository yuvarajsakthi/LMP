import Layout from '../../../layout/Layout';
import EmiCalculator from '../../../components/dashboard/emiCalculator/EmiCalculator';

const EmiCalculatorPage = () => {
  return (
    <Layout>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        <h1 style={{ marginBottom: '24px', color: '#1890ff' }}>EMI Calculator</h1>
        <EmiCalculator />
      </div>
    </Layout>
  );
};

export default EmiCalculatorPage;