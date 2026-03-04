import { useState, useEffect, useCallback } from "react";
import { motion } from "framer-motion";
import { Header } from "@/components/layout";
import { activityLogService, type ActivityLogItem, type ActivityLogResponse } from "@/lib/activityLogService";

const container = {
  hidden: { opacity: 0 },
  show: { opacity: 1, transition: { staggerChildren: 0.05 } },
};

const item = {
  hidden: { opacity: 0, y: 20 },
  show: { opacity: 1, y: 0 },
};

// ── Constants ──────────────────────────────────────────────
const roleTabs = [
  { key: "", label: "Tất cả", icon: "groups" },
  { key: "admin", label: "Admin", icon: "admin_panel_settings" },
  { key: "mentor", label: "Mentor", icon: "school" },
  { key: "evaluator", label: "Evaluator", icon: "fact_check" },
  { key: "student", label: "Sinh viên", icon: "person" },
];

const severityConfig: Record<string, { bg: string; text: string; border: string; icon: string }> = {
  info: { bg: "bg-blue-50", text: "text-blue-700", border: "border-blue-200", icon: "info" },
  warning: { bg: "bg-yellow-50", text: "text-yellow-700", border: "border-yellow-200", icon: "warning" },
  error: { bg: "bg-red-50", text: "text-red-700", border: "border-red-200", icon: "error" },
  critical: { bg: "bg-red-100", text: "text-red-900", border: "border-red-300", icon: "emergency" },
};

const roleColors: Record<string, string> = {
  admin: "bg-slate-800 text-slate-100 border-slate-700",
  mentor: "bg-purple-50 text-purple-700 border-purple-100",
  evaluator: "bg-orange-50 text-orange-700 border-orange-100",
  student: "bg-slate-100 text-slate-700 border-slate-200",
};

const PAGE_SIZE = 20;

