import { Collapse, Card } from 'antd';
import { QuestionCircleOutlined } from '@ant-design/icons';
import Layout from '../../../layout/Layout';

const FAQ = () => {
  const faqData = [
    {
      key: '1',
      question: 'What documents are required for loan application?',
      answer: 'You need PAN card, Aadhaar card, salary slips (last 3 months), bank statements (last 6 months), and employment certificate.'
    },
    {
      key: '2',
      question: 'How long does loan approval take?',
      answer: 'Loan approval typically takes 2-7 business days depending on document verification and eligibility check.'
    },
    {
      key: '3',
      question: 'What is the minimum credit score required?',
      answer: 'Minimum credit score of 550 is required for personal loans, 650 for home loans, and 600 for vehicle loans.'
    },
    {
      key: '4',
      question: 'Can I prepay my loan without penalty?',
      answer: 'Yes, you can prepay your loan after 12 months without any penalty charges. Early prepayment helps reduce interest burden.'
    },
    {
      key: '5',
      question: 'What happens if I miss an EMI payment?',
      answer: 'Missing EMI payments attracts late payment charges and affects your credit score. Contact us immediately if you face payment difficulties.'
    }
  ];

  return (
    <Layout>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        <Card>
          <div style={{ textAlign: 'center', marginBottom: '32px' }}>
            <QuestionCircleOutlined style={{ fontSize: '48px', color: '#1890ff', marginBottom: '16px' }} />
            <h1>Frequently Asked Questions</h1>
            <p>Find answers to common questions about our loan services</p>
          </div>
          
          <Collapse 
            accordion
            items={faqData.map(item => ({
              key: item.key,
              label: item.question,
              children: <p>{item.answer}</p>
            }))}
          />
        </Card>
      </div>
    </Layout>
  );
};

export default FAQ;