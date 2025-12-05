import { type ReactNode } from 'react';
import { Button, Layout as AntLayout } from 'antd';
import { MenuOutlined, MenuFoldOutlined } from '@ant-design/icons';
import { motion, AnimatePresence } from 'framer-motion';
import { useLocation } from 'react-router-dom';
import { useLayout } from '../context';
import Sidebar from './sidebar/Sidebar';
import Logo from './logo/Logo';
import Navbar from './navbar/Navbar';
import styles from './Layout.module.css';

const { Header, Content, Sider } = AntLayout;

interface LayoutProps {
  children: ReactNode;
}

const Layout = ({ children }: LayoutProps) => {
  const { collapsed, setCollapsed } = useLayout();
  const location = useLocation();

  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };

  return (
    <AntLayout className={styles.layout}>
      <Sider 
        trigger={null} 
        collapsible 
        collapsed={collapsed}
        className={styles.sider}
        width={220}
        collapsedWidth={60}
      >
        <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
          <Logo />
          <Sidebar collapsed={collapsed} />
        </div>
      </Sider>
      
      <AntLayout>
        <Header className={styles.header}>
          <Button
            type="text"
            icon={collapsed ? <MenuOutlined /> : <MenuFoldOutlined />}
            onClick={toggleSidebar}
            className={styles.toggleButton}
          />
          <Navbar />
        </Header>
        
        <Content className={styles.content}>
          <AnimatePresence mode="wait">
            <motion.div
              key={location.pathname}
              initial={{ opacity: 0, y: 10 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -10 }}
              transition={{ duration: 0.2 }}
              className={styles.contentWrapper}
            >
              {children}
            </motion.div>
          </AnimatePresence>
        </Content>
      </AntLayout>
    </AntLayout>
  );
};

export default Layout;