import { useState, useEffect } from 'react';
import { Card, List, Button, Input, message, Tag, Space } from 'antd';
import { QuestionCircleOutlined, PlusOutlined, CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';
import Layout from '../../../layout/Layout';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { createFaq, getFaqsByCustomerId } from '../../../store/slices/faqSlice';
import { useAuth } from '../../../context';

const { TextArea } = Input;

const FAQ = () => {
  const dispatch = useAppDispatch();
  const { token } = useAuth();
  const { faqs, loading } = useAppSelector((state) => state.faq);
  const [showNewQuestion, setShowNewQuestion] = useState(false);
  const [newQuestion, setNewQuestion] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const customerId = token?.customerId || token?.CustomerId;
    if (customerId) {
      dispatch(getFaqsByCustomerId(parseInt(customerId)));
    }
  }, [dispatch, token]);

  const handleSubmitQuestion = async () => {
    if (!newQuestion.trim()) {
      message.warning('Please enter your question');
      return;
    }

    setSubmitting(true);
    try {
      const customerId = token?.customerId || token?.CustomerId;
      if (!customerId) {
        message.error('Customer ID not found');
        return;
      }
      await dispatch(createFaq({
        customerId: parseInt(customerId),
        question: newQuestion
      })).unwrap();
      message.success('Question submitted successfully');
      setNewQuestion('');
      setShowNewQuestion(false);
      dispatch(getFaqsByCustomerId(parseInt(customerId)));
    } catch (error) {
      message.error('Failed to submit question');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Layout>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        <Card>
          <div style={{ textAlign: 'center', marginBottom: '32px' }}>
            <QuestionCircleOutlined style={{ fontSize: '48px', color: '#1890ff', marginBottom: '16px' }} />
            <h1>Frequently Asked Questions</h1>
            <p>Find answers to common questions about our loan services</p>
          </div>

          <div style={{ marginBottom: '24px', textAlign: 'center' }}>
            {!showNewQuestion ? (
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => setShowNewQuestion(true)}
              >
                Ask New Question
              </Button>
            ) : (
              <div style={{ textAlign: 'left' }}>
                <TextArea
                  rows={4}
                  placeholder="Type your question here..."
                  value={newQuestion}
                  onChange={(e) => setNewQuestion(e.target.value)}
                  style={{ marginBottom: '8px' }}
                />
                <Button
                  type="primary"
                  loading={submitting}
                  onClick={handleSubmitQuestion}
                  style={{ marginRight: '8px' }}
                >
                  Submit Question
                </Button>
                <Button onClick={() => {
                  setShowNewQuestion(false);
                  setNewQuestion('');
                }}>
                  Cancel
                </Button>
              </div>
            )}
          </div>
          
          <List
            loading={loading}
            dataSource={faqs}
            renderItem={(faq) => (
              <List.Item
                key={faq.id}
                style={{
                  flexDirection: 'column',
                  alignItems: 'stretch',
                  padding: '20px',
                  border: '1px solid #f0f0f0',
                  borderRadius: '8px',
                  marginBottom: '16px'
                }}
              >
                <div style={{ marginBottom: '12px' }}>
                  <Space>
                    {(faq.status?.toLowerCase() === 'pending' || faq.status === '0' || !faq.answer) ? (
                      <Tag icon={<ClockCircleOutlined />} color="warning">Pending</Tag>
                    ) : (
                      <Tag icon={<CheckCircleOutlined />} color="success">Answered</Tag>
                    )}
                  </Space>
                </div>

                <div style={{ marginBottom: '12px' }}>
                  <strong style={{ fontSize: '16px', color: '#262626' }}>Q: {faq.question}</strong>
                </div>

                {faq.answer ? (
                  <div style={{ 
                    backgroundColor: '#f6ffed', 
                    padding: '12px', 
                    borderRadius: '4px',
                    borderLeft: '3px solid #52c41a'
                  }}>
                    <strong style={{ color: '#52c41a' }}>A: </strong>
                    <span>{faq.answer}</span>
                  </div>
                ) : (
                  <div style={{ 
                    backgroundColor: '#fff7e6', 
                    padding: '12px', 
                    borderRadius: '4px',
                    borderLeft: '3px solid #faad14'
                  }}>
                    <span style={{ color: '#999', fontStyle: 'italic' }}>Waiting for manager's response...</span>
                  </div>
                )}
              </List.Item>
            )}
          />
        </Card>
      </div>
    </Layout>
  );
};

export default FAQ;
