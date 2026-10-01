import { useCallback, useEffect, useState } from 'react';
import { ApiError } from '../api/client';
import { customersApi } from '../api/customers';
import type { Customer } from '../api/types';
import { ActiveBadge, Btn, Card, ErrorBanner, Field, Page, inputCls } from '../components/ui';

export default function CustomersPage() {
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');

  const refresh = useCallback(async () => {
    try {
      setCustomers(await customersApi.list());
    } catch (e) {
      setError(e instanceof ApiError ? e.message : 'Failed to load customers');
    }
  }, []);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  async function create(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    try {
      await customersApi.create(username.trim(), email.trim());
      setUsername('');
      setEmail('');
      await refresh();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Create failed');
    }
  }

  async function toggle(c: Customer) {
    setError('');
    try {
      if (c.isActive) await customersApi.deactivate(c.id);
      else await customersApi.activate(c.id);
      await refresh();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : 'Update failed');
    }
  }

  return (
    <Page title="Customers">
      {error && <ErrorBanner message={error} onClose={() => setError('')} />}

      <Card>
        <form onSubmit={create} className="flex flex-wrap items-end gap-3">
          <Field label="Username">
            <input className={inputCls} value={username} onChange={(e) => setUsername(e.target.value)} required />
          </Field>
          <Field label="Email">
            <input
              className={inputCls}
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </Field>
          <Btn type="submit" variant="primary">
            Create customer
          </Btn>
        </form>
      </Card>

      {customers.map((c) => (
        <Card key={c.id}>
          <div className="flex items-center justify-between gap-3">
            <div>
              <div className="font-medium">{c.username}</div>
              <div className="text-sm text-gray-500">{c.email}</div>
            </div>
            <div className="flex items-center gap-2">
              <ActiveBadge active={c.isActive} />
              <Btn onClick={() => void toggle(c)}>{c.isActive ? 'Deactivate' : 'Activate'}</Btn>
            </div>
          </div>
        </Card>
      ))}
    </Page>
  );
}
