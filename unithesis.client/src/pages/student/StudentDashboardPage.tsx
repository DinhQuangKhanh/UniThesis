import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.08 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const quickAccess = [
    { label: 'Kho đề tài', icon: 'folder_shared', color: 'blue', path: '/student/topics' },
    { label: 'Biểu mẫu', icon: 'assignment', color: 'orange', path: '#' },
    { label: 'Nộp bài', icon: 'cloud_upload', color: 'purple', path: '#' },
    { label: 'Hỗ trợ', icon: 'live_help', color: 'pink', path: '#' },
]

const deadlines = [
    { day: '15', month: 'T10', type: 'Quan trọng', typeColor: 'red', title: 'Nộp bản thảo Chương 3 - Cơ sở lý thuyết', time: '10:00 AM', location: 'Hệ thống LMS' },
    { day: '20', month: 'T10', type: 'Họp nhóm', typeColor: 'blue', title: 'Báo cáo tiến độ với Giảng viên hướng dẫn', time: '08:30 AM', location: 'Phòng 302' },
    { day: '05', month: 'T11', type: 'Nộp bài', typeColor: 'gray', title: 'Nộp báo cáo giữa kỳ (Soft copy)', time: '23:59 PM', location: 'Email GV' },
]

export function StudentDashboardPage() {
    const navigate = useNavigate()

    return (
        <>
            {/* Header */}
            <header className="bg-white border-b border-[#e9ecf1] h-16 flex items-center justify-between px-8 shrink-0 z-10 sticky top-0">
                <div className="flex items-center gap-4 flex-1 max-w-xl">
                    <div className="relative w-full group">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-[#58698d] group-focus-within:text-primary transition-colors">search</span>
                        </div>
                        <input
                            className="block w-full pl-10 pr-3 py-2 border-none rounded-lg leading-5 bg-[#f6f7f8] text-gray-900 placeholder-[#58698d] focus:outline-none focus:bg-white focus:ring-1 focus:ring-primary transition-all sm:text-sm h-10"
                            placeholder="Tìm kiếm đề tài, giảng viên, tài liệu..."
                            type="text"
                        />
                    </div>
                </div>
                <div className="flex items-center gap-6">
                    <button className="relative text-[#58698d] hover:text-primary transition-colors p-1">
                        <span className="material-symbols-outlined">notifications</span>
                        <span className="absolute top-1 right-1 h-2 w-2 rounded-full bg-red-500 border border-white" />
                    </button>
                    <div className="h-8 w-[1px] bg-[#e9ecf1]" />
                    <button className="text-[#58698d] hover:text-primary transition-colors text-sm font-medium flex items-center gap-1">
                        Trợ giúp
                        <span className="material-symbols-outlined text-lg">help</span>
                    </button>
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8">
                <motion.div variants={container} initial="hidden" animate="show" className="max-w-[1200px] mx-auto flex flex-col gap-6">
                    {/* Welcome Section */}
                    <motion.section variants={item} className="bg-white rounded-xl p-6 border border-[#e9ecf1] shadow-sm flex flex-col md:flex-row items-center justify-between gap-6">
                        <div className="flex flex-col gap-2">
                            <h2 className="text-2xl font-bold text-[#101319]">Chào buổi sáng, An! 👋</h2>
                            <p className="text-[#58698d] text-sm">
                                Bạn có <span className="text-primary font-bold underline cursor-pointer hover:text-primary-light">2 thông báo mới</span> từ giảng viên cần phản hồi trước 20/10.
                            </p>
                        </div>
                        <div className="flex flex-wrap gap-3">
                            <button className="px-4 py-2 bg-white border border-[#e9ecf1] text-[#58698d] hover:text-primary hover:border-primary rounded-lg text-sm font-bold transition-all flex items-center gap-2">
                                <span className="material-symbols-outlined text-[20px]">add_task</span>
                                Tạo báo cáo tuần
                            </button>
                            <button className="px-4 py-2 bg-primary text-white hover:bg-primary-light rounded-lg text-sm font-bold shadow-sm transition-colors flex items-center gap-2">
                                Kho tài liệu
                                <span className="material-symbols-outlined text-[20px]">folder</span>
                            </button>
                        </div>
                    </motion.section>

                    {/* Main Grid */}
                    <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
                        {/* Topic Overview */}
                        <motion.div variants={item} className="lg:col-span-8 bg-white rounded-xl border border-[#e9ecf1] shadow-sm p-6 flex flex-col">
                            <div className="flex items-center justify-between mb-6">
                                <h3 className="font-bold text-lg text-[#101319] flex items-center gap-2">
                                    <span className="material-symbols-outlined text-primary">donut_large</span>
                                    Tổng quan đề tài
                                </h3>
                                <button
                                    onClick={() => navigate('/student/my-topic')}
                                    className="text-sm text-[#58698d] hover:text-primary font-medium flex items-center gap-1 transition-colors"
                                >
                                    Chi tiết
                                    <span className="material-symbols-outlined text-[18px]">arrow_right_alt</span>
                                </button>
                            </div>
                            <div className="flex flex-col gap-5">
                                <div>
                                    <h4 className="text-xl font-bold text-[#101319] leading-tight mb-2">
                                        Xây dựng hệ thống quản lý thư viện số
                                    </h4>
                                    <p className="text-sm text-[#58698d] mb-1">
                                        GVHD: <span className="font-semibold text-gray-700">TS. Trần Minh Tuấn</span>
                                    </p>
                                </div>
                                <div className="bg-gray-50 p-5 rounded-lg border border-gray-100">
                                    <p className="text-sm text-gray-600 leading-relaxed">
                                        Nghiên cứu và phát triển giải pháp quản lý tài liệu số hóa, tích hợp công nghệ nhận dạng ký tự quang học (OCR) và module gợi ý sách dựa trên lịch sử đọc.
                                    </p>
                                </div>
                                <div className="flex flex-wrap items-center gap-3 mt-auto">
                                    <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-bold bg-green-50 text-green-700 border border-green-100">
                                        <span className="w-1.5 h-1.5 rounded-full bg-green-500" />
                                        Đang thực hiện
                                    </span>
                                    <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium text-gray-600 border border-gray-200">
                                        <span className="material-symbols-outlined text-[14px]">schedule</span>
                                        Còn 38 ngày
                                    </span>
                                </div>
                            </div>
                        </motion.div>

                        {/* Quick Access */}
                        <motion.div variants={item} className="lg:col-span-4 bg-white rounded-xl border border-[#e9ecf1] shadow-sm p-6 flex flex-col">
                            <h3 className="font-bold text-lg text-[#101319] mb-4 flex items-center gap-2">
                                <span className="material-symbols-outlined text-primary">bolt</span>
                                Truy cập nhanh
                            </h3>
                            <div className="grid grid-cols-2 gap-4 h-full">
                                {quickAccess.map((qa) => (
                                    <button
                                        key={qa.label}
                                        onClick={() => qa.path !== '#' && navigate(qa.path)}
                                        className={`group flex flex-col items-center justify-center p-4 rounded-xl border border-[#e9ecf1] bg-white hover:border-${qa.color}-200 hover:shadow-md hover:-translate-y-1 transition-all`}
                                    >
                                        <div className={`w-12 h-12 mb-3 rounded-full bg-${qa.color}-50 text-${qa.color}-600 flex items-center justify-center group-hover:scale-110 transition-transform`}>
                                            <span className="material-symbols-outlined text-[24px]">{qa.icon}</span>
                                        </div>
                                        <span className={`text-sm font-semibold text-gray-700 group-hover:text-${qa.color}-700`}>{qa.label}</span>
                                    </button>
                                ))}
                            </div>
                        </motion.div>
                    </div>

                    {/* Deadlines Section */}
                    <motion.section variants={item} className="bg-white rounded-xl border border-[#e9ecf1] shadow-sm overflow-hidden">
                        <div className="p-5 border-b border-[#e9ecf1] flex items-center justify-between bg-gray-50/50">
                            <div className="flex items-center gap-3">
                                <div className="bg-secondary/10 p-1.5 rounded-lg text-secondary">
                                    <span className="material-symbols-outlined text-[20px]">calendar_clock</span>
                                </div>
                                <h3 className="font-bold text-[#101319]">Sắp tới (Deadlines)</h3>
                            </div>
                            <div className="flex gap-2">
                                <button className="p-1 hover:bg-gray-200 rounded transition-colors text-[#58698d]">
                                    <span className="material-symbols-outlined text-[20px]">filter_list</span>
                                </button>
                                <button className="p-1 hover:bg-gray-200 rounded transition-colors text-[#58698d]">
                                    <span className="material-symbols-outlined text-[20px]">more_horiz</span>
                                </button>
                            </div>
                        </div>
                        <div className="flex flex-col">
                            {deadlines.map((dl, idx) => (
                                <div key={idx} className="flex items-center gap-4 p-4 border-b border-[#e9ecf1] last:border-0 hover:bg-gray-50 transition-colors group cursor-pointer">
                                    <div className={`w-14 h-14 rounded-xl bg-${dl.typeColor}-50 text-${dl.typeColor}-600 flex flex-col items-center justify-center border border-${dl.typeColor}-100 shrink-0`}>
                                        <span className="text-[10px] font-bold uppercase tracking-wider">{dl.month}</span>
                                        <span className="text-xl font-bold leading-none">{dl.day}</span>
                                    </div>
                                    <div className="flex-1 min-w-0">
                                        <div className="flex items-center gap-2 mb-1">
                                            <span className={`text-xs font-bold text-${dl.typeColor}-600 bg-${dl.typeColor}-50 px-2 py-0.5 rounded border border-${dl.typeColor}-100`}>
                                                {dl.type}
                                            </span>
                                            <span className="text-xs text-[#58698d] flex items-center gap-1">
                                                <span className="material-symbols-outlined text-[14px]">schedule</span>
                                                {dl.time}
                                            </span>
                                        </div>
                                        <h4 className="font-bold text-[#101319] text-sm truncate group-hover:text-primary transition-colors">
                                            {dl.title}
                                        </h4>
                                    </div>
                                    <div className="hidden sm:flex flex-col items-end gap-1 text-right shrink-0">
                                        <span className="text-xs text-[#58698d] font-medium flex items-center gap-1">
                                            <span className="material-symbols-outlined text-[16px]">location_on</span>
                                            {dl.location}
                                        </span>
                                        <button className="text-xs font-bold text-primary opacity-0 group-hover:opacity-100 transition-opacity">
                                            Chi tiết
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </motion.section>

                    {/* Footer */}
                    <div className="mt-12 pt-6 border-t border-[#e9ecf1] flex flex-col md:flex-row justify-between items-center text-[#58698d] text-sm pb-8">
                        <p>© 2023 University Thesis Management System.</p>
                        <div className="flex gap-4 mt-2 md:mt-0">
                            <a className="hover:text-primary" href="#">Quy định bảo mật</a>
                            <a className="hover:text-primary" href="#">Điều khoản sử dụng</a>
                        </div>
                    </div>
                </motion.div>
            </div>
        </>
    )
}
