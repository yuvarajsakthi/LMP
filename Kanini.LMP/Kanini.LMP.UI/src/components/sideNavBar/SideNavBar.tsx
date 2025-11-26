import { useState } from 'react';
import { Menu } from 'antd';
import { useNavigate } from 'react-router-dom';
import SideNavBarCss from './SideNavBar.module.css';
import { Logo1, FaqIcon, Emi, LogOut, ChartEdit, Arrow, Dashboard, Status, Settings } from '../../assets';
import { ROUTES } from '../../config';
import { useAuth } from '../../context';
import { authMiddleware } from '../../middleware';

const SideNavBar = () => {
    const [collapsed, setCollapsed] = useState(true);
    const [selectedKey, setSelectedKey] = useState('');

    const toggleCollapsed = () => {
        try {
            setCollapsed(!collapsed);
        } catch (error) {
            console.error('Failed to toggle sidebar:', error);
        }
    };

    const navigate = useNavigate();
    const { logout: contextLogout } = useAuth();

    const handleClick = (e: any) => {
        setSelectedKey(e.key);
        
        const routes = {
            '1': ROUTES.CUSTOMER_DASHBOARD,
            '2': '/apply-loan',
            '3': '/view-status',
            '4': '/emi-calculator',
            '5': '/faq',
            '6': '/settings'
        };
        
        const route = routes[e.key as keyof typeof routes];
        if (route) {
            navigate(route);
        }
    };

    const handleLogout = () => {
        authMiddleware.removeToken();
        contextLogout();
        navigate(ROUTES.LOGIN);
    };

    return (
        <div style={{ width: 100 }} className={SideNavBarCss.menuitem}>
            <Menu
                className={SideNavBarCss.menu}
                defaultSelectedKeys={['0']}
                mode="inline"
                theme="light"
                inlineCollapsed={collapsed}
                onClick={handleClick}
                selectedKeys={[selectedKey]}
            >
                <Menu.Item className={SideNavBarCss.logo} icon={<img src={Logo1} width={25} height={25} alt="Logo"></img>}>
                    Loan Accelerator
                </Menu.Item>
                <hr />
                <Menu.Item icon={collapsed ? <img src={Arrow} alt="Toggle" /> : <img src={LogOut} alt="Toggle" />} onClick={toggleCollapsed}></Menu.Item>

                <Menu.Item key="1" icon={<img src={Dashboard} alt="Dashboard"></img>} title="Dashboard" className={SideNavBarCss.options}>
                    &nbsp;&nbsp; Dashboard
                </Menu.Item>

                <Menu.Item key="2" icon={<img src={ChartEdit} alt="Apply Loan"></img>} title="Apply Loan" className={SideNavBarCss.options}>
                    &nbsp; &nbsp; Apply Loan
                </Menu.Item>

                <Menu.Item key="3" icon={<img src={Status} alt="Status"></img>} title="View Status" className={SideNavBarCss.options}>
                    &nbsp; &nbsp; View Status
                </Menu.Item>

                <Menu.Item key="4" icon={<img src={Emi} alt="EMI"></img>} title="EMI Calculator" className={SideNavBarCss.options}>
                    &nbsp; &nbsp;  EMI Calculator
                </Menu.Item>

                <Menu.Item key="5" icon={<img src={FaqIcon} alt="FAQ"></img>} title="FAQ's" className={SideNavBarCss.options}>
                    &nbsp; &nbsp; FAQ's
                </Menu.Item>

                <Menu.Item key="6" icon={<img src={Settings} alt="Settings"></img>} title="Settings" className={SideNavBarCss.options}>
                    &nbsp; &nbsp; Settings
                </Menu.Item>

                <Menu.Item key="7" icon={<img src={LogOut} alt="Logout"></img>} title="Logout" className={SideNavBarCss.logout} onClick={handleLogout}>
                    &nbsp;    Logout
                </Menu.Item>
            </Menu>
        </div>
    );
};

export default SideNavBar;