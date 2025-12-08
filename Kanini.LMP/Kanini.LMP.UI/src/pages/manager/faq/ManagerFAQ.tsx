import React, { useState, useEffect } from 'react';
import { Card, List, Input, Button, message, Tag, Space } from 'antd';
import { QuestionCircleOutlined, CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';
import Layout from '../../../layout/Layout';
import axios from 'axios';
import { useAuth } from '../../../context';

const { TextArea } = Input;

interface FAQ {
  id: number;
  customerId: number;
  question: string;
  answer: string | null;
  status: number;
}

const ManagerFAQ: React.FC = () => {
  const { token } = useAuth();
  const [faqs, setFaqs] = useState<FAQ[]>([]);
  const [loading, setLoading] = useState(true);
  const [answeringId, setAnsweringId] = useState<number | null>(null);
  const [answerText, setAnswerText] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    fetchFAQs();
  }, []);

  const fetchFAQs = async () => {
    try {
      const res = await axios.get('/api/Faq', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setFaqs(res.data?.data || []);
    } catch (error) {
      message.error('Failed to fetch FAQs');
    } finally {
      setLoading(false);
    }
  };

  const handleAnswerSubmit = async (faqId: number) => {
    if (!answerText.trim()) {
      message.warning('Please enter an answer');
      return;
    }

    setSubmitting(true);
    try {
      await axios.put(`/api/Faq/${faqId}`, {
        id: faqId,
        answer: answerText,
        status: 1
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });
      message.success('Answer submitted successfully');
      setAnsweringId(null);
      setAnswerText('');
      fetchFAQs();
    } catch (error) {
      message.error('Failed to submit answer');
    } finally {
      setSubmitting(false);
    }
  };

  const getStatusTag = (status: number) => {
    switch (status) {
      case 0:
        return <Tag icon={<ClockCircleOutlined />} color="warning">Pending</Tag>;
      case 1:
        return <Tag icon={<CheckCircleOutlined />} color="success">Answered</Tag>;
      default:
        return <Tag color="default">Unknown</Tag>;
    }
  };

  return (
    <Layout>
      <div style={{ maxWidth: '1000px', margin: '0 auto' }}>
        <Card>
          <div style={{ textAlign: 'center', marginBottom: '32px' }}>
            <QuestionCircleOutlined style={{ fontSize: '48px', color: '#1890ff', marginBottom: '16px' }} />
            <h1>Customer Questions</h1>
            <p>Answer customer questions to help them with their queries</p>
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
                    {getStatusTag(faq.status)}
                    <span style={{ color: '#999', fontSize: '12px' }}>Customer ID: {faq.customerId}</span>
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
                ) : answeringId === faq.id ? (
                  <div>
                    <TextArea
                      rows={4}
                      placeholder="Type your answer here..."
                      value={answerText}
                      onChange={(e) => setAnswerText(e.target.value)}
                      style={{ marginBottom: '8px' }}
                    />
                    <Space>
                      <Button
                        type="primary"
                        loading={submitting}
                        onClick={() => handleAnswerSubmit(faq.id)}
                      >
                        Submit Answer
                      </Button>
                      <Button onClick={() => {
                        setAnsweringId(null);
                        setAnswerText('');
                      }}>
                        Cancel
                      </Button>
                    </Space>
                  </div>
                ) : (
                  <Button
                    type="primary"
                    onClick={() => setAnsweringId(faq.id)}
                  >
                    Answer Question
                  </Button>
                )}
              </List.Item>
            )}
          />
        </Card>
      </div>
    </Layout>
  );
};

export default ManagerFAQ;
