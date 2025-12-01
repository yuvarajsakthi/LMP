import { useState, useCallback } from 'react';
import { Menu, Drawer, Button } from 'antd';
import {
    DashboardOutlined,
    FileTextOutlined,
    CheckCircleOutlined,
    CalculatorOutlined,
    QuestionCircleOutlined,
    SettingOutlined,
    LogoutOutlined,
    MenuOutlined,
    CloseOutlined
} from '@ant-design/icons';
import styles from './Sidebar.module.css';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context';
import { ROUTES } from '../../config';
import logo from '../../assets/images/logo1.svg';

const Sidebar = () => {
    const [collapsed, setCollapsed] = useState(false);
    const [mobileOpen, setMobileOpen] = useState(false);
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

    const SidebarContent = useCallback(() => (
        <div className={styles.sidebarContainer}>
            {/* Logo Section */}
            <div className={styles.sidebarHeader}>
                <img src={logo} alt="Logo" className={styles.sidebarLogo} />
                {!collapsed && <span className={styles.sidebarTitle}>Loan Accelerator</span>}
            </div>

            <div className={styles.sidebarDivider} />

            {/* Collapse Toggle - Desktop Only */}
            <div className={`${styles.collapseToggle} ${styles.desktopOnly}`}>
                <Button
                    type="text"
                    icon={collapsed ? <MenuOutlined /> : <CloseOutlined />}
                    onClick={() => setCollapsed(!collapsed)}
                    className={styles.collapseBtn}
                />
            </div>

            {/* Menu Items */}
            <Menu
                mode="inline"
                selectedKeys={[location.pathname]}
                className={styles.sidebarMenu}
                inlineCollapsed={collapsed}
                items={menuItems}
            />

            {/* Logout Button */}
            <div className={styles.sidebarFooter}>
                <Menu
                    mode="inline"
                    className={styles.sidebarMenu}
                    inlineCollapsed={collapsed}
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
    ), [collapsed, location.pathname, menuItems, handleLogout]);

    return (
        <>
            {/* Mobile Menu Button */}
            <Button
                className={styles.mobileMenuBtn}
                type="primary"
                icon={<MenuOutlined />}
                onClick={() => setMobileOpen(true)}
            />

            {/* Desktop Sidebar */}
            <div className={`${styles.sidebarWrapper} ${collapsed ? styles.collapsed : ''}`}>
                <SidebarContent />
            </div>

            {/* Mobile Drawer */}
            <Drawer
                placement="left"
                onClose={() => setMobileOpen(false)}
                open={mobileOpen}
                className={styles.mobileSidebarDrawer}
                width={280}
                styles={{ body: { padding: 0 } }}
            >
                <SidebarContent />
            </Drawer>
        </>
    );
};

export default Sidebar;
