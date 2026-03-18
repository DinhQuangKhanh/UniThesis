const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "";

function getToken(): string | null {
  try {
    const stored = localStorage.getItem("user");
    if (!stored) return null;
    const user = JSON.parse(stored);
    return user?.firebaseToken ?? null;
  } catch {
    return null;
  }
}

interface ApiErrorBody {
  code?: string;
  message?: string;
  errors?: Record<string, string[]>;
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string>),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (!response.ok) {
    let message = `HTTP ${response.status}: ${response.statusText}`;
    try {
      const body: ApiErrorBody = await response.json();
      if (body?.message) {
        message = body.message;
      }
      if (body?.errors) {
        const fieldErrors = Object.values(body.errors).flat().join(" ");
        if (fieldErrors) message += ` — ${fieldErrors}`;
      }
    } catch {
      // body is not JSON, keep the default message
    }
    throw new Error(message);
  }

  return response.json() as Promise<T>;
}

export const apiClient = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) => request<T>(path, { method: "POST", body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) => request<T>(path, { method: "PUT", body: JSON.stringify(body) }),
  /** Send a FormData payload (file uploads). Browser sets Content-Type + boundary automatically. */
  postForm: <T>(path: string, formData: FormData): Promise<T> => {
    const token = getToken();
    const headers: Record<string, string> = {};
    if (token) headers["Authorization"] = `Bearer ${token}`;
    return fetch(`${API_BASE}${path}`, { method: "POST", headers, body: formData })
      .then(async (res) => {
        if (!res.ok) {
          let message = `HTTP ${res.status}: ${res.statusText}`;
          try {
            const body: ApiErrorBody = await res.json();
            if (body?.message) message = body.message;
            if (body?.errors) {
              const fieldErrors = Object.values(body.errors).flat().join(" ");
              if (fieldErrors) message += ` — ${fieldErrors}`;
            }
          } catch { /* non-JSON body */ }
          throw new Error(message);
        }
        return res.json() as Promise<T>;
      });
  },
};