// ── Component ──────────────────────────────────────────────
export function ActivityLogsPage() {
  const [activeRole, setActiveRole] = useState("");
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [page, setPage] = useState(1);
  const [data, setData] = useState<ActivityLogResponse | null>(null);
  const [loading, setLoading] = useState(false);

  // Debounce search input
  useEffect(() => {
    const id = setTimeout(() => setDebouncedSearch(search), 400);
    return () => clearTimeout(id);
  }, [search]);

  // Fetch data
  const fetchLogs = useCallback(async () => {
    setLoading(true);
    try {
      const result = await activityLogService.getLogs({
        role: activeRole || undefined,
        search: debouncedSearch || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      setData(result);
    } catch {
      // silently fail — could integrate error toast later
    } finally {
      setLoading(false);
    }
  }, [activeRole, debouncedSearch, page]);

  useEffect(() => {
    fetchLogs();
  }, [fetchLogs]);

  // Reset page when filters change
  useEffect(() => {
    setPage(1);
  }, [activeRole, debouncedSearch]);

  const totalPages = data?.totalPages ?? 1;

  return (
    <>
      <Header title="Nhật Ký Hoạt Động" showSearch={false} />

      <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
        <motion.div variants={container} initial="hidden" animate="show" className="flex flex-col h-full">
          {/* ── Filters ─────────────────────────────────────── */}
          <motion.div variants={item} className="flex flex-col lg:flex-row lg:items-center justify-between gap-4 mb-6">
            {/* Role tabs */}
            <div className="flex bg-white p-1 rounded-lg border border-slate-200 shadow-sm overflow-x-auto scrollbar-hide">
              {roleTabs.map((tab) => (
                <button
                  key={tab.key}
                  onClick={() => setActiveRole(tab.key)}
                  className={`flex items-center gap-1.5 px-4 py-2 text-sm font-medium rounded-md whitespace-nowrap transition-all ${
                    activeRole === tab.key
                      ? "bg-primary text-white shadow-sm"
                      : "text-slate-600 hover:bg-slate-50 hover:text-slate-900"
                  }`}
                >
                  <span className="material-symbols-outlined text-[18px]">{tab.icon}</span>
                  {tab.label}
                </button>
              ))}
            </div>

            {/* Search */}
            <div className="flex items-center gap-3 w-full lg:w-auto">
              <div className="relative flex-1 lg:w-80">
                <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-slate-400 text-[18px]">
                  search
                </span>
                <input
                  className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary bg-white placeholder-slate-400 text-slate-700"
                  placeholder="Tìm theo tên, email, hành động..."
                  type="text"
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                />
              </div>
              <button
                onClick={fetchLogs}
                className="flex items-center gap-2 px-4 py-2 bg-white border border-slate-200 text-slate-700 text-sm font-medium rounded-md hover:bg-slate-50 hover:text-primary transition-colors whitespace-nowrap"
              >
                <span className="material-symbols-outlined text-[20px]">refresh</span>
                <span className="hidden sm:inline">Làm mới</span>
              </button>
            </div>
          </motion.div>

          {/* ── Summary Cards ────────────────────────────────── */}
          <motion.div variants={item} className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
            <SummaryCard
              icon="groups"
              iconColor="text-primary"
              iconBg="bg-primary/10"
              value={data?.totalCount ?? 0}
              label="Tổng bản ghi"
            />
            <SummaryCard
              icon="admin_panel_settings"
              iconColor="text-slate-700"
              iconBg="bg-slate-200"
              value={countByRole(data, "admin")}
              label="Admin"
            />
            <SummaryCard
              icon="school"
              iconColor="text-purple-600"
              iconBg="bg-purple-100"
              value={countByRole(data, "mentor")}
              label="Mentor"
            />
            <SummaryCard
              icon="person"
              iconColor="text-blue-600"
              iconBg="bg-blue-100"
              value={countByRole(data, "student")}
              label="Sinh viên"
            />
          </motion.div>

          {/* ── Table ─────────────────────────────────────────── */}
          <motion.div
            variants={item}
            className="bento-card rounded-md overflow-hidden bg-white flex flex-col flex-1 min-h-0"
          >
            <div className="overflow-auto flex-1">
              <table className="w-full text-left text-sm text-slate-600">
                <thead className="bg-slate-50 text-xs uppercase font-bold text-slate-500 border-b border-slate-200 sticky top-0 z-10">
                  <tr>
                    <th className="px-6 py-4">Người dùng</th>
                    <th className="px-6 py-4">Vai trò</th>
                    <th className="px-6 py-4">Hành động</th>
                    <th className="px-6 py-4">Đối tượng</th>
                    <th className="px-6 py-4">Mức độ</th>
                    <th className="px-6 py-4">Thời gian</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {loading && !data ? (
                    <tr>
                      <td colSpan={6} className="px-6 py-20 text-center text-slate-400">
                        <span className="material-symbols-outlined animate-spin text-[28px] mb-2 block">
                          progress_activity
                        </span>
                        Đang tải dữ liệu...
                      </td>
                    </tr>
                  ) : data && data.items.length > 0 ? (
                    data.items.map((log) => <LogRow key={log.id} log={log} />)
                  ) : (
                    <tr>
                      <td colSpan={6} className="px-6 py-20 text-center text-slate-400">
                        <span className="material-symbols-outlined text-[40px] mb-2 block text-slate-300">
                          search_off
                        </span>
                        Không tìm thấy bản ghi nào
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            {data && data.totalPages > 1 && (
              <div className="p-4 border-t border-slate-200 flex items-center justify-between bg-white shrink-0">
                <span className="text-sm text-slate-500 hidden sm:inline">
                  Hiển thị{" "}
                  <span className="font-medium text-slate-900">
                    {(page - 1) * PAGE_SIZE + 1}-{Math.min(page * PAGE_SIZE, data.totalCount)}
                  </span>{" "}
                  trên <span className="font-medium text-slate-900">{data.totalCount.toLocaleString()}</span> bản ghi
                </span>
                <div className="flex gap-1 w-full sm:w-auto justify-center sm:justify-end">
                  <button
                    disabled={page <= 1}
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm disabled:opacity-50 transition-colors"
                  >
                    Trước
                  </button>
                  {getPageNumbers(page, totalPages).map((p, i) =>
                    p === "..." ? (
                      <span key={`dot-${i}`} className="px-2 py-1 text-slate-400 hidden sm:inline">
                        ...
                      </span>
                    ) : (
                      <button
                        key={p}
                        onClick={() => setPage(p as number)}
                        className={`px-3 py-1 rounded text-sm transition-colors ${
                          page === p
                            ? "bg-primary text-white hover:bg-primary-light"
                            : "border border-slate-200 hover:bg-slate-50 text-slate-600"
                        }`}
                      >
                        {p}
                      </button>
                    ),
                  )}
                  <button
                    disabled={page >= totalPages}
                    onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                    className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm disabled:opacity-50 transition-colors"
                  >
                    Sau
                  </button>
                </div>
              </div>
            )}
          </motion.div>
        </motion.div>
      </div>
    </>
  );
}

// ── Helper Components ──────────────────────────────────────

function SummaryCard({
  icon,
  iconColor,
  iconBg,
  value,
  label,
}: {
  icon: string;
  iconColor: string;
  iconBg: string;
  value: number;
  label: string;
}) {
  return (
    <div className="bento-card p-4 rounded-md flex items-center gap-4">
      <div className={`${iconBg} p-2.5 rounded-lg`}>
        <span className={`material-symbols-outlined ${iconColor} text-[22px]`}>{icon}</span>
      </div>
      <div>
        <p className="text-xl font-bold text-slate-800">{value.toLocaleString()}</p>
        <p className="text-xs text-slate-500">{label}</p>
      </div>
    </div>
  );
}

function LogRow({ log }: { log: ActivityLogItem }) {
  const sev = severityConfig[log.severity] ?? severityConfig.info;
  const roleColor = roleColors[log.userRole] ?? roleColors.student;

  return (
    <motion.tr whileHover={{ backgroundColor: "rgb(248 250 252)" }} className="transition-colors group">
      {/* User */}
      <td className="px-6 py-4">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center text-xs font-bold shrink-0">
            {getInitials(log.userName)}
          </div>
          <div className="min-w-0">
            <p className="font-semibold text-slate-800 truncate">{log.userName}</p>
            <p className="text-xs text-slate-500 truncate">{log.userEmail ?? log.userId}</p>
          </div>
        </div>
      </td>
      {/* Role */}
      <td className="px-6 py-4">
        <span
          className={`inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold border capitalize ${roleColor}`}
        >
          {log.userRole}
        </span>
      </td>
      {/* Action */}
      <td className="px-6 py-4">
        <div>
          <p className="font-medium text-slate-800">{formatAction(log.action)}</p>
          {log.category && <p className="text-xs text-slate-400">{log.category}</p>}
        </div>
      </td>
      {/* Entity */}
      <td className="px-6 py-4">
        {log.entityType ? (
          <span className="text-xs bg-slate-50 text-slate-600 px-2 py-1 rounded border border-slate-200 font-mono">
            {log.entityType}
          </span>
        ) : (
          <span className="text-xs text-slate-400">—</span>
        )}
      </td>
      {/* Severity */}
      <td className="px-6 py-4">
        <span
          className={`inline-flex items-center gap-1 px-2 py-0.5 rounded text-xs font-medium border ${sev.bg} ${sev.text} ${sev.border}`}
        >
          <span className="material-symbols-outlined text-[14px]">{sev.icon}</span>
          {log.severity}
        </span>
      </td>
      {/* Timestamp */}
      <td className="px-6 py-4 whitespace-nowrap text-xs text-slate-500">{formatTimestamp(log.timestamp)}</td>
    </motion.tr>
  );
}

// ── Utilities ───────────────────────────────────────────────

function getInitials(name: string): string {
  return name
    .split(" ")
    .filter(Boolean)
    .map((w) => w[0])
    .slice(0, 2)
    .join("")
    .toUpperCase();
}

function formatAction(action: string): string {
  // Convert PascalCase to spaced words: "MemberRemoved" → "Member Removed"
  return action.replace(/([a-z])([A-Z])/g, "$1 $2");
}

function formatTimestamp(iso: string): string {
  const d = new Date(iso);
  const now = new Date();
  const diffMs = now.getTime() - d.getTime();
  const diffMin = Math.floor(diffMs / 60000);

  if (diffMin < 1) return "Vừa xong";
  if (diffMin < 60) return `${diffMin} phút trước`;
  if (diffMin < 1440) return `${Math.floor(diffMin / 60)} giờ trước`;
  if (diffMin < 10080) return `${Math.floor(diffMin / 1440)} ngày trước`;

  return (
    d.toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" }) +
    " " +
    d.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" })
  );
}

function countByRole(data: ActivityLogResponse | null, role: string): number {
  if (!data) return 0;
  return data.items.filter((i) => i.userRole === role).length;
}

function getPageNumbers(current: number, total: number): (number | "...")[] {
  if (total <= 5) return Array.from({ length: total }, (_, i) => i + 1);
  const pages: (number | "...")[] = [1];
  if (current > 3) pages.push("...");
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
    pages.push(i);
  }
  if (current < total - 2) pages.push("...");
  pages.push(total);
  return pages;
}
