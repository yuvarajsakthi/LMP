import Image from '../../assets/images/LogoN.svg';
import styles from './Logo.module.css';

const Logo = () => {
    return (
        <div className={styles.logoContainer}>
            <div className={styles.logoWrapper}>
                <img src={Image} alt="Logo" className={styles.logoImage} />
            </div>
        </div>
    );
};

export default Logo;