export interface VehicleLoanDetails {
  vehicleType: number;
  manufacturer: string;
  model: string;
  manufacturingYear: number;
  onRoadPrice: number;
  downPayment: number;
  loanPurposeVehicle: number;
}

export enum VehicleType {
  Car = 0,
  Bike = 1,
  Truck = 2,
  Bus = 3,
  Scooter = 4,
  Van = 5,
  Tractor = 6,
  Bicycle = 7,
  AutoRickshaw = 8,
  Motorcycle = 9,
  ElectricCar = 10,
  PickupTruck = 11,
  SUV = 12,
  Minivan = 13
}

export enum LoanPurposeVehicle {
  New = 0,
  Used = 1
}
