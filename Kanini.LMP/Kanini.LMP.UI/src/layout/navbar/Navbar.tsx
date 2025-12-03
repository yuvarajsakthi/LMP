import { Avatar, Dropdown, Space, Typography } from "antd";
import { UserOutlined, BellOutlined, LogoutOutlined, SettingOutlined } from "@ant-design/icons";
import { useAuth } from "../../context";
import { useNavigate } from "react-router-dom";
import { COMMON_ROUTES, CUSTOMER_ROUTES } from "../../config";
import styles from "./Navbar.module.css";

const { Text } = Typography;

const Navbar = () => {
  const { token, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate(COMMON_ROUTES.LOGIN);
  };

  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Profile',
      onClick: () => navigate(CUSTOMER_ROUTES.SETTINGS)
    },
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

  return (
    <div className={styles.navbar}>
      <div className={styles.navRight}>
        <BellOutlined className={styles.notificationIcon} />
        <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
          <Space className={styles.userSection}>
            <Avatar icon={<UserOutlined />} className={styles.avatar} />
            <Text className={styles.userName}>{token?.FullName || token?.name || token?.username || token?.email || 'User'}</Text>
          </Space>
        </Dropdown>
      </div>
    </div>
  );
};

export default Navbar;