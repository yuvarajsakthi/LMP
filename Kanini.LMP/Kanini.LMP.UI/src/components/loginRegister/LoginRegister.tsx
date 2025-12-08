import { CustomerDashboard, Logo } from "../../assets";
import LoginRegisterCss from "./LoginRegister.module.css"
const LoginRegister = () => {
    return (
        <div className={LoginRegisterCss.background}>
            <div className={LoginRegisterCss.ray1}></div>
            <div className={LoginRegisterCss.ray2}></div>

            <div className={LoginRegisterCss.content}>
                <img src={Logo} alt="Logo" className={LoginRegisterCss.logo} />
                <p className={LoginRegisterCss.welcome}>
                    Welcome to <span className={LoginRegisterCss.brandHighlight}>LMP</span>
                </p>
                <p className={LoginRegisterCss.subText}>Login to access your account</p>
            </div>
            <img src={CustomerDashboard} alt="Dashboard Preview" className={LoginRegisterCss.image} />
        </div>
    );
};

export default LoginRegister;