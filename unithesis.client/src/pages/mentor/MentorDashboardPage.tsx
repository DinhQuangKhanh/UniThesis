import { useState } from 'react'
import { motion } from 'framer-motion'
import { RegisterTopicModal } from '@/components/mentor/RegisterTopicModal'
import { NotificationDropdown } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.08 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const stats = [
    { label: 'Nhóm hướng dẫn', value: '05', icon: 'groups', color: 'blue', trend: '+1 nhóm mới', trendColor: 'text-emerald-600' },
    { label: 'Sinh viên tham gia', value: '18', icon: 'school', color: 'indigo', progress: 85, progressLabel: '85% hoạt động tích cực' },
    { label: 'Đề tài chờ duyệt', value: '02', icon: 'pending_actions', color: 'amber', trend: 'Cần phản hồi', trendColor: 'text-amber-600' },
    { label: 'Hạn nộp báo cáo', value: '15 Thg 10', icon: 'event_busy', color: 'rose', note: 'Báo cáo tiến độ GĐ 1' },
]

const recentProjects = [
    { name: 'AI trong chẩn đoán Y Khoa', code: 'PROJ-2024-001', icon: 'medical_services', iconColor: 'blue', student: 'Trần Văn B', status: 'pending' },
    { name: 'Hệ thống E-commerce Local', code: 'PROJ-2024-045', icon: 'shopping_cart', iconColor: 'emerald', student: 'Lê Thị C', status: 'approved' },
    { name: 'Robot hỗ trợ người già', code: 'PROJ-2024-089', icon: 'smart_toy', iconColor: 'rose', student: 'Phạm Văn D', status: 'revision' },
]

const feedbacks = [
    { author: 'TS. Lê (Thẩm định)', time: '10p trước', content: 'Đã yêu cầu chỉnh sửa đề tài "Robot hỗ trợ người già". Cần làm rõ tính khả thi của module cảm biến.', icon: 'gavel', color: 'rose' },
    { author: 'Hội đồng A', time: '2h trước', content: 'Đề tài "E-commerce Local" đã được phê duyệt. Nhóm có thể bắt đầu triển khai.', icon: 'check_circle', color: 'emerald' },
    { author: 'ThS. Phạm', time: '1 ngày trước', content: 'Đã thêm bình luận vào đề tài "AI trong chẩn đoán Y Khoa".', icon: 'comment', color: 'blue' },
]

