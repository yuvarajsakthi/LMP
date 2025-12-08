import React, { useState, useEffect } from 'react';
import { Collapse, Card, Button, Input, message, Tag } from 'antd';
import { QuestionCircleOutlined, PlusOutlined, CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';
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

const FAQ = () => {
  const { token } = useAuth();
  const [faqs, setFaqs] = useState<FAQ[]>([]);
  const [loading, setLoading] = useState(true);
  const [showNewQuestion, setShowNewQuestion] = useState(false);
  const [newQuestion, setNewQuestion] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    fetchFAQs();
  }, []);

  const fetchFAQs = async () => {
    try {
      const customerId = localStorage.getItem('customerId');
      const res = await axios.get(`/api/Faq/customer/${customerId}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      setFaqs(res.data?.data || []);
    } catch (error) {
      message.error('Failed to fetch FAQs');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmitQuestion = async () => {
    if (!newQuestion.trim()) {
      message.warning('Please enter your question');
      return;
    }

    setSubmitting(true);
    try {
      const customerId = localStorage.getItem('customerId');
      await axios.post('/api/Faq', {
        customerId: parseInt(customerId || '0'),
        question: newQuestion
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });
      message.success('Question submitted successfully');
      setNewQuestion('');
      setShowNewQuestion(false);
      fetchFAQs();
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
          
          <Collapse 
            accordion
            loading={loading}
            items={faqs.map(item => ({
              key: item.id.toString(),
              label: (
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <span>{item.question}</span>
                  {item.status === 0 ? (
                    <Tag icon={<ClockCircleOutlined />} color="warning">Pending</Tag>
                  ) : (
                    <Tag icon={<CheckCircleOutlined />} color="success">Answered</Tag>
                  )}
                </div>
              ),
              children: item.answer ? (
                <p style={{ color: '#52c41a', fontWeight: 500 }}>{item.answer}</p>
              ) : (
                <p style={{ color: '#999', fontStyle: 'italic' }}>Waiting for manager's response...</p>
              )
            }))}
          />
        </Card>
      </div>
    </Layout>
  );
};}

export default FAQ;