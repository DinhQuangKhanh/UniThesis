import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { useAuth } from "@/contexts/AuthContext";
import { apiClient } from "@/lib/apiClient";
import { useEvaluatorError } from "@/contexts/EvaluatorErrorContext";

interface EvaluatorStatsDto {
  totalAssigned: number;
  pendingCount: number;
  approvedCount: number;
  rejectedCount: number;
  needsModificationCount: number;
  reviewedCount: number;
  avgReviewDays: number | null;
}

interface PendingEvaluationDto {
  assignmentId: string;
  projectId: string;
  projectCode: string;
  projectNameVi: string;
  majorName: string;
  studentName: string;
  studentAvatar: string | null;
  assignedAt: string;
  isUrgent: boolean;
}

interface RecentReviewedDto {
  projectId: string;
  projectNameVi: string;
  result: string;
  evaluatedAt: string;
}

interface EvaluatorDashboardDto {
  stats: EvaluatorStatsDto;
  pendingEvaluations: PendingEvaluationDto[];
  recentReviewed: RecentReviewedDto[];
}

const container = {
  hidden: { opacity: 0 },
  show: { opacity: 1, transition: { staggerChildren: 0.08 } },
};

const item = {
  hidden: { opacity: 0, y: 20 },
  show: { opacity: 1, y: 0 },
};

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("vi-VN", { day: "2-digit", month: "2-digit", year: "numeric" });
}

function relativeTime(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const days = Math.floor(diff / 86400000);
  if (days === 0) return "Hôm nay";
  if (days === 1) return "Hôm qua";
  return `${days} ngày trước`;
}

function resultLabel(result: string): string {
  const map: Record<string, string> = {
    Approved: "Đã duyệt",
    NeedsModification: "Cần chỉnh sửa",
    Rejected: "Từ chối",
  };
  return map[result] ?? result;
}

