import { useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import { productsApi } from '../api/products';
import type { Product } from '../api/types';
import { ActiveBadge, Btn, Card, ErrorBanner, Field, Page, inputCls } from '../components/ui';

function ProductRow({ product, onChanged, onError }: {
  product: Product;
  onChanged: () => void;
  onError: (msg: string) => void;
}) {
  const [price, setPrice] = useState(String(product.price));
  const [qty, setQty] = useState('1');

  async function run(action: Promise<unknown>) {
    try {
      await action;
      onChanged();
    } catch (err) {
      onError(err instanceof ApiError ? err.message : 'Update failed');
    }
  }

  return (
    <Card>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <div className="font-medium">{product.name}</div>
          <div className="text-sm text-gray-500">
            ${product.price} · stock {product.stock}
          </div>
        </div>
        <ActiveBadge active={product.isActive} />
      </div>

      <div className="mt-3 flex flex-wrap items-end gap-2">
        <Field label="Price">
          <input
            className={`${inputCls} w-24`}
            type="number"
            min="0"
            step="0.01"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
          />
        </Field>
        <Btn onClick={() => void run(productsApi.changePrice(product.id, Number(price)))}>Set price</Btn>

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
        <Btn onClick={() => void run(productsApi.increaseStock(product.id, Number(qty)))}>+ Stock</Btn>
        <Btn onClick={() => void run(productsApi.decreaseStock(product.id, Number(qty)))}>− Stock</Btn>

        {product.isActive ? (
          <Btn variant="danger" onClick={() => void run(productsApi.deactivate(product.id))}>
            Deactivate
          </Btn>
        ) : (
          <Btn onClick={() => void run(productsApi.activate(product.id))}>Activate</Btn>
        )}
      </div>
    </Card>
  );
}

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [stock, setStock] = useState('');
  const [error, setError] = useState('');

  const refresh = useCallback(async () => {
    try {
      setProducts(await productsApi.list());
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Failed to load products');
    }
  }, []);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  async function create(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    try {
      await productsApi.create(name.trim(), Number(price), Number(stock));
      setName('');
      setPrice('');
      setStock('');
      await refresh();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Create failed');
    }
  }

  return (
    <Page title="Products">
      {error && <ErrorBanner message={error} onClose={() => setError('')} />}

      <Card>
        <form onSubmit={create} className="flex flex-wrap items-end gap-3">
          <Field label="Name">
            <input className={inputCls} value={name} onChange={(e) => setName(e.target.value)} required />
          </Field>
          <Field label="Price">
            <input
              className={`${inputCls} w-28`}
              type="number"
              min="0"
              step="0.01"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              required
            />
          </Field>
          <Field label="Stock">
            <input
              className={`${inputCls} w-24`}
              type="number"
              min="0"
              step="1"
              value={stock}
              onChange={(e) => setStock(e.target.value)}
              required
            />
          </Field>
          <Btn type="submit" variant="primary">
            Create product
          </Btn>
        </form>
      </Card>

      {products.map((p) => (
        <ProductRow key={p.id} product={p} onChanged={() => void refresh()} onError={setError} />
      ))}
    </Page>
  );
}
