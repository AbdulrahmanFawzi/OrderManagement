// Single place for all HTTP concerns: base URL, JSON handling,
// and translating the backend's error shape ({title, detail, status}
// from ExceptionHandlingMiddleware) into a typed ApiError.
// Pages never call fetch() directly.

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080';

export class ApiError extends Error {
  status: number;
  title: string;

  constructor(status: number, title: string, detail: string) {
    // `detail` carries the useful message, e.g. "Insufficient stock...".
    super(detail || title);
    this.status = status;
    this.title = title;
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${API_URL}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...init?.headers },
  });

  // Activate/deactivate-style endpoints return 204 No Content.
  if (res.status === 204) return undefined as T;

  const text = await res.text();
  const body = text ? JSON.parse(text) : undefined;

  if (!res.ok) {
    throw new ApiError(res.status, body?.title ?? 'Request failed', body?.detail ?? '');
  }

  return body as T;
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, payload?: unknown) =>
    request<T>(path, { method: 'POST', body: payload === undefined ? undefined : JSON.stringify(payload) }),
  put: <T>(path: string, payload: unknown) =>
    request<T>(path, { method: 'PUT', body: JSON.stringify(payload) }),
  del: <T>(path: string) => request<T>(path, { method: 'DELETE' }),
};
