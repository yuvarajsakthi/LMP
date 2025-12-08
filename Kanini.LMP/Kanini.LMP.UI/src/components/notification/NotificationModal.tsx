import { Modal, List, Badge, Empty, Button } from 'antd';
import { BellOutlined, DeleteOutlined } from '@ant-design/icons';
import { useState, useEffect } from 'react';
import { notificationAPI } from '../../services';
import { useAuth } from '../../context';

interface Notification {
  notificationId: number;
  message: string;
  createdAt: string;
  isRead: boolean;
}

export const NotificationModal = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(false);
  const { token } = useAuth();

  const fetchNotifications = async () => {
    if (!token?.customerId) return;
    setLoading(true);
    try {
      const response = await notificationAPI.getAllNotifications(Number(token.customerId));
      setNotifications(response || []);
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isOpen) {
      fetchNotifications();
    }
  }, [isOpen]);

  const handleDelete = async (id: number) => {
    try {
      await notificationAPI.deleteNotification(id);
      setNotifications(prev => prev.filter(n => n.notificationId !== id));
    } catch (error) {
      console.error('Failed to delete notification:', error);
    }
  };

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <>
      <Badge count={unreadCount} offset={[-5, 5]}>
        <BellOutlined 
          style={{ fontSize: '20px', cursor: 'pointer', color: '#2C76C9' }}
          onClick={() => setIsOpen(true)}
        />
      </Badge>

      <Modal
        title="Notifications"
        open={isOpen}
        onCancel={() => setIsOpen(false)}
        footer={null}
        width={500}
      >
        {notifications.length === 0 ? (
          <Empty description="No notifications" />
        ) : (
          <List
            loading={loading}
            dataSource={notifications}
            renderItem={(item) => (
              <List.Item
                actions={[
                  <Button 
                    type="text" 
                    icon={<DeleteOutlined />} 
                    onClick={() => handleDelete(item.notificationId)}
                    danger
                  />
                ]}
                style={{ 
                  backgroundColor: item.isRead ? 'transparent' : '#f0f7ff',
                  padding: '12px',
                  borderRadius: '4px',
                  marginBottom: '8px'
                }}
              >
                <List.Item.Meta
                  title={item.message}
                  description={new Date(item.createdAt).toLocaleString()}
                />
              </List.Item>
            )}
          />
        )}
      </Modal>
    </>
  );
};

export default NotificationModal;
