import type { ReactNode } from 'react';
import { OrderStatusLabel } from '../api/types';

// Tiny presentational building blocks shared by all pages.
// No data fetching here, only markup + Tailwind classes.

export function Page({ title, children }: { title: string; children: ReactNode }) {
  return (
    <div className="mx-auto max-w-4xl px-4 py-6">
      <h1 className="mb-4 text-2xl font-semibold text-gray-900">{title}</h1>
      {children}
    </div>
  );
}

export function Card({ children }: { children: ReactNode }) {
  return <div className="mb-4 rounded-lg border border-gray-200 bg-white p-4 shadow-sm">{children}</div>;
}

export function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <label className="flex flex-col gap-1 text-sm text-gray-700">
      <span className="font-medium">{label}</span>
      {children}
    </label>
  );
}

export const inputCls =
  'rounded border border-gray-300 px-2 py-1 text-sm focus:border-blue-500 focus:outline-none';

type BtnProps = {
  children: ReactNode;
  onClick?: () => void;
  type?: 'button' | 'submit';
  variant?: 'primary' | 'ghost' | 'danger';
  disabled?: boolean;
};

export function Btn({ children, onClick, type = 'button', variant = 'ghost', disabled }: BtnProps) {
  const base = 'rounded px-3 py-1 text-sm font-medium disabled:opacity-50';
  const styles = {
    primary: 'bg-blue-600 text-white hover:bg-blue-700',
    ghost: 'border border-gray-300 bg-white hover:bg-gray-50',
    danger: 'border border-red-300 bg-white text-red-700 hover:bg-red-50',
  } as const;
  return (
    <button type={type} onClick={onClick} disabled={disabled} className={`${base} ${styles[variant]}`}>
      {children}
    </button>
  );
}

export function ErrorBanner({ message, onClose }: { message: string; onClose: () => void }) {
  return (
    <div className="mb-4 flex items-start justify-between gap-4 rounded border border-red-300 bg-red-50 px-3 py-2 text-sm text-red-800">
      <span>{message}</span>
      <button onClick={onClose} className="font-bold" aria-label="Dismiss">
        ×
      </button>
    </div>
  );
}

export function ActiveBadge({ active }: { active: boolean }) {
  return (
    <span
      className={`rounded-full px-2 py-0.5 text-xs font-medium ${
        active ? 'bg-green-100 text-green-800' : 'bg-gray-200 text-gray-600'
      }`}
    >
      {active ? 'Active' : 'Inactive'}
    </span>
  );
}

const statusColor = [
  'bg-yellow-100 text-yellow-800', // Pending
  'bg-blue-100 text-blue-800', // Confirmed
  'bg-purple-100 text-purple-800', // Processing
  'bg-indigo-100 text-indigo-800', // Shipped
  'bg-green-100 text-green-800', // Delivered
  'bg-gray-200 text-gray-600', // Cancelled
];

export function StatusBadge({ status }: { status: number }) {
  return (
    <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${statusColor[status] ?? ''}`}>
      {OrderStatusLabel[status] ?? status}
    </span>
  );
}
