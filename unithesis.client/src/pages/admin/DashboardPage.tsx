import { motion } from 'framer-motion'
import { Header } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: {
        opacity: 1,
        transition: {
            staggerChildren: 0.1
        }
    }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

export function DashboardPage() {
    return (
        <>
            <Header title="Tổng Quan Hệ Thống" />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="space-y-6"
                >
                    {/* Stats Cards */}
                    <motion.div variants={item} className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                        <StatsCard
                            icon="group"
                            iconColor="text-blue-600"
                            iconBg="bg-blue-500/10"
                            value="2,450"
                            label="Tổng Sinh Viên"
                            change="+12% vs last term"
                        />
                        <StatsCard
                            icon="school"
                            iconColor="text-green-600"
                            iconBg="bg-green-500/10"
                            value="142"
                            label="Giảng Viên Hướng Dẫn"
                            change="Active"
                        />
                        <StatsCard
                            icon="folder_open"
                            iconColor="text-purple-600"
                            iconBg="bg-purple-500/10"
                            value="856"
                            label="Đề Tài Đăng Ký"
                            change="Current Sem"
                        />
                        <StatsCard
                            icon="pending_actions"
                            iconColor="text-orange-600"
                            iconBg="bg-orange-500/10"
                            value="45"
                            label="Chờ Duyệt Gấp"
                            change="High Priority"
                            changeColor="text-error"
                        />
                    </motion.div>

                    {/* Progress & Chart Row */}
                    <motion.div variants={item} className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                        {/* Semester Progress */}
                        <div className="lg:col-span-2 bento-card p-6 rounded-md flex flex-col h-full">
                            <div className="flex justify-between items-center mb-6">
                                <div>
                                    <h3 className="text-slate-800 text-lg font-bold">Tiến Độ Kỳ Học: Spring 2024</h3>
                                    <p className="text-slate-500 text-sm">
                                        Giai đoạn hiện tại: <span className="text-primary font-semibold">Thẩm Định Đề Tài</span>
                                    </p>
                                </div>
                                <button className="flex items-center gap-2 bg-primary/5 hover:bg-primary/10 text-primary px-3 py-1.5 rounded text-sm font-medium transition-colors border border-primary/10">
                                    <span className="material-symbols-outlined text-[18px]">edit_calendar</span>
                                    Điều chỉnh
                                </button>
                            </div>

                            {/* Timeline */}
                            <div className="relative py-4 px-2">
                                <div className="absolute top-1/2 left-0 w-full h-1 bg-slate-200 -translate-y-1/2 rounded-full z-0" />
                                <div className="absolute top-1/2 left-0 w-[45%] h-1 bg-gradient-to-r from-blue-600 to-primary -translate-y-1/2 rounded-full z-0" />

                                <div className="relative z-10 flex justify-between w-full">
                                    <TimelineStep icon="check" label="Đăng Ký" status="completed" />
                                    <TimelineStep icon="fact_check" label="Thẩm Định" status="current" subtitle="Đang diễn ra (12 ngày còn lại)" />
                                    <TimelineStep icon="code" label="Thực Hiện" status="pending" />
                                    <TimelineStep icon="shield" label="Bảo Vệ" status="pending" />
                                </div>
                            </div>

                            {/* Info boxes */}
                            <div className="mt-auto pt-6 border-t border-slate-100 flex gap-4">
                                <div className="flex-1 bg-slate-50 p-3 rounded border border-slate-200">
                                    <p className="text-xs text-slate-500 mb-1">Hạn chót thẩm định</p>
                                    <p className="text-sm font-semibold text-slate-800">15/04/2024</p>
                                </div>
                                <div className="flex-1 bg-slate-50 p-3 rounded border border-slate-200">
                                    <p className="text-xs text-slate-500 mb-1">Tổng hội đồng</p>
                                    <p className="text-sm font-semibold text-slate-800">12 Hội đồng</p>
                                </div>
                                <div className="flex-1 bg-slate-50 p-3 rounded border border-slate-200">
                                    <p className="text-xs text-slate-500 mb-1">Giảng viên phản biện</p>
                                    <p className="text-sm font-semibold text-slate-800">45 Giảng viên</p>
                                </div>
                            </div>
                        </div>

                        {/* Approval Rate Chart */}
                        <div className="bento-card p-6 rounded-md flex flex-col">
                            <h3 className="text-slate-800 text-lg font-bold mb-4">Tỷ Lệ Duyệt Đề Tài</h3>
                            <div className="flex-1 flex flex-col items-center justify-center relative">
                                <div
                                    className="relative w-40 h-40 rounded-full bg-slate-100 flex items-center justify-center"
                                    style={{
                                        background: 'conic-gradient(#5F8F61 0% 65%, #A64B4B 65% 75%, #2c6090 75% 100%)',
                                    }}
                                >
                                    <div className="w-28 h-28 bg-white rounded-full flex flex-col items-center justify-center z-10">
                                        <span className="text-3xl font-bold text-slate-800">65%</span>
                                        <span className="text-[10px] text-slate-400 uppercase tracking-wide">Đã duyệt</span>
                                    </div>
                                </div>
                                <div className="w-full mt-6 space-y-3">
                                    <LegendItem color="bg-success" label="Đã duyệt" value="556" />
                                    <LegendItem color="bg-primary" label="Chờ duyệt" value="214" />
                                    <LegendItem color="bg-error" label="Từ chối" value="86" />
                                </div>
                            </div>
                        </div>
                    </motion.div>

                    {/* Support Table */}
                    <motion.div variants={item} className="bento-card rounded-md overflow-hidden flex flex-col">
                        <div className="p-6 border-b border-slate-200 flex justify-between items-center">
                            <div className="flex items-center gap-2">
                                <span className="material-symbols-outlined text-slate-500">health_and_safety</span>
                                <h3 className="text-slate-800 text-lg font-bold">Hệ Thống & Yêu Cầu Hỗ Trợ</h3>
                            </div>
                            <button className="text-xs font-medium text-primary hover:text-primary-light transition-colors">
                                Xem tất cả
                            </button>
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left text-sm text-slate-500">
                                <thead className="bg-slate-50 text-xs uppercase font-semibold text-slate-600 border-b border-slate-200">
                                    <tr>
                                        <th className="px-6 py-4">ID</th>
                                        <th className="px-6 py-4">Vấn đề / Yêu cầu</th>
                                        <th className="px-6 py-4">Người báo cáo</th>
                                        <th className="px-6 py-4">Thời gian</th>
                                        <th className="px-6 py-4">Trạng thái</th>
                                        <th className="px-6 py-4 text-right">Hành động</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100">
                                    <SupportRow
                                        id="#TICK-4923"
                                        icon="error"
                                        iconColor="text-error"
                                        issue="Lỗi upload file PDF đồ án"
                                        reporter="Nguyễn Văn A (SV)"
                                        time="10 phút trước"
                                        status="Chưa xử lý"
                                        statusColor="bg-error/10 text-error border-error/20"
                                    />
                                    <SupportRow
                                        id="#TICK-4922"
                                        icon="help"
                                        iconColor="text-blue-500"
                                        issue="Yêu cầu đổi tên đề tài (Quá hạn)"
                                        reporter="Trần Thị B (SV)"
                                        time="2 giờ trước"
                                        status="Đang xem xét"
                                        statusColor="bg-blue-500/10 text-blue-600 border-blue-500/20"
                                    />
                                    <SupportRow
                                        id="#SYS-LOG-01"
                                        icon="warning"
                                        iconColor="text-yellow-500"
                                        issue="High Server Load detected"
                                        reporter="System Monitor"
                                        time="4 giờ trước"
                                        status="Đã giải quyết"
                                        statusColor="bg-success/10 text-success border-success/20"
                                    />
                                </tbody>
                            </table>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}

// Helper Components
function StatsCard({
    icon,
    iconColor,
    iconBg,
    value,
    label,
    change,
    changeColor = 'text-slate-500',
}: {
    icon: string
    iconColor: string
    iconBg: string
    value: string
    label: string
    change: string
    changeColor?: string
}) {
    return (
        <motion.div
            whileHover={{ scale: 1.02, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}
            className="bento-card p-5 rounded-md flex flex-col justify-between group transition-shadow"
        >
            <div className="flex justify-between items-start">
                <div className={`p-2 ${iconBg} rounded-md ${iconColor}`}>
                    <span className="material-symbols-outlined">{icon}</span>
                </div>
                <span className={`text-xs font-medium ${changeColor}`}>{change}</span>
            </div>
            <div className="mt-4">
                <h3 className="text-2xl font-bold text-slate-800 tracking-tight">{value}</h3>
                <p className="text-sm text-slate-500 font-medium">{label}</p>
            </div>
        </motion.div>
    )
}

function TimelineStep({
    icon,
    label,
    status,
    subtitle,
}: {
    icon: string
    label: string
    status: 'completed' | 'current' | 'pending'
    subtitle?: string
}) {
    const isCurrent = status === 'current'
    const isPending = status === 'pending'

    return (
        <div className={`flex flex-col items-center gap-2 group cursor-pointer ${isPending ? 'opacity-50' : ''}`}>
            <div
                className={`rounded-full flex items-center justify-center ${isCurrent
                    ? 'w-10 h-10 bg-primary text-white shadow-lg shadow-primary/20 ring-4 ring-white'
                    : status === 'completed'
                        ? 'w-8 h-8 bg-white border-2 border-primary text-primary shadow-sm'
                        : 'w-8 h-8 bg-white border-2 border-slate-300 text-slate-400'
                    }`}
            >
                <span className={`material-symbols-outlined ${isCurrent ? 'text-[20px]' : 'text-[16px]'}`}>{icon}</span>
            </div>
            <p className={`text-xs font-medium ${isCurrent ? 'text-sm font-bold text-slate-800' : 'text-slate-500'}`}>
                {label}
            </p>
            {subtitle && (
                <span className="text-[10px] text-primary-light font-semibold bg-primary/10 px-2 py-0.5 rounded">
                    {subtitle}
                </span>
            )}
        </div>
    )
}

function LegendItem({ color, label, value }: { color: string; label: string; value: string }) {
    return (
        <div className="flex justify-between items-center text-sm">
            <div className="flex items-center gap-2">
                <div className={`w-2 h-2 rounded-full ${color}`} />
                <span className="text-slate-600">{label}</span>
            </div>
            <span className="font-medium text-slate-800">{value}</span>
        </div>
    )
}

function SupportRow({
    id,
    icon,
    iconColor,
    issue,
    reporter,
    time,
    status,
    statusColor,
}: {
    id: string
    icon: string
    iconColor: string
    issue: string
    reporter: string
    time: string
    status: string
    statusColor: string
}) {
    return (
        <tr className="hover:bg-slate-50 transition-colors">
            <td className="px-6 py-4 font-mono text-xs text-slate-600">{id}</td>
            <td className="px-6 py-4">
                <div className="flex items-center gap-2">
                    <span className={`material-symbols-outlined ${iconColor} text-[18px]`}>{icon}</span>
                    <span className="text-slate-800 font-medium">{issue}</span>
                </div>
            </td>
            <td className="px-6 py-4 text-slate-600">{reporter}</td>
            <td className="px-6 py-4 text-slate-600">{time}</td>
            <td className="px-6 py-4">
                <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded text-xs font-medium border ${statusColor}`}>
                    {status}
                </span>
            </td>
            <td className="px-6 py-4 text-right">
                <button className="text-slate-400 hover:text-slate-600 transition-colors">
                    <span className="material-symbols-outlined text-[20px]">more_vert</span>
                </button>
            </td>
        </tr>
    )
}
