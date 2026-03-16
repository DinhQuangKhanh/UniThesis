import { apiClient } from "./apiClient";

// ── Types ────────────────────────────────────────────────────────────────────

export interface PoolTopicItem {
  id: string;
  code: string;
  nameVi: string;
  nameEn: string;
  description: string;
  technologies: string | null;
  majorId: number;
  majorName: string;
  majorCode: string;
  poolStatus: number;
  poolStatusName: string;
  maxStudents: number;
  mentorName: string;
  mentorId: string;
  createdAt: string;
}

export interface PoolTopicDetail {
  id: string;
  code: string;
  nameVi: string;
  nameEn: string;
  nameAbbr: string;
  description: string;
  objectives: string;
  scope: string | null;
  technologies: string | null;
  expectedResults: string | null;
  majorId: number;
  majorName: string;
  majorCode: string;
  poolStatus: number;
  poolStatusName: string;
  maxStudents: number;
  mentors: MentorSummary[];
  createdAt: string;
  updatedAt: string | null;
}

export interface MentorSummary {
  mentorId: string;
  fullName: string;
}

export interface PoolTopicsResponse {
  items: PoolTopicItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PoolTopicFilters {
  majorId?: number;
  search?: string;
  poolStatus?: number;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

export interface MajorOption {
  id: number;
  name: string;
  code: string;
}

// ── Service ──────────────────────────────────────────────────────────────────

export const topicPoolService = {
  getTopics: (filters: PoolTopicFilters = {}): Promise<PoolTopicsResponse> => {
    const params = buildParams(filters);
    return apiClient.get<PoolTopicsResponse>(
      `/api/topic-pools/topics?${params.toString()}`
    );
  },

  getTopicDetail: (projectId: string): Promise<PoolTopicDetail> => {
    return apiClient.get<PoolTopicDetail>(
      `/api/topic-pools/topics/${projectId}`
    );
  },

  getMajors: (): Promise<MajorOption[]> => {
    return apiClient.get<MajorOption[]>("/api/majors");
  },

  registerTopic: (body: {
    projectId: string;
    groupId: string;
    note?: string;
  }) => {
    return apiClient.post("/api/topic-pools/registrations", body);
  },
};

// ── Helpers ──────────────────────────────────────────────────────────────────

function buildParams(filters: PoolTopicFilters): URLSearchParams {
  const params = new URLSearchParams();
  if (filters.majorId != null) params.set("majorId", String(filters.majorId));
  if (filters.search) params.set("search", filters.search);
  if (filters.poolStatus != null)
    params.set("poolStatus", String(filters.poolStatus));
  if (filters.sortBy) params.set("sortBy", filters.sortBy);
  params.set("page", String(filters.page ?? 1));
  params.set("pageSize", String(filters.pageSize ?? 12));
  return params;
}
