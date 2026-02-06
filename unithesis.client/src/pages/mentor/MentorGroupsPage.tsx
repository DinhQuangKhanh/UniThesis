import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'
import { NotificationDropdown } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.08 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const groups = [
    { id: 'G1', name: 'Nhóm 01 - K15', field: 'Công nghệ Web', icon: 'code', topic: 'Xây dựng nền tảng học trực tuyến E-Learning tích hợp AI', members: 3 },
    { id: 'G2', name: 'Nhóm 05 - K15', field: 'Ứng dụng Mobile', icon: 'smartphone', topic: 'Ứng dụng đặt sân bóng đá và quản lý giải đấu mini', members: 2 },
    { id: 'G3', name: 'Nhóm 12 - K15', field: 'IoT / Robotics', icon: 'smart_toy', topic: 'Hệ thống nhà thông minh điều khiển bằng giọng nói tiếng Việt', members: 4 },
    { id: 'G4', name: 'Nhóm 18 - K15', field: 'Big Data', icon: 'database', topic: 'Phân tích hành vi người dùng trên mạng xã hội', members: 2 },
]

export function MentorGroupsPage() {
    const navigate = useNavigate()

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-slate-800 border-b border-slate-700 flex-shrink-0 z-50 shadow-md">
                <div className="flex items-center gap-4">
                    <div className="flex items-center gap-2 text-white">
                        <span className="text-slate-400 font-medium text-sm">Quản lý</span>
                        <span className="material-symbols-outlined text-sm text-slate-500">chevron_right</span>
                        <h2 className="text-lg font-bold">Nhóm của tôi</h2>
                    </div>
                </div>
                <div className="flex items-center gap-4">
                    <div className="relative hidden md:block">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-slate-400 text-[20px]">search</span>
                        </div>
                        <input
                            type="text"
                            className="block w-64 pl-10 pr-3 py-2 border-none rounded-lg bg-slate-700 text-white placeholder-slate-400 focus:outline-none focus:bg-slate-600 focus:ring-1 focus:ring-primary/50 sm:text-sm transition-all"
                            placeholder="Tìm kiếm nhóm, đề tài..."
                        />
                    </div>
                    <NotificationDropdown role="mentor" isNavy={true} />
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-100">
                <motion.div variants={container} initial="hidden" animate="show" className="max-w-[1200px] mx-auto space-y-8">
                    {/* Title */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h1 className="text-2xl font-bold text-slate-900">Danh sách nhóm hướng dẫn</h1>
                            <p className="text-slate-500 text-sm mt-1">Quản lý tiến độ và theo dõi các nhóm sinh viên khóa K15</p>
                        </div>
                        <div className="flex items-center gap-3">
                            <select className="appearance-none bg-white border border-slate-200 text-slate-700 py-2 pl-4 pr-10 rounded-lg focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary text-sm font-medium shadow-sm">
                                <option>Tất cả trạng thái</option>
                                <option>Đúng tiến độ</option>
                                <option>Chậm tiến độ</option>
                                <option>Cần xem xét</option>
                            </select>
                        </div>
                    </motion.div>

                    {/* Groups Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                        {groups.map((group) => (
                            <motion.div
                                key={group.id}
                                variants={item}
                                className="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 flex flex-col overflow-hidden"
                            >
                                <div className="p-5 flex-1">
                                    <div className="flex justify-between items-start mb-3">
                                        <div>
                                            <h3 className="text-lg font-bold text-slate-900">{group.name}</h3>
                                            <span className="inline-flex items-center gap-1 text-xs text-slate-500 mt-1">
                                                <span className="material-symbols-outlined text-[14px]">{group.icon}</span>
                                                {group.field}
                                            </span>
                                        </div>
                                        <button className="text-slate-400 hover:text-slate-600">
                                            <span className="material-symbols-outlined">more_horiz</span>
                                        </button>
                                    </div>
                                    <h4 className="font-medium text-slate-800 text-sm mb-4 line-clamp-2 h-10">{group.topic}</h4>
                                    <div className="flex items-center justify-between">
                                        <div className="flex -space-x-2 overflow-hidden">
                                            {Array.from({ length: Math.min(group.members, 3) }).map((_, i) => (
                                                <div
                                                    key={i}
                                                    className="inline-block size-8 rounded-full ring-2 ring-white bg-slate-200 flex items-center justify-center text-xs font-bold text-slate-500"
                                                >
                                                    {i + 1}
                                                </div>
                                            ))}
                                            {group.members > 3 && (
                                                <div className="inline-block size-8 rounded-full ring-2 ring-white bg-slate-100 flex items-center justify-center text-xs font-bold text-slate-500">
                                                    +{group.members - 3}
                                                </div>
                                            )}
                                        </div>
                                        <span className="text-xs font-medium text-slate-500">{group.members} thành viên</span>
                                    </div>
                                </div>
                                <div className="px-5 py-3 bg-slate-50 border-t border-slate-100 flex items-center justify-end">
                                    <button
                                        onClick={() => navigate('/mentor/groups/1')}
                                        className="text-primary hover:text-primary/80 text-sm font-semibold flex items-center gap-1"
                                    >
                                        Chi tiết <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
                                    </button>
                                </div>
                            </motion.div>
                        ))}
                    </div>

                    {/* Pagination */}
                    <motion.div variants={item} className="flex items-center justify-between border-t border-slate-200 pt-4">
                        <p className="text-sm text-slate-500">
                            Hiển thị <span className="font-medium text-slate-900">1-4</span> trên <span className="font-medium text-slate-900">4</span> nhóm
                        </p>
                        <div className="flex gap-2">
                            <button className="px-3 py-1 border border-slate-300 rounded-md text-sm font-medium text-slate-500 hover:bg-slate-50 disabled:opacity-50" disabled>
                                Trước
                            </button>
                            <button className="px-3 py-1 bg-primary text-white border border-primary rounded-md text-sm font-medium hover:bg-primary/90">1</button>
                            <button className="px-3 py-1 border border-slate-300 rounded-md text-sm font-medium text-slate-700 hover:bg-slate-50" disabled>
                                Sau
                            </button>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}
