import { apiClient } from "./apiClient";

// ── Types (flat / legacy) ───────────────────────────────────────────────────

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

// ── Types (grouped) ─────────────────────────────────────────────────────────

export interface SeverityCounts {
  info: number;
  warning: number;
  error: number;
  critical: number;
}

export interface GroupedActivityLogItem {
  userId: string;
  userName: string;
  userEmail: string | null;
  userRole: string;
  action: string;
  category: string | null;
  totalCount: number;
  latestTimestamp: string;
  severityCounts: SeverityCounts;
}

export interface GroupedActivityLogResponse {
  items: GroupedActivityLogItem[];
  totalGroups: number;
  page: number;
  pageSize: number;
  totalPages: number;
  roleCounts: Record<string, number>;
}

export interface ErrorDetailItem {
  message: string;
  errorType: string | null;
  count: number;
  latestAt: string;
}

export interface ErrorDetailsResponse {
  errors: ErrorDetailItem[];
}

// ── Service ─────────────────────────────────────────────────────────────────

export const activityLogService = {
  /**
   * Fetches a paginated, filtered list of user activity logs (flat list).
   */
  getLogs: (filters: ActivityLogFilters = {}): Promise<ActivityLogResponse> => {
    const params = buildParams(filters);
    return apiClient.get<ActivityLogResponse>(`/api/admin/activity-logs?${params.toString()}`);
  },

  /**
   * Fetches grouped activity logs (one row per user+action) with severity counts and role totals.
   */
  getGroupedLogs: (filters: ActivityLogFilters = {}): Promise<GroupedActivityLogResponse> => {
    const params = buildParams(filters);
    return apiClient.get<GroupedActivityLogResponse>(`/api/admin/activity-logs/grouped?${params.toString()}`);
  },

  /**
   * Fetches error details for a specific (userId, action) pair.
   */
  getErrorDetails: (
    userId: string,
    action: string,
    from?: string,
    to?: string,
  ): Promise<ErrorDetailsResponse> => {
    const params = new URLSearchParams();
    params.set("userId", userId);
    params.set("action", action);
    if (from) params.set("from", from);
    if (to) params.set("to", to);
    return apiClient.get<ErrorDetailsResponse>(`/api/admin/activity-logs/errors?${params.toString()}`);
  },
};

// ── Helpers ──────────────────────────────────────────────────────────────────

function buildParams(filters: ActivityLogFilters): URLSearchParams {
  const params = new URLSearchParams();
  if (filters.role) params.set("role", filters.role);
  if (filters.category) params.set("category", filters.category);
  if (filters.severity) params.set("severity", filters.severity);
  if (filters.search) params.set("search", filters.search);
  if (filters.from) params.set("from", filters.from);
  if (filters.to) params.set("to", filters.to);
  params.set("page", String(filters.page ?? 1));
  params.set("pageSize", String(filters.pageSize ?? 20));
  return params;
}