export function EvaluatorDashboardPage() {
  const { user } = useAuth();
  const { showError } = useEvaluatorError();
  const [data, setData] = useState<EvaluatorDashboardDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    apiClient
      .get<EvaluatorDashboardDto>("/api/evaluator/dashboard")
      .then(setData)
      .catch((err) => showError(err.message))
      .finally(() => setLoading(false));
  }, [showError]);

  const stats = data?.stats;
  const pending = data?.pendingEvaluations ?? [];
  const recentReviewed = data?.recentReviewed ?? [];

  const reviewedPct =
    stats && stats.totalAssigned > 0 ? Math.round((stats.reviewedCount / stats.totalAssigned) * 100) : 0;

  return (
    <div className="p-8 flex flex-col gap-8">
      {/* Header */}
      <motion.header
        initial={{ opacity: 0, y: -20 }}
        animate={{ opacity: 1, y: 0 }}
        className="flex flex-col md:flex-row md:items-end justify-between gap-6"
      >
        <div className="flex flex-col gap-1">
          <h2 className="text-slate-900 text-2xl md:text-3xl font-bold tracking-tight">
            Xin chào, {user?.name ?? "Giảng viên"}
          </h2>
          <p className="text-slate-500 text-sm md:text-base">
            {loading ? (
              "Đang tải..."
            ) : (
              <>
                Bạn có <span className="text-primary font-bold">{stats?.pendingCount ?? 0} đề tài</span> đang chờ thẩm
                định.
              </>
            )}
          </p>
        </div>
        <div className="flex gap-3">
          <button className="hidden md:flex items-center justify-center gap-2 h-10 px-4 rounded-lg border border-gray-200 bg-white text-slate-900 text-sm font-semibold hover:bg-gray-50 transition-colors">
            <span className="material-symbols-outlined text-[20px]">filter_list</span>
            <span>Lọc</span>
          </button>
          <button className="flex items-center justify-center gap-2 h-10 px-5 rounded-lg bg-primary text-white text-sm font-semibold hover:bg-primary-dark transition-colors shadow-lg shadow-primary/20">
            <span className="material-symbols-outlined text-[20px]">play_circle</span>
            <span>Bắt đầu thẩm định</span>
          </button>
        </div>
      </motion.header>

      <motion.div
        variants={container}
        initial="hidden"
        animate="show"
        className="grid grid-cols-1 xl:grid-cols-12 gap-6"
      >
        {/* Main Content */}
        <div className="xl:col-span-8 flex flex-col gap-6">
          {/* Pending Reviews Table */}
          <motion.div
            variants={item}
            className="bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden flex flex-col"
          >
            <div className="px-6 py-5 border-b border-gray-100 flex justify-between items-center">
              <div className="flex items-center gap-2">
                <div className="p-1.5 rounded-md bg-primary/10 text-primary">
                  <span className="material-symbols-outlined text-xl">pending_actions</span>
                </div>
                <h3 className="text-slate-900 text-lg font-bold">Chờ thẩm định</h3>
              </div>
              <a className="text-primary text-sm font-bold hover:text-primary-dark transition-colors" href="#">
                Xem tất cả
              </a>
            </div>
            <div className="overflow-x-auto">
              {loading ? (
                <div className="flex items-center justify-center py-16 text-slate-400 gap-3">
                  <span className="material-symbols-outlined animate-spin">progress_activity</span>
                  <span className="text-sm">Đang tải...</span>
                </div>
              ) : pending.length === 0 ? (
                <div className="flex flex-col items-center justify-center py-16 text-slate-400 gap-2">
                  <span className="material-symbols-outlined text-4xl">check_circle</span>
                  <p className="text-sm font-medium">Không có đề tài nào đang chờ thẩm định</p>
                </div>
              ) : (
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-gray-50/80 border-b border-gray-100">
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider w-1/3">
                        Đề tài
                      </th>
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden sm:table-cell">
                        Sinh viên
                      </th>
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden md:table-cell">
                        Chuyên ngành
                      </th>
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden lg:table-cell">
                        Ngày giao
                      </th>
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                        Trạng thái
                      </th>
                      <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-right">
                        Thao tác
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-100">
                    {pending.map((project) => (
                      <motion.tr
                        key={project.assignmentId}
                        whileHover={{ backgroundColor: "rgb(249 250 251)" }}
                        className="group transition-colors"
                      >
                        <td className="px-6 py-4">
                          <div className="flex flex-col">
                            <span className="text-slate-900 font-semibold text-sm line-clamp-1">
                              {project.projectNameVi}
                            </span>
                            <span className="text-xs font-mono text-slate-500 mt-1">{project.projectCode}</span>
                            <span className="text-xs text-slate-500 mt-1 sm:hidden">{project.studentName}</span>
                          </div>
                        </td>
                        <td className="px-6 py-4 hidden sm:table-cell">
                          <div className="flex items-center gap-3">
                            {project.studentAvatar ? (
                              <div
                                className="size-8 rounded-full bg-gray-200 bg-cover ring-1 ring-gray-100"
                                style={{ backgroundImage: `url('${project.studentAvatar}')` }}
                              />
                            ) : (
                              <div className="size-8 rounded-full bg-primary/10 text-primary flex items-center justify-center text-xs font-bold ring-1 ring-gray-100">
                                {project.studentName.charAt(0)}
                              </div>
                            )}
                            <span className="text-slate-900 font-medium text-sm">{project.studentName || "—"}</span>
                          </div>
                        </td>
                        <td className="px-6 py-4 hidden md:table-cell">
                          <span className="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-gray-100 text-gray-700 border border-gray-200">
                            {project.majorName || "—"}
                          </span>
                        </td>
                        <td className="px-6 py-4 hidden lg:table-cell">
                          <span className="text-slate-500 text-sm font-medium">{formatDate(project.assignedAt)}</span>
                        </td>
                        <td className="px-6 py-4">
                          {project.isUrgent ? (
                            <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border bg-red-50 text-red-600 border-red-100">
                              <span className="size-1.5 rounded-full bg-red-500 animate-pulse" />
                              Khẩn cấp
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border bg-gray-100 text-gray-600 border-gray-200">
                              Đang chờ
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-4 text-right">
                          <button
                            className={`inline-flex items-center justify-center h-8 px-4 text-xs font-bold rounded-lg transition-all ${
                              project.isUrgent
                                ? "bg-primary text-white hover:bg-primary-dark shadow-sm shadow-primary/20"
                                : "bg-white border border-gray-200 text-slate-900 hover:bg-gray-50 hover:border-primary/50 hover:text-primary"
                            }`}
                          >
                            Thẩm định
                          </button>
                        </td>
                      </motion.tr>
                    ))}
                  </tbody>
                </table>
              )}
            </div>
          </motion.div>
        </div>

        {/* Right Column */}
        <div className="xl:col-span-4 flex flex-col gap-6">
          {/* Performance Card */}
          <motion.div variants={item} className="bg-white rounded-2xl border border-gray-200 p-6 shadow-sm">
            <div className="flex justify-between items-start mb-6">
              <div className="flex items-center gap-2">
                <div className="p-1.5 rounded-md bg-green-50 text-green-700">
                  <span className="material-symbols-outlined text-xl">monitoring</span>
                </div>
                <h3 className="text-slate-900 text-lg font-bold">Hiệu suất</h3>
              </div>
              <div className="px-2.5 py-1 bg-gray-100 rounded-md text-xs font-bold text-slate-500">Học kỳ này</div>
            </div>
            <div className="flex items-center gap-6">
              <div className="relative size-32 shrink-0">
                <svg className="size-full -rotate-90" viewBox="0 0 36 36">
                  <path
                    className="text-gray-100"
                    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="3"
                  />
                  <path
                    className="text-primary"
                    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                    fill="none"
                    stroke="currentColor"
                    strokeDasharray={`${reviewedPct}, 100`}
                    strokeLinecap="round"
                    strokeWidth="3"
                  />
                </svg>
                <div className="absolute inset-0 flex flex-col items-center justify-center text-center">
                  <span className="text-3xl font-bold text-slate-900 tracking-tight">{reviewedPct}%</span>
                </div>
              </div>
              <div className="flex flex-col gap-3 flex-1">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="size-2 rounded-full bg-primary" />
                    <span className="text-sm text-slate-500 font-medium">Đã thẩm định</span>
                  </div>
                  <span className="text-sm font-bold text-slate-900">{stats?.reviewedCount ?? "—"}</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="size-2 rounded-full bg-gray-200" />
                    <span className="text-sm text-slate-500 font-medium">Đang chờ</span>
                  </div>
                  <span className="text-sm font-bold text-slate-900">{stats?.pendingCount ?? "—"}</span>
                </div>
                <div className="mt-1 pt-3 border-t border-gray-100 flex justify-between items-center">
                  <span className="text-xs text-slate-500 font-medium">Thời gian TB</span>
                  <span className="text-xs font-bold text-green-600 bg-green-50 px-2 py-0.5 rounded-full">
                    {stats?.avgReviewDays != null ? `${stats.avgReviewDays} ngày` : "—"}
                  </span>
                </div>
              </div>
            </div>
          </motion.div>

          {/* Recent Reviews Card */}
          <motion.div
            variants={item}
            className="bg-white rounded-2xl border border-gray-200 p-6 shadow-sm flex flex-col gap-4"
          >
            <div className="flex justify-between items-center mb-1">
              <div className="flex items-center gap-2">
                <div className="p-1.5 rounded-md bg-amber-50 text-amber-700">
                  <span className="material-symbols-outlined text-xl">comment</span>
                </div>
                <h3 className="text-slate-900 text-lg font-bold">Thẩm định gần đây</h3>
              </div>
            </div>
            {loading ? (
              <div className="flex items-center justify-center py-8 text-slate-400">
                <span className="material-symbols-outlined animate-spin text-xl">progress_activity</span>
              </div>
            ) : recentReviewed.length === 0 ? (
              <p className="text-sm text-slate-400 text-center py-6">Chưa có thẩm định nào</p>
            ) : (
              <div className="flex flex-col gap-3">
                {recentReviewed.map((r) => (
                  <div
                    key={`${r.projectId}-${r.evaluatedAt}`}
                    className="group cursor-pointer p-4 rounded-xl bg-gray-50 hover:bg-white hover:shadow-md transition-all border border-transparent hover:border-gray-100"
                  >
                    <div className="flex justify-between items-start mb-2">
                      <span className="text-xs font-bold text-slate-900 line-clamp-1 flex-1 pr-2">
                        {r.projectNameVi}
                      </span>
                      <span className="text-[10px] font-medium text-slate-500 bg-white px-1.5 py-0.5 rounded border border-gray-100 shrink-0">
                        {relativeTime(r.evaluatedAt)}
                      </span>
                    </div>
                    <p className="text-xs text-slate-500 leading-relaxed group-hover:text-slate-900 transition-colors">
                      {resultLabel(r.result)}
                    </p>
                  </div>
                ))}
              </div>
            )}
            <button className="w-full py-2.5 text-xs font-bold text-primary hover:text-primary-dark hover:bg-primary/5 rounded-lg transition-colors mt-2">
              Xem toàn bộ lịch sử
            </button>
          </motion.div>
        </div>
      </motion.div>
    </div>
  );
}
