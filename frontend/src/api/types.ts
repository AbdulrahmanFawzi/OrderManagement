// DTO shapes mirror the backend Application DTOs (CustomerDto,
// ProductDto, OrderDto). ASP.NET serializes them as camelCase JSON,
// so field names here match the wire format, not the C# names.

export interface Customer {
  id: string;
  username: string;
  email: string;
  isActive: boolean;
  createdAt: string;
}

export interface Product {
  id: string;
  name: string;
  price: number;
  stock: number;
  isActive: boolean;
}

export interface OrderItem {
  id: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: string;
  customerId: string;
  /** Matches backend OrderStatus enum order: 0 Pending ... 5 Cancelled */
  status: number;
  createdAt: string;
  items: OrderItem[];
  totalAmount: number;
}

export const OrderStatusLabel = [
  'Pending',
  'Confirmed',
  'Processing',
  'Shipped',
  'Delivered',
  'Cancelled',
] as const;
