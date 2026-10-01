import { api } from './client';
import type { Customer } from './types';

// One function per backend endpoint. No business logic here,
// just URL + payload shapes (mirrors the thin API controllers).
export const customersApi = {
  list: () => api.get<Customer[]>('/api/customers'),
  get: (id: string) => api.get<Customer>(`/api/customers/${id}`),
  create: (username: string, email: string) =>
    api.post<Customer>('/api/customers', { username, email }),
  activate: (id: string) => api.post<void>(`/api/customers/${id}/activate`),
  deactivate: (id: string) => api.post<void>(`/api/customers/${id}/deactivate`),
};
