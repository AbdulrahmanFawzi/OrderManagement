import { useCallback, useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ApiError } from '../api/client';
import { ordersApi } from '../api/orders';
import { productsApi } from '../api/products';
import type { Order, Product } from '../api/types';
import { Btn, Card, ErrorBanner, Field, Page, StatusBadge, inputCls } from '../components/ui';

// Which lifecycle buttons are valid mirrors the backend Order guards:
// Pending -> confirm/cancel, Confirmed -> process/cancel, etc.
// The backend still enforces this; the UI just hides invalid actions.
function actionsFor(status: number): { label: string; run: (id: string) => Promise<Order> }[] {
  switch (status) {
    case 0:
      return [
        { label: 'Confirm', run: ordersApi.confirm },
        { label: 'Cancel', run: ordersApi.cancel },
      ];
    case 1:
      return [
        { label: 'Start processing', run: ordersApi.process },
        { label: 'Cancel', run: ordersApi.cancel },
      ];
    case 2:
      return [{ label: 'Ship', run: ordersApi.ship }];
    case 3:
      return [{ label: 'Deliver', run: ordersApi.deliver }];
    default:
      return [];
  }
}

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [products, setProducts] = useState<Product[]>([]);
  const [productId, setProductId] = useState('');
  const [qty, setQty] = useState('1');
  const [error, setError] = useState('');

  const refresh = useCallback(async () => {
    if (!id) return;
    try {
      setOrder(await ordersApi.get(id));
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Failed to load order');
    }
  }, [id]);

  useEffect(() => {
    void refresh();
    productsApi
      .list()
      .then((list) => {
        setProducts(list);
        if (list.length > 0) setProductId((prev) => prev || list[0].id);
      })
      .catch((e) => setError(e instanceof ApiError ? e.message : 'Failed to load products'));
  }, [refresh]);

  async function mutate(action: Promise<Order>) {
    setError('');
    try {
      setOrder(await action);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Action failed');
    }
  }

  if (!order) {
    return (
      <Page title="Order">
        {error && <ErrorBanner message={error} onClose={() => setError('')} />}
        <p className="text-sm text-gray-500">Loading…</p>
      </Page>
    );
  }

  const pending = order.status === 0;

  return (
    <Page title="Order">
      {error && <ErrorBanner message={error} onClose={() => setError('')} />}

      <Card>
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <div className="font-mono text-sm">{order.id}</div>
            <div className="text-sm text-gray-500">
              {order.items.length} item(s) · total ${order.totalAmount}
            </div>
          </div>
          <StatusBadge status={order.status} />
        </div>

        <div className="mt-3 flex flex-wrap gap-2">
          {actionsFor(order.status).map((a) => (
            <Btn key={a.label} variant="primary" onClick={() => void mutate(a.run(order.id))}>
              {a.label}
            </Btn>
          ))}
        </div>
      </Card>

      <Card>
        <h2 className="mb-2 font-medium">Items</h2>
        {order.items.length === 0 && (
          <p className="text-sm text-gray-500">No items yet. Confirm requires at least one.</p>
        )}
        {order.items.map((i) => (
          <div key={i.id} className="flex items-center justify-between border-t py-2 text-sm first:border-0">
            <span>
              {i.quantity} × ${i.unitPrice} = ${i.lineTotal}
              <span className="ml-2 font-mono text-xs text-gray-400" title={i.productId}>
                {i.productId.slice(0, 8)}…
              </span>
            </span>
            {pending && (
              <Btn variant="danger" onClick={() => void mutate(ordersApi.removeItem(order.id, i.id))}>
                Remove
              </Btn>
            )}
          </div>
        ))}

        {pending && (
          <div className="mt-3 flex flex-wrap items-end gap-2">
            <Field label="Product">
              <select className={inputCls} value={productId} onChange={(e) => setProductId(e.target.value)}>
                {products.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name} (${p.price}, stock {p.stock})
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Qty">
              <input
                className={`${inputCls} w-20`}
                type="number"
                min="1"
                step="1"
                value={qty}
                onChange={(e) => setQty(e.target.value)}
              />
            </Field>
            <Btn
              variant="primary"
              onClick={() => void mutate(ordersApi.addItem(order.id, productId, Number(qty)))}
              disabled={!productId}
            >
              Add item
            </Btn>
          </div>
        )}
      </Card>

      <Link to="/orders" className="text-sm text-blue-700 hover:underline">
        ← Back to orders
      </Link>
    </Page>
  );
}
