import { NavLink, Navigate, Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import CustomersPage from './pages/CustomersPage';
import OrderDetailPage from './pages/OrderDetailPage';
import OrdersPage from './pages/OrdersPage';
import ProductsPage from './pages/ProductsPage';

function Nav() {
  const link = ({ isActive }: { isActive: boolean }) =>
    `rounded px-3 py-1 text-sm font-medium ${isActive ? 'bg-blue-600 text-white' : 'text-gray-700 hover:bg-gray-100'}`;

  return (
    <nav className="border-b border-gray-200 bg-white">
      <div className="mx-auto flex max-w-4xl gap-1 px-4 py-2">
        <NavLink to="/customers" className={link}>
          Customers
        </NavLink>
        <NavLink to="/products" className={link}>
          Products
        </NavLink>
        <NavLink to="/orders" className={link}>
          Orders
        </NavLink>
      </div>
    </nav>
  );
}

// Thin shell: routing + nav only. Each page owns its own
// data fetching through the api/ layer (no global store -
// the backend is the source of truth, refetched after each action).
export default function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50">
        <Nav />
        <Routes>
          <Route path="/" element={<Navigate to="/orders" replace />} />
          <Route path="/customers" element={<CustomersPage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/orders" element={<OrdersPage />} />
          <Route path="/orders/:id" element={<OrderDetailPage />} />
        </Routes>
      </div>
    </Router>
  );
}
