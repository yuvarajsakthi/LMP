import { useState } from 'react';
import style from './LoanTypes.module.css';
import VL from '../../assets/images/Vehicle-Loan.svg';
import PL from '../../assets/images/PersonalLoan.svg';
import HL from '../../assets/images/Housing Loan.svg';
import note from '../../assets/images/note.svg';
import bell from '../../assets/images/bell.svg';
import arrow from '../../assets/images/NextButtonArrow.svg';
import { Button, Card } from 'antd';
import { useNavigate } from 'react-router-dom';
import { CUSTOMER_ROUTES } from '../../config';
const { Meta } = Card;

interface LoanCategory {
  loanCategoryId: number;
  loanCategoryName: string;
  loanCategoryKey: string;
}

const LoanTypes = () => {
    const [selectedCategory, setSelectedCategory] = useState<LoanCategory | null>(null);
    const [clickedCard, setClickedCard] = useState<number | null>(null);

    const handleCardClick = (category: LoanCategory) => {
        setClickedCard(category.loanCategoryId);
        setSelectedCategory(category);
    };

    const categories: LoanCategory[] = [
        { loanCategoryId: 1, loanCategoryName: 'Personal Loan', loanCategoryKey: 'personal' },
        { loanCategoryId: 2, loanCategoryName: 'Vehicle Loan', loanCategoryKey: 'vehicle' },
        { loanCategoryId: 3, loanCategoryName: 'Housing Loan', loanCategoryKey: 'home' }
    ];

    const navigate = useNavigate();

    const getImageByFilename = (filename: string) => {
        switch (filename) {
            case 'Vehicle Loan':
                return VL;
            case 'Personal Loan':
                return PL;
            case 'Housing Loan':
                return HL;
            default:
                return PL;
        }
    };

    return (
        <div className={style.grey}>
            <div className={style.container}>
                <div className={style.sidebar}>
                    <img src={bell} alt="notification" className={style.icon} />
                    <h2 className={style.sidebarTitle}>A few clicks away from applying your loan.</h2>
                    <p className={style.sidebarText}>
                        Avail quick & easy Loans from the comfort of your home. Apply online & get instant approval.
                    </p>
                    <img src={note} alt="note" className={style.noteIcon} />
                </div>

                <div className={style.mainContent}>
                    <h1 className={style.title}>Select loan category</h1>
                    <p className={style.subtitle}>
                        Applying your loan is just a few steps away. Select any one of the loan type to continue.
                    </p>

                    <div className={style.cardGrid}>
                        {categories.map((category) => (
                            <Card
                                key={category.loanCategoryId}
                                hoverable
                                className={`${style.card} ${clickedCard === category.loanCategoryId ? style.cardSelected : ''}`}
                                cover={
                                    <div className={style.imageContainer}>
                                        <img
                                            alt={category.loanCategoryKey}
                                            src={getImageByFilename(category.loanCategoryName)}
                                            className={style.image}
                                        />
                                    </div>
                                }
                                onClick={() => handleCardClick(category)}
                            >
                                <Meta title={category.loanCategoryName} className={style.cardTitle} />
                            </Card>
                        ))}
                    </div>

                    <div className={style.buttonContainer}>
                        <Button
                            type="primary"
                            size="large"
                            className={style.nextButton}
                            onClick={() => {
                                if (selectedCategory) {
                                    navigate(CUSTOMER_ROUTES.LOAN_APPLICATION, { 
                                        state: { 
                                            selectedCategory: {
                                                loanProductId: selectedCategory.loanCategoryId,
                                                loanProductName: selectedCategory.loanCategoryName
                                            }
                                        } 
                                    });
                                }
                            }}
                            disabled={!selectedCategory}
                        >
                            NEXT
                            <img src={arrow} alt="arrow" className={style.arrowIcon} />
                        </Button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default LoanTypes;
