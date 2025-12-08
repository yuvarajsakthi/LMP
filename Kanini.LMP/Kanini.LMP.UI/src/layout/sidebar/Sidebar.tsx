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
import { COMMON_ROUTES, CUSTOMER_ROUTES, MANAGER_ROUTES } from '../../config';

interface SidebarProps {
  collapsed?: boolean;
}

const Sidebar = ({ collapsed = false }: SidebarProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const { logout, token } = useAuth();

    const handleLogout = useCallback(() => {
        logout();
        navigate(COMMON_ROUTES.LOGIN);
    }, [logout, navigate]);

    const handleNavigation = useCallback((path: string) => () => navigate(path), [navigate]);

    const isManager = token?.role?.toLowerCase() === 'manager';

    const customerMenuItems = [
        { key: CUSTOMER_ROUTES.CUSTOMER_DASHBOARD, icon: <DashboardOutlined />, label: 'Dashboard', onClick: handleNavigation(CUSTOMER_ROUTES.CUSTOMER_DASHBOARD) },
        { key: CUSTOMER_ROUTES.LOAN_TYPES, icon: <FileTextOutlined />, label: 'Apply Loan', onClick: handleNavigation(CUSTOMER_ROUTES.LOAN_TYPES) },
        { key: CUSTOMER_ROUTES.VIEWSTATUS, icon: <CheckCircleOutlined />, label: 'View Status', onClick: handleNavigation(CUSTOMER_ROUTES.VIEWSTATUS) },
        { key: CUSTOMER_ROUTES.EMI_CALCULATOR, icon: <CalculatorOutlined />, label: 'EMI Calculator', onClick: handleNavigation(CUSTOMER_ROUTES.EMI_CALCULATOR) },
        { key: CUSTOMER_ROUTES.FAQ, icon: <QuestionCircleOutlined />, label: "FAQ's", onClick: handleNavigation(CUSTOMER_ROUTES.FAQ) },
        { key: CUSTOMER_ROUTES.SETTINGS, icon: <SettingOutlined />, label: 'Settings', onClick: handleNavigation(CUSTOMER_ROUTES.SETTINGS) }
    ];

    const managerMenuItems = [
        { key: MANAGER_ROUTES.MANAGER_DASHBOARD, icon: <DashboardOutlined />, label: 'Dashboard', onClick: handleNavigation(MANAGER_ROUTES.MANAGER_DASHBOARD) },
        { key: MANAGER_ROUTES.APPLIED_LOAN, icon: <FileTextOutlined />, label: 'Applied Loans', onClick: handleNavigation(MANAGER_ROUTES.APPLIED_LOAN) },
        { key: MANAGER_ROUTES.MANAGER_FAQ, icon: <QuestionCircleOutlined />, label: "FAQ's", onClick: handleNavigation(MANAGER_ROUTES.MANAGER_FAQ) }
    ];

    const menuItems = isManager ? managerMenuItems : customerMenuItems;

    return (
        <div className={`${styles.sidebarContainer} ${collapsed ? styles.collapsed : ''}`}>
            <div className={styles.sidebarDivider} />

            <div style={{ flex: 1, overflow: 'auto' }}>
                <Menu
                    mode="inline"
                    selectedKeys={[location.pathname]}
                    className={styles.sidebarMenu}
                    inlineCollapsed={collapsed}
                    items={menuItems}
                />
            </div>

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