export function MentorDashboardPage() {
    const [isModalOpen, setIsModalOpen] = useState(false)

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-white border-b border-slate-200 flex-shrink-0 z-50 shadow-sm">
                <div className="flex items-center gap-4">
                    <h2 className="text-slate-800 text-lg font-bold">Tổng quan</h2>
                </div>
                <div className="flex items-center gap-4">
                    <div className="relative hidden md:block">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-slate-400 text-[20px]">search</span>
                        </div>
                        <input
                            type="text"
                            className="block w-64 pl-10 pr-3 py-2 border-none rounded-lg bg-slate-100 text-slate-900 placeholder-slate-500 focus:outline-none focus:bg-white focus:ring-1 focus:ring-primary sm:text-sm transition-all"
                            placeholder="Tìm kiếm sinh viên, đề tài..."
                        />
                    </div>
                    <NotificationDropdown role="mentor" />
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8">
                <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
                    {/* Welcome & Action */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-end justify-between gap-4">
                        <div>
                            <h2 className="text-3xl font-extrabold text-slate-900 tracking-tight">Chào mừng trở lại, Mentor Nguyễn!</h2>
                            <p className="text-slate-500 mt-1 text-base">
                                Bạn có <span className="text-amber-600 font-semibold">2 phản hồi mới</span> từ hội đồng thẩm định cần xem xét.
                            </p>
                        </div>
                        <button
                            onClick={() => setIsModalOpen(true)}
                            className="inline-flex items-center justify-center gap-2 bg-primary hover:bg-primary/90 text-white px-5 py-2.5 rounded-lg font-semibold shadow-md shadow-blue-900/10 transition-all active:scale-95"
                        >
                            <span className="material-symbols-outlined text-[20px]">add_circle</span>
                            <span>Đăng ký đề tài mới</span>
                        </button>
                    </motion.div>

                    {/* Stats Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
                        {stats.map((stat, index) => (
                            <motion.div
                                key={stat.label}
                                variants={item}
                                className="bg-white p-5 rounded-xl border border-slate-100 shadow-sm flex flex-col justify-between h-36 relative overflow-hidden group hover:border-primary/30 transition-all"
                            >
                                <div className="flex justify-between items-start z-10">
                                    <div>
                                        <p className="text-slate-500 text-sm font-medium">{stat.label}</p>
                                        <h3 className={`${stat.value.length > 4 ? 'text-xl mt-2' : 'text-3xl mt-1'} font-bold text-slate-800`}>{stat.value}</h3>
                                    </div>
                                    <div className={`bg-${stat.color}-50 text-${stat.color}-600 p-2 rounded-lg`}>
                                        <span className="material-symbols-outlined">{stat.icon}</span>
                                    </div>
                                </div>
                                {stat.trend && (
                                    <div className={`flex items-center gap-1 ${stat.trendColor} text-sm font-medium z-10`}>
                                        <span className="material-symbols-outlined text-[16px]">{index < 2 ? 'trending_up' : 'priority_high'}</span>
                                        <span>{stat.trend}</span>
                                    </div>
                                )}
                                {stat.progress && (
                                    <>
                                        <div className="w-full bg-slate-100 rounded-full h-1.5 mt-2 overflow-hidden">
                                            <div className="bg-indigo-500 h-1.5 rounded-full" style={{ width: `${stat.progress}%` }} />
                                        </div>
                                        <p className="text-slate-400 text-xs mt-1">{stat.progressLabel}</p>
                                    </>
                                )}
                                {stat.note && <p className="text-slate-500 text-xs">{stat.note}</p>}
                                <div className="absolute -right-4 -bottom-4 bg-gradient-to-br from-blue-50 to-transparent size-24 rounded-full opacity-50 group-hover:scale-110 transition-transform" />
                            </motion.div>
                        ))}
                    </div>

                    {/* Main Content Grid */}
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                        {/* Projects Table */}
                        <motion.div variants={item} className="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden flex flex-col">
                            <div className="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-white">
                                <h3 className="text-slate-800 font-bold text-lg">Trạng thái đề tài vừa gửi</h3>
                                <a href="#" className="text-primary text-sm font-medium hover:underline flex items-center gap-1">
                                    Xem tất cả <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
                                </a>
                            </div>
                            <div className="overflow-x-auto">
                                <table className="w-full text-left border-collapse">
                                    <thead className="bg-slate-50/50 text-slate-500 text-xs uppercase font-semibold">
                                        <tr>
                                            <th className="px-6 py-4">Tên đề tài</th>
                                            <th className="px-6 py-4">Sinh viên đại diện</th>
                                            <th className="px-6 py-4 text-center">Trạng thái</th>
                                            <th className="px-6 py-4 text-right">Hành động</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-slate-100">
                                        {recentProjects.map((project) => (
                                            <tr key={project.code} className="hover:bg-slate-50/50 transition-colors group">
                                                <td className="px-6 py-4">
                                                    <div className="flex items-center gap-3">
                                                        <div className={`bg-${project.iconColor}-100 text-${project.iconColor}-600 rounded p-1.5 flex-shrink-0`}>
                                                            <span className="material-symbols-outlined text-[20px]">{project.icon}</span>
                                                        </div>
                                                        <div>
                                                            <p className="font-semibold text-slate-900 text-sm">{project.name}</p>
                                                            <p className="text-xs text-slate-500">Mã: {project.code}</p>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td className="px-6 py-4">
                                                    <div className="flex items-center gap-2">
                                                        <div className="bg-slate-200 rounded-full size-6 flex items-center justify-center text-slate-600 text-xs font-bold">
                                                            {project.student.charAt(0)}
                                                        </div>
                                                        <span className="text-sm text-slate-700">{project.student}</span>
                                                    </div>
                                                </td>
                                                <td className="px-6 py-4 text-center">
                                                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${project.status === 'approved'
                                                        ? 'bg-emerald-50 text-emerald-700 border border-emerald-100'
                                                        : project.status === 'revision'
                                                            ? 'bg-rose-50 text-rose-700 border border-rose-100'
                                                            : 'bg-amber-50 text-amber-700 border border-amber-100'
                                                        }`}>
                                                        {project.status === 'approved' ? 'Đã duyệt' : project.status === 'revision' ? 'Yêu cầu sửa' : 'Chờ thẩm định'}
                                                    </span>
                                                </td>
                                                <td className="px-6 py-4 text-right">
                                                    <button className="text-slate-400 hover:text-primary transition-colors">
                                                        <span className="material-symbols-outlined">more_vert</span>
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </motion.div>

                        {/* Feedback Feed */}
                        <motion.div variants={item} className="lg:col-span-1 bg-white rounded-xl border border-slate-200 shadow-sm flex flex-col h-full">
                            <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
                                <h3 className="text-slate-800 font-bold text-lg">Phản hồi mới</h3>
                                <span className="material-symbols-outlined text-slate-400">forum</span>
                            </div>
                            <div className="p-4 space-y-4 overflow-y-auto flex-1 max-h-[400px]">
                                {feedbacks.map((fb, idx) => (
                                    <div key={idx} className="flex gap-3 relative pb-4 border-b border-dashed border-slate-100 last:border-0 last:pb-0">
                                        <div className="mt-1">
                                            <div className={`size-8 rounded-full bg-${fb.color}-100 flex items-center justify-center text-${fb.color}-600`}>
                                                <span className="material-symbols-outlined text-[16px]">{fb.icon}</span>
                                            </div>
                                        </div>
                                        <div className="flex-1">
                                            <div className="flex justify-between items-start">
                                                <p className="text-sm font-bold text-slate-800">{fb.author}</p>
                                                <span className="text-[10px] text-slate-400 whitespace-nowrap">{fb.time}</span>
                                            </div>
                                            <p className="text-xs text-slate-600 mt-1 leading-relaxed">{fb.content}</p>
                                            {idx === 0 && (
                                                <button className="mt-2 text-xs font-medium text-primary hover:underline">Xem chi tiết</button>
                                            )}
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <div className="p-3 border-t border-slate-100 bg-slate-50/50 rounded-b-xl text-center">
                                <button className="text-xs font-semibold text-slate-500 hover:text-primary transition-colors uppercase tracking-wide">
                                    Xem tất cả thông báo
                                </button>
                            </div>
                        </motion.div>
                    </div>
                </motion.div>
            </div>

            {/* Modal */}
            <RegisterTopicModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </>
    )
}
