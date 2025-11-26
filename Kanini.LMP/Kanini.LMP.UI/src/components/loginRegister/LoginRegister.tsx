import logo from '../../assets/images/logo.svg';
import image from '../../assets/images/Customer Dashboard 1.svg';
import LoginRegisterCss from "./LoginRegister.module.css"
const LoginRegister = () => {
    return (
        <div className={LoginRegisterCss.background}>
            <div className={LoginRegisterCss.ray1}></div>
            <div className={LoginRegisterCss.ray2}></div>

            <div className={LoginRegisterCss.content}>
                <img src={logo} alt="Logo" className={LoginRegisterCss.logo} />
                <p className={LoginRegisterCss.welcome}>
                    Welcome to <span className={LoginRegisterCss.brandHighlight}>Loan Accelerator</span>
                </p>
                <p className={LoginRegisterCss.subText}>Login to access your account</p>
            </div>

            <img src={image} alt="Dashboard Preview" className={LoginRegisterCss.image} />
        </div>
    );
};
export default LoginRegister;