import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ApiError } from '../api/client';
import { customersApi } from '../api/customers';
import { ordersApi } from '../api/orders';
import type { Customer, Order } from '../api/types';
import { Btn, Card, ErrorBanner, Field, Page, StatusBadge, inputCls } from '../components/ui';

export default function OrdersPage() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [customerId, setCustomerId] = useState('');
  const [orders, setOrders] = useState<Order[]>([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    customersApi
      .list()
      .then((list) => {
        setCustomers(list);
        if (list.length > 0) setCustomerId((prev) => prev || list[0].id);
      })
      .catch((e) => setError(e instanceof ApiError ? e.message : 'Failed to load customers'));
  }, []);

  const refresh = useCallback(async (id: string) => {
    if (!id) return;
    try {
      setOrders(await ordersApi.byCustomer(id));
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Failed to load orders');
    }
  }, []);

  useEffect(() => {
    void refresh(customerId);
  }, [customerId, refresh]);

  async function createOrder() {
    setError('');
    try {
      const order = await ordersApi.create(customerId);
      // Jump straight to the new order so items can be added.
      navigate(`/orders/${order.id}`);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Create failed');
    }
  }

  return (
    <Page title="Orders">
      {error && <ErrorBanner message={error} onClose={() => setError('')} />}

      <Card>
        <div className="flex flex-wrap items-end gap-3">
          <Field label="Customer">
            <select
              className={inputCls}
              value={customerId}
              onChange={(e) => setCustomerId(e.target.value)}
            >
              {customers.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.username} ({c.isActive ? 'active' : 'inactive'})
                </option>
              ))}
            </select>
          </Field>
          <Btn variant="primary" onClick={() => void createOrder()} disabled={!customerId}>
            New order
          </Btn>
        </div>
      </Card>

      {orders.map((o) => (
        <Card key={o.id}>
          <div className="flex items-center justify-between gap-3">
            <div>
              <Link to={`/orders/${o.id}`} className="font-mono text-sm text-blue-700 hover:underline">
                {o.id.slice(0, 8)}…
              </Link>
              <div className="text-sm text-gray-500">
                {o.items.length} item(s) · total ${o.totalAmount}
              </div>
            </div>
            <StatusBadge status={o.status} />
          </div>
        </Card>
      ))}

      {orders.length === 0 && (
        <p className="text-sm text-gray-500">No orders for this customer yet.</p>
      )}
    </Page>
  );
}
