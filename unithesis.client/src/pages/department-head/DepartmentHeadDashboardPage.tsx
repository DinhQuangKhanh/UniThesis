export function DepartmentHeadDashboardPage() {
  return (
    <div className="p-8">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-slate-900">Tổng quan - Chủ nhiệm bộ môn</h1>
        <p className="text-slate-500 mt-1">Quản lý và phân công thẩm định đề tài trong bộ môn</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl border border-slate-200 p-6">
          <div className="flex items-center gap-3 mb-2">
            <span className="material-symbols-outlined text-primary">topic</span>
            <h3 className="font-semibold text-slate-700">Đề tài bộ môn</h3>
          </div>
          <p className="text-3xl font-bold text-slate-900">--</p>
          <p className="text-sm text-slate-500 mt-1">Tổng số đề tài</p>
        </div>

        <div className="bg-white rounded-xl border border-slate-200 p-6">
          <div className="flex items-center gap-3 mb-2">
            <span className="material-symbols-outlined text-amber-500">pending_actions</span>
            <h3 className="font-semibold text-slate-700">Chờ phân công</h3>
          </div>
          <p className="text-3xl font-bold text-slate-900">--</p>
          <p className="text-sm text-slate-500 mt-1">Đề tài chưa có evaluator</p>
        </div>

        <div className="bg-white rounded-xl border border-slate-200 p-6">
          <div className="flex items-center gap-3 mb-2">
            <span className="material-symbols-outlined text-green-500">check_circle</span>
            <h3 className="font-semibold text-slate-700">Đã phân công</h3>
          </div>
          <p className="text-3xl font-bold text-slate-900">--</p>
          <p className="text-sm text-slate-500 mt-1">Đề tài đã có đủ evaluator</p>
        </div>
      </div>
    </div>
  );
}
