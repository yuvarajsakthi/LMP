import { useEffect } from "react";
import { Avatar, Dropdown, Space, Typography } from "antd";
import { UserOutlined, LogoutOutlined, SettingOutlined } from "@ant-design/icons";
import { useAuth } from "../../context";
import { useNavigate } from "react-router-dom";
import { COMMON_ROUTES, CUSTOMER_ROUTES } from "../../config";
import { NotificationModal } from "../../components";
import { useAppDispatch, useAppSelector } from "../../hooks";
import { fetchCustomerById } from "../../store";
import styles from "./Navbar.module.css";

const { Text } = Typography;

const Navbar = () => {
  const { token, logout } = useAuth();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { currentCustomer } = useAppSelector((state) => state.customer);

  const handleLogout = () => {
    logout();
    navigate(COMMON_ROUTES.LOGIN);
  };

  const isManager = token?.role?.toLowerCase() === 'manager';

  const customerMenuItems = [
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: 'Settings',
      onClick: () => navigate(CUSTOMER_ROUTES.SETTINGS)
    },
    {
      type: 'divider' as const
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Logout',
      onClick: handleLogout
    }
  ];

  const managerMenuItems = [
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Logout',
      onClick: handleLogout
    }
  ];

  const userMenuItems = isManager ? managerMenuItems : customerMenuItems;

  useEffect(() => {
    if (!isManager && token?.CustomerId && !currentCustomer) {
      dispatch(fetchCustomerById(parseInt(token.CustomerId)));
    }
  }, [token?.CustomerId, isManager, dispatch, currentCustomer]);

  const profileImage = (currentCustomer?.profileImageBase64 || currentCustomer?.profileImage)
    ? `data:image/jpeg;base64,${currentCustomer.profileImageBase64 || currentCustomer.profileImage}` 
    : null;

  return (
    <div className={styles.navbar}>
      <div className={styles.navRight}>
        <NotificationModal />
        <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
          <Space className={styles.userSection}>
            <Avatar 
              src={profileImage || undefined} 
              icon={<UserOutlined />} 
              className={styles.avatar} 
            />
            <Text className={styles.userName}>{token?.FullName || token?.name || token?.username || token?.email || 'User'}</Text>
          </Space>
        </Dropdown>
      </div>
    </div>
  );
};

export default Navbar;