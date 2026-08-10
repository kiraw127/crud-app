import { Car } from './car.model';

export interface Rental {
  id: number;
  carId: number;
  car?: Car;
  customerName: string;
  phone: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  status: string;
}

export interface CreateRentalRequest {
  carId: number;
  phone: string;
  startDate: string;
  endDate: string;
}
