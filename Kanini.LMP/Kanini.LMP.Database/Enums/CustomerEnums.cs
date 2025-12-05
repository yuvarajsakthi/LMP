namespace Kanini.LMP.Database.Enums
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum HomeOwnershipStatus
    {
        Rented,
        Owned,
        Mortage
    }

    public enum ApplicationStatus
    {
        Draft,
        Submitted,
        Withdrawn,
        Pending,
        Rejected,
        Approved,
        Disbursed
    }

    public enum IndianStates
    {
        AndhraPradesh,
        ArunachalPradesh,
        Assam,
        Bihar,
        Chhattisgarh,
        Goa,
        Gujarat,
        Haryana,
        HimachalPradesh,
        Jharkhand,
        Karnataka,
        Kerala,
        MadhyaPradesh,
        Maharashtra,
        Manipur,
        Meghalaya,
        Mizoram,
        Nagaland,
        Odisha,
        Punjab,
        Rajasthan,
        Sikkim,
        TamilNadu,
        Telangana,
        Tripura,
        UttarPradesh,
        Uttarakhand,
        WestBengal,
        AndamanAndNicobarIslands,
        Chandigarh,
        DadraAndNagarHaveliAndDamanAndDiu,
        Delhi,
        JammuAndKashmir,
        Ladakh,
        Lakshadweep,
        Puducherry
    }

    public enum EMIPlanStatus
    {
        Active,
        Overdue,
        Closed
    }

    public enum LoanStatus
    {
        Pending,
        Approved,
        Rejected,
        Disbursed,
        Closed
    }

    public enum EducationQualification
    {
        BelowMatric = 1,
        Matric = 2,
        HigherSecondary = 3,
        Graduate = 4,
        PostGraduate = 5,
        Doctorate = 6,
        Other = 7
    }

    public enum ResidentialStatus
    {
        Owned = 1,
        Rented = 2,
        Parental = 3,
        CompanyProvided = 4,
        PayingGuest = 5,
        Other = 6
    }

    public enum EligibilityStatus
    {
        NotEligible,
        ConditionallyEligible,
        Eligible,
        HighlyEligible
    }
}
