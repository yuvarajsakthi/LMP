namespace Kanini.LMP.Database.Enums
{
    public enum LoanType
    {
        Personal = 1,
        Vehicle = 2,
        Home = 3
    }

    public enum PropertyType
    {
        Residential,
        Commercial,
        Industrial,
        SpecialPurpose
    }

    public enum OwnershipType
    {
        Owned,
        Builder
    }

    public enum LoanPurposeHome
    {
        Purchase,
        Construction,
        Renovation
    }

    public enum LoanPurposeVehicle
    {
        New,
        Used
    }

    public enum VehicleType
    {
        Car,
        Bike,
        Truck,
        Bus,
        Scooter,
        Van,
        Tractor,
        Bicycle,
        AutoRickshaw,
        Motorcycle,
        ElectricCar,
        PickupTruck,
        SUV,
        Minivan
    }

    public enum EmploymentType
    {
        Salaried,
        SelfEmployed,
        Business,
        Professional
    }

    public enum LoanPurposePersonal
    {
        Medical,
        Education,
        Travel,
        Wedding,
        DebtConsolidation,
        HomeRenovation,
        Other
    }
}
