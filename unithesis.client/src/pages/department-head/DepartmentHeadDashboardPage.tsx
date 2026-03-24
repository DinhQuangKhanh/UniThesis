import { useState, useEffect, useCallback } from "react";
import { Link } from "react-router-dom";
import {
  departmentHeadService,
  groupProjects,
  type DepartmentProjectsResponse,
  type GroupedProjects,
} from "@/lib/departmentHeadService";

export function DepartmentHeadDashboardPage() {
  const [raw, setRaw] = useState<DepartmentProjectsResponse | null>(null);
  const [grouped, setGrouped] = useState<GroupedProjects | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const resp = await departmentHeadService.getProjects();
      setRaw(resp ?? null);
      setGrouped(groupProjects(resp));
    } catch {
      // silently ignore on dashboard
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const stats = [
    {
      label: "Đề tài bộ môn",
      icon: "topic",
      iconColor: "text-primary",
      value: loading ? "—" : raw?.totalCount ?? 0,
      sub: "Tổng số đề tài",
    },
    {
      label: "Chờ phân công",
      icon: "pending_actions",
      iconColor: "text-amber-500",
      value: loading ? "—" : raw?.pendingAssignmentCount ?? 0,
      sub: "Đề tài chưa có evaluator",
    },
    {
      label: "Đang thẩm định",
      icon: "rate_review",
      iconColor: "text-blue-500",
      value: loading ? "—" : raw?.inEvaluationCount ?? 0,
      sub: "Đề tài đang chờ kết quả",
    },
    {
      label: "Cần quyết định",
      icon: "gavel",
      iconColor: "text-orange-500",
      value: loading ? "—" : raw?.needsFinalDecisionCount ?? 0,
      sub: "Kết quả không thống nhất",
      highlight: (raw?.needsFinalDecisionCount ?? 0) > 0,
    },
    {
      label: "Hoàn thành",
      icon: "check_circle",
      iconColor: "text-green-500",
      value: loading ? "—" : raw?.completedCount ?? 0,
      sub: "Đã có kết quả cuối cùng",
    },
  ];

  return (
    <div className="p-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-slate-900">Tổng quan - Chủ nhiệm bộ môn</h1>
        <p className="text-slate-500 mt-1">Quản lý và phân công thẩm định đề tài trong bộ môn</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-5 gap-4">
        {stats.map((s) => (
          <div
            key={s.label}
            className={`bg-white rounded-xl border p-5 ${
              s.highlight ? "border-orange-300 ring-1 ring-orange-200" : "border-slate-200"
            }`}
          >
            <div className="flex items-center gap-3 mb-2">
              <span className={`material-symbols-outlined ${s.iconColor}`}>{s.icon}</span>
              <h3 className="font-semibold text-slate-700 text-sm">{s.label}</h3>
            </div>
            <p className={`text-3xl font-bold ${s.highlight ? "text-orange-600" : "text-slate-900"}`}>{s.value}</p>
            <p className="text-xs text-slate-500 mt-1">{s.sub}</p>
          </div>
        ))}
      </div>

      {/* Quick actions */}
      {(grouped?.needsDecision?.length ?? 0) > 0 && (
        <div className="mt-6 p-4 bg-amber-50 border border-amber-200 rounded-xl flex items-center gap-4">
          <span className="material-symbols-outlined text-amber-500 text-[28px]">warning</span>
          <div className="flex-1">
            <p className="font-semibold text-amber-800">
              Có {grouped?.needsDecision?.length ?? 0} đề tài cần quyết định cuối cùng
            </p>
            <p className="text-sm text-amber-600">Kết quả thẩm định của 2 evaluator không thống nhất</p>
          </div>
          <Link
            to="/department-head/assign"
            className="flex items-center gap-2 px-4 py-2 bg-amber-500 text-white text-sm font-medium rounded-lg hover:bg-amber-600 transition-colors shrink-0"
          >
            <span className="material-symbols-outlined text-[18px]">gavel</span>
            Xem ngay
          </Link>
        </div>
      )}

      {(grouped?.pendingAssignment?.length ?? 0) > 0 && (
        <div className="mt-4 p-4 bg-blue-50 border border-blue-200 rounded-xl flex items-center gap-4">
          <span className="material-symbols-outlined text-blue-500 text-[28px]">assignment_ind</span>
          <div className="flex-1">
            <p className="font-semibold text-blue-800">
              Có {grouped?.pendingAssignment?.length ?? 0} đề tài chờ phân công evaluator
            </p>
            <p className="text-sm text-blue-600">Vui lòng phân công 2 evaluator cho mỗi đề tài</p>
          </div>
          <Link
            to="/department-head/assign"
            className="flex items-center gap-2 px-4 py-2 bg-primary text-white text-sm font-medium rounded-lg hover:bg-primary/90 transition-colors shrink-0"
          >
            <span className="material-symbols-outlined text-[18px]">person_add</span>
            Phân công
          </Link>
        </div>
      )}
    </div>
  );
}
