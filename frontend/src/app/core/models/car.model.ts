export interface Car {
  id: number;
  brand: string;
  model: string;
  year: number;
  category: string;
  dailyRate: number;
  transmission: string;
  seats: number;
  imageUrl: string;
  isAvailable: boolean;
  description?: string;
}

export type SaveCarRequest = Omit<Car, 'id' | 'isAvailable'>;

export const createEmptyCar = (): Car => ({
  id: 0,
  brand: '',
  model: '',
  year: new Date().getFullYear(),
  category: 'Комфорт',
  dailyRate: 20000,
  transmission: 'Автомат',
  seats: 5,
  imageUrl:
    'https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=1200&q=80',
  isAvailable: true,
  description: '',
});
