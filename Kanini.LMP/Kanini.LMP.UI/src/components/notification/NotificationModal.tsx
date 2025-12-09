import { Modal, List, Badge, Empty, Button } from 'antd';
import { BellOutlined, DeleteOutlined } from '@ant-design/icons';
import { useState, useEffect } from 'react';
import { useAuth } from '../../context';
import { useAppDispatch, useAppSelector } from '../../hooks';
import { getAllNotifications, deleteNotification } from '../../store';

interface Notification {
  notificationId: number;
  message: string;
  createdAt: string;
  isRead: boolean;
}

export const NotificationModal = () => {
  const [isOpen, setIsOpen] = useState(false);
  const { token } = useAuth();
  const dispatch = useAppDispatch();
  const { notifications, unreadCount, loading } = useAppSelector((state) => state.notification);

  useEffect(() => {
    if (isOpen && notifications.length === 0) {
      dispatch(getAllNotifications());
    }
  }, [isOpen, dispatch, notifications.length]);

  const handleDelete = (id: number) => {
    dispatch(deleteNotification(id));
  };

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
