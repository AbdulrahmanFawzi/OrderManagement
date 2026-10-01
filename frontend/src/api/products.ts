import { api } from './client';
import type { Product } from './types';

export const productsApi = {
  list: () => api.get<Product[]>('/api/products'),
  get: (id: string) => api.get<Product>(`/api/products/${id}`),
  create: (name: string, price: number, stock: number) =>
    api.post<Product>('/api/products', { name, price, stock }),
  changePrice: (id: string, newPrice: number) =>
    api.put<void>(`/api/products/${id}/price`, { newPrice }),
  increaseStock: (id: string, quantity: number) =>
    api.post<void>(`/api/products/${id}/stock/increase`, { quantity }),
  decreaseStock: (id: string, quantity: number) =>
    api.post<void>(`/api/products/${id}/stock/decrease`, { quantity }),
  activate: (id: string) => api.post<void>(`/api/products/${id}/activate`),
  deactivate: (id: string) => api.post<void>(`/api/products/${id}/deactivate`),
};
