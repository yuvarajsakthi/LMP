import { useCallback } from 'react';
import { Menu } from 'antd';
import {
    DashboardOutlined,
    FileTextOutlined,
    CheckCircleOutlined,
    CalculatorOutlined,
    QuestionCircleOutlined,
    SettingOutlined,
    LogoutOutlined
} from '@ant-design/icons';
import styles from './Sidebar.module.css';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context';
import { ROUTES } from '../../config';

interface SidebarProps {
  collapsed?: boolean;
}

const Sidebar = ({ collapsed = false }: SidebarProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const { logout } = useAuth();

    const handleLogout = useCallback(() => {
        logout();
        navigate(ROUTES.LOGIN);
    }, [logout, navigate]);

    const handleNavigation = useCallback((path: string) => () => navigate(path), [navigate]);

    const menuItems = [
        { key: ROUTES.CUSTOMER_DASHBOARD, icon: <DashboardOutlined />, label: 'Dashboard', onClick: handleNavigation(ROUTES.CUSTOMER_DASHBOARD) },
        { key: ROUTES.LOAN_TYPES, icon: <FileTextOutlined />, label: 'Apply Loan', onClick: handleNavigation(ROUTES.LOAN_TYPES) },
        { key: ROUTES.INTEGRATION, icon: <CheckCircleOutlined />, label: 'View Status', onClick: handleNavigation(ROUTES.INTEGRATION) },
        { key: ROUTES.EMI_CALCULATOR, icon: <CalculatorOutlined />, label: 'EMI Calculator', onClick: handleNavigation(ROUTES.EMI_CALCULATOR) },
        { key: ROUTES.FAQ, icon: <QuestionCircleOutlined />, label: "FAQ's", onClick: handleNavigation(ROUTES.FAQ) },
        { key: ROUTES.SETTINGS, icon: <SettingOutlined />, label: 'Settings', onClick: handleNavigation(ROUTES.SETTINGS) }
    ];

    return (
        <div className={`${styles.sidebarContainer} ${collapsed ? styles.collapsed : ''}`}>
            <div className={styles.sidebarDivider} />

            <Menu
                mode="inline"
                selectedKeys={[location.pathname]}
                className={styles.sidebarMenu}
                inlineCollapsed={collapsed}
                items={menuItems}
            />

            <div className={styles.sidebarFooter}>
                <Menu
                    mode="inline"
                    className={styles.sidebarMenu}
                    items={[
                        {
                            key: 'logout',
                            icon: <LogoutOutlined />,
                            label: 'Logout',
                            onClick: handleLogout,
                            className: styles.logoutItem
                        }
                    ]}
                />
            </div>
        </div>
    );
};

export default Sidebar;
