import { api } from './client';
import type { Order } from './types';

export const ordersApi = {
  create: (customerId: string) => api.post<Order>('/api/orders', { customerId }),
  get: (id: string) => api.get<Order>(`/api/orders/${id}`),
  byCustomer: (customerId: string) => api.get<Order[]>(`/api/orders/customer/${customerId}`),
  addItem: (orderId: string, productId: string, quantity: number) =>
    api.post<Order>(`/api/orders/${orderId}/items`, { productId, quantity }),
  removeItem: (orderId: string, itemId: string) =>
    api.del<Order>(`/api/orders/${orderId}/items/${itemId}`),
  confirm: (orderId: string) => api.post<Order>(`/api/orders/${orderId}/confirm`),
  cancel: (orderId: string) => api.post<Order>(`/api/orders/${orderId}/cancel`),
  process: (orderId: string) => api.post<Order>(`/api/orders/${orderId}/process`),
  ship: (orderId: string) => api.post<Order>(`/api/orders/${orderId}/ship`),
  deliver: (orderId: string) => api.post<Order>(`/api/orders/${orderId}/deliver`),
};
