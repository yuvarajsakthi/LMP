
import Sidebar from "../../../layout/sidebar/Sidebar";
import EmiCalculator from "../../../components/dashboard/emiCalculator/EmiCalculator";
import LoanReco from "../../../components/dashboard/loanRecommendation/LoanRecommendation";
import EligibilityScore from "../../../components/dashboard/eligibilityScore/EligibilityScore";
import ApplicationStatus from "../../../components/dashboard/applicationStatus/ApplicationStatus";
import ApplicationProfile from "../../../components/dashboard/applicationProfile/ApplicationProfile";
import { useAuth } from "../../../context";
import './CustomerDashboard.css';

const CustomerDashboard = () => {
  const { token } = useAuth();

  return (
    <div className="dshboard">
      <div className="maindashboard123">
        <div>
          <Sidebar />
        </div>

        <div className="dashhome234">
          <div id="namecontent">
            <p className="nameshown"> Hi {token?.FullName || 'User'} </p>
            <p className="statusshown">Have you applied for any loan yet!</p>
          </div>

          <div className="boxes">
            <LoanReco />
            <EmiCalculator />
            <EligibilityScore />
          </div>

          <div className="row2">
            <div id="application">
              <ApplicationStatus />
            </div>
            <div className="appprofileres">
              <ApplicationProfile />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CustomerDashboard;
