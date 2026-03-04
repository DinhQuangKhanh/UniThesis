import { apiClient } from "./apiClient";

// ── Types ──────────────────────────────────────────────────────────────────────

export interface ActivityLogItem {
  id: string;
  userId: string;
  userName: string;
  userEmail: string | null;
  userRole: string;
  action: string;
  category: string | null;
  entityType: string | null;
  entityId: string | null;
  severity: string;
  ipAddress: string | null;
  timestamp: string;
}

export interface ActivityLogResponse {
  items: ActivityLogItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ActivityLogFilters {
  role?: string;
  category?: string;
  severity?: string;
  search?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

// ── Service ────────────────────────────────────────────────────────────────────

export const activityLogService = {
  /**
   * Fetches a paginated, filtered list of user activity logs.
   */
  getLogs: (filters: ActivityLogFilters = {}): Promise<ActivityLogResponse> => {
    const params = new URLSearchParams();
    if (filters.role) params.set("role", filters.role);
    if (filters.category) params.set("category", filters.category);
    if (filters.severity) params.set("severity", filters.severity);
    if (filters.search) params.set("search", filters.search);
    if (filters.from) params.set("from", filters.from);
    if (filters.to) params.set("to", filters.to);
    params.set("page", String(filters.page ?? 1));
    params.set("pageSize", String(filters.pageSize ?? 20));

    return apiClient.get<ActivityLogResponse>(`/api/admin/activity-logs?${params.toString()}`);
  },
};
