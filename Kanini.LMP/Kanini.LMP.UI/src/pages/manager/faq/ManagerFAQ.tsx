import { useState, useEffect } from 'react';
import { Card, List, Input, Button, message, Tag, Space } from 'antd';
import { QuestionCircleOutlined, CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';
import Layout from '../../../layout/Layout';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { getAllFaqs, updateFaq } from '../../../store/slices/faqSlice';

const { TextArea } = Input; 

const ManagerFAQ = () => {
  const dispatch = useAppDispatch();
  const { faqs, loading } = useAppSelector((state) => state.faq);
  const [answeringId, setAnsweringId] = useState<number | null>(null);
  const [answerText, setAnswerText] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    dispatch(getAllFaqs()).then((result) => {
      console.log('FAQ Response:', result);
    });
  }, [dispatch]);

  const handleAnswerSubmit = async (faqId: number, customerId: number, question: string) => {
    if (!answerText.trim()) {
      message.warning('Please enter an answer');
      return;
    }

    setSubmitting(true);
    try {
      await dispatch(updateFaq({
        id: faqId,
        faq: {
          id: faqId,
          customerId,
          question,
          answer: answerText,
          status: '1'
        }
      })).unwrap();
      message.success('Answer submitted successfully');
      setAnsweringId(null);
      setAnswerText('');
    } catch (error) {
      message.error('Failed to submit answer');
    } finally {
      setSubmitting(false);
    }
  };

  const getStatusTag = (status: string | undefined) => {
    switch (status?.toLowerCase()) {
      case 'pending':
      case '0':
        return <Tag icon={<ClockCircleOutlined />} color="warning">Pending</Tag>;
      case 'answered':
      case '1':
        return <Tag icon={<CheckCircleOutlined />} color="success">Answered</Tag>;
      default:
        return <Tag color="default">{status || 'Unknown'}</Tag>;
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
            renderItem={(faq) => {
              console.log('FAQ Item:', faq);
              return (
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
                    {faq.customerName && <span style={{ color: '#999', fontSize: '12px' }}>Customer: {faq.customerName}</span>}
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
                        onClick={() => handleAnswerSubmit(faq.id!, faq.customerId!, faq.question)}
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
                    onClick={() => setAnsweringId(faq.id!)}
                  >
                    Answer Question
                  </Button>
                )}
              </List.Item>
            );
            }}
          />
        </Card>
      </div>
    </Layout>
  );
};

export default ManagerFAQ;
