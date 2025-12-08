import { useState } from 'react';
import { Menu, Drawer, Button } from 'antd';
import {
  DashboardOutlined,
  FileTextOutlined,
  CodeSandboxOutlined,
  BarChartOutlined,
  EyeOutlined,
  AppstoreOutlined,
  UserOutlined,
  CheckCircleOutlined,
  LogoutOutlined,
  MenuOutlined,
  CloseOutlined
} from '@ant-design/icons';
import './Sidenavbar-Manager.css';
import { useNavigate, useLocation } from 'react-router-dom';
import logo from '../../assets/images/logo.svg';

const SidenavbarManager = () => {
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  const menuItems = [
    {
      key: 'dashboard',
      icon: <DashboardOutlined />,
      label: 'Dashboard',
      onClick: () => { }
    },
    {
      key: 'loan-origination',
      icon: <FileTextOutlined />,
      label: 'Loan Origination',
      onClick: () => { }
    },
    {
      key: 'loan-products',
      icon: <CodeSandboxOutlined />,
      label: 'Loan Products',
      children: [
        {
          key: '/loan-centric',
          label: 'Loan Analytics',
          icon: <BarChartOutlined />,
          onClick: () => navigate('/loan-centric')
        },
        {
          key: 'loan-centric-view',
          label: 'Loan Centric View',
          icon: <EyeOutlined />,
          onClick: () => { }
        },
        {
          key: 'superset',
          label: 'Superset Dashboard',
          icon: <AppstoreOutlined />,
          onClick: () => { }
        }
      ]
    },
    {
      key: 'customer-scape',
      icon: <UserOutlined />,
      label: 'Customer Scape',
      onClick: () => { }
    },
    {
      key: '/applied-loan',
      icon: <CheckCircleOutlined />,
      label: 'Applied Loans',
      onClick: () => navigate('/applied-loan')
    }
  ];

  const SidebarContent = () => (
    <div className="manager-sidebar-container">
      {/* Logo Section */}
      <div className="manager-sidebar-header">
        <img src={logo} alt="Logo" className="manager-sidebar-logo" />
        {!collapsed && <span className="manager-sidebar-title">LMP</span>}
      </div>

      <div className="manager-sidebar-divider" />

      {/* Collapse Toggle - Desktop Only */}
      <div className="manager-collapse-toggle desktop-only">
        <Button
          type="text"
          icon={collapsed ? <MenuOutlined /> : <CloseOutlined />}
          onClick={() => setCollapsed(!collapsed)}
          className="manager-collapse-btn"
        />
      </div>

      {/* Menu Items */}
      <Menu
        mode="inline"
        selectedKeys={[location.pathname]}
        defaultOpenKeys={['loan-products']}
        className="manager-sidebar-menu"
        inlineCollapsed={collapsed}
        items={menuItems}
      />

      {/* Logout Button */}
      <div className="manager-sidebar-footer">
        <Menu
          mode="inline"
          className="manager-sidebar-menu"
          inlineCollapsed={collapsed}
          items={[
            {
              key: 'logout',
              icon: <LogoutOutlined />,
              label: 'Logout',
              onClick: handleLogout,
              className: 'manager-logout-item'
            }
          ]}
        />
      </div>
    </div>
  );

  return (
    <>
      {/* Mobile Menu Button */}
      <Button
        className="manager-mobile-menu-btn"
        type="primary"
        icon={<MenuOutlined />}
        onClick={() => setMobileOpen(true)}
      />

      {/* Desktop Sidebar */}
      <div className={`manager-sidebar-wrapper ${collapsed ? 'collapsed' : ''}`}>
        <SidebarContent />
      </div>

      {/* Mobile Drawer */}
      <Drawer
        placement="left"
        onClose={() => setMobileOpen(false)}
        open={mobileOpen}
        className="manager-mobile-sidebar-drawer"
        width={280}
        styles={{ body: { padding: 0 } }}
      >
        <SidebarContent />
      </Drawer>
    </>
  );
};

export default SidenavbarManager;
