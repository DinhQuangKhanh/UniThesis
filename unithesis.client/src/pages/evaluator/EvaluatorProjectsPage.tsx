import { motion } from 'framer-motion'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const projects = [
    {
        id: '#CS-2023-084',
        title: 'Hệ thống điều khiển giao thông thông minh AI',
        major: 'KHMT',
        student: 'Nguyễn Văn A',
        studentCode: 'K15 - HCM',
        mentor: 'Dr. Emily Tran',
        submitted: '24/10/2023',
        status: 'Chờ duyệt',
        statusColor: 'bg-blue-50 text-blue-600 border-blue-100',
        animate: true,
    },
    {
        id: '#SE-2023-102',
        title: 'Nền tảng tối ưu hóa E-commerce Microservices',
        major: 'KTPM',
        student: 'Trần Thị B',
        studentCode: 'K15 - HN',
        mentor: 'ThS. Le Hoang',
        submitted: '23/10/2023',
        status: 'Cần chỉnh sửa',
        statusColor: 'bg-amber-50 text-amber-600 border-amber-100',
        animate: false,
    },
    {
        id: '#IS-2023-055',
        title: 'Ứng dụng Blockchain trong chuỗi cung ứng nông sản',
        major: 'HTTT',
        student: 'Lê Văn C',
        studentCode: 'K14 - DN',
        mentor: 'TS. Nguyen Minh',
        submitted: '22/10/2023',
        status: 'Đã duyệt',
        statusColor: 'bg-green-50 text-green-600 border-green-100',
        animate: false,
    },
    {
        id: '#AI-2023-112',
        title: 'Xây dựng Model AI dự đoán quy hoạch đô thị',
        major: 'KHMT',
        student: 'Phạm T',
        studentCode: 'K15 - HCM',
        mentor: 'Prof. John Doe',
        submitted: '20/10/2023',
        status: 'Bị hủy',
        statusColor: 'bg-gray-100 text-gray-500 border-gray-200',
        animate: false,
        disabled: true,
    },
    {
        id: '#SEC-2023-019',
        title: 'Phát hiện tấn công mạng bằng Deep Learning',
        major: 'ATTT',
        student: 'Hoàng Long',
        studentCode: 'K15 - HN',
        mentor: 'TS. Pham Ha',
        submitted: '25/10/2023',
        status: 'Chờ duyệt',
        statusColor: 'bg-blue-50 text-blue-600 border-blue-100',
        animate: true,
        priority: true,
    },
]

export function EvaluatorProjectsPage() {
    return (
        <>
            {/* Header */}
            <header className="bg-primary px-8 py-6 shrink-0 shadow-lg z-10">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 w-full">
                    <div className="flex flex-col gap-1">
                        <h2 className="text-white text-2xl font-bold tracking-tight flex items-center gap-2">
                            <span className="material-symbols-outlined">folder_shared</span>
                            Danh sách đề tài
                        </h2>
                        <p className="text-blue-100/80 text-sm">Quản lý và thẩm định tất cả các đề tài đồ án được phân công.</p>
                    </div>
                    <div className="flex gap-3">
                        <div className="hidden md:flex items-center bg-primary-dark/50 rounded-lg px-4 py-2 border border-blue-400/30">
                            <span className="text-blue-100 text-xs font-semibold uppercase tracking-wider mr-2">Học kỳ:</span>
                            <span className="text-white text-sm font-bold">Fall 2023</span>
                        </div>
                        <button className="flex items-center justify-center gap-2 h-10 px-5 rounded-lg bg-white text-primary text-sm font-bold hover:bg-blue-50 transition-colors shadow-md">
                            <span className="material-symbols-outlined text-[20px]">add_task</span>
                            <span>Thẩm định nhanh</span>
                        </button>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <div className="w-full p-6 md:p-8 flex flex-col gap-6 flex-1">
                <motion.div variants={container} initial="hidden" animate="show" className="flex flex-col gap-6">
                    {/* Filters */}
                    <motion.div variants={item} className="bg-white rounded-xl border border-gray-200 p-5 shadow-sm">
                        <div className="grid grid-cols-1 md:grid-cols-12 gap-4 items-end">
                            <div className="md:col-span-3 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Tìm kiếm</label>
                                <div className="relative group">
                                    <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-primary transition-colors">search</span>
                                    <input
                                        className="w-full pl-10 pr-4 py-2.5 rounded-lg border border-gray-200 bg-gray-50 text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none"
                                        placeholder="Mã đề tài, Tên đề tài..."
                                        type="text"
                                    />
                                </div>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Kỳ học</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option selected>Fall 2023</option>
                                    <option>Summer 2023</option>
                                    <option>Spring 2023</option>
                                </select>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Chuyên ngành</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option>Khoa học máy tính</option>
                                    <option>Kỹ thuật phần mềm</option>
                                    <option>Hệ thống thông tin</option>
                                    <option>An toàn thông tin</option>
                                </select>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Mức độ ưu tiên</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option>Cao</option>
                                    <option>Trung bình</option>
                                    <option>Thấp</option>
                                </select>
                            </div>
                            <div className="md:col-span-3 flex justify-end gap-2">
                                <button className="flex-1 md:flex-none h-[42px] px-4 rounded-lg border border-gray-200 text-slate-500 font-semibold text-sm hover:bg-gray-50 transition-colors flex items-center justify-center gap-2">
                                    <span className="material-symbols-outlined text-[18px]">filter_alt_off</span>
                                    Xóa lọc
                                </button>
                                <button className="flex-1 md:flex-none h-[42px] px-6 rounded-lg bg-primary text-white font-semibold text-sm hover:bg-primary-dark transition-colors flex items-center justify-center gap-2 shadow-lg shadow-primary/20">
                                    <span className="material-symbols-outlined text-[18px]">filter_list</span>
                                    Áp dụng
                                </button>
                            </div>
                        </div>
                    </motion.div>

                    {/* Table */}
                    <motion.div variants={item} className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden flex flex-col flex-1">
                        <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
                            <div className="flex items-center gap-2">
                                <span className="flex items-center justify-center size-6 rounded bg-primary/10 text-primary text-xs font-bold">14</span>
                                <h3 className="text-slate-900 text-base font-bold">Danh sách đề tài cần thẩm định</h3>
                            </div>
                            <div className="flex gap-2">
                                <button className="p-2 rounded-lg hover:bg-gray-100 text-slate-500 transition-colors">
                                    <span className="material-symbols-outlined text-[20px]">download</span>
                                </button>
                                <button className="p-2 rounded-lg hover:bg-gray-100 text-slate-500 transition-colors">
                                    <span className="material-symbols-outlined text-[20px]">print</span>
                                </button>
                            </div>
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="bg-gray-50/80 border-b border-gray-100">
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider whitespace-nowrap">Mã đề tài</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider w-1/4">Tên đề tài</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Sinh viên</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Mentor</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider whitespace-nowrap">Ngày nộp</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-center">Trạng thái</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-right sticky right-0 bg-gray-50/80 shadow-[-10px_0_10px_-10px_rgba(0,0,0,0.05)]">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-gray-100">
                                    {projects.map((project, idx) => (
                                        <motion.tr
                                            key={idx}
                                            whileHover={{ backgroundColor: 'rgb(239 246 255 / 0.3)' }}
                                            className="group transition-colors"
                                        >
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="font-mono text-xs font-bold text-slate-500 bg-gray-100 px-2 py-1 rounded">{project.id}</span>
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex flex-col">
                                                    <span className="text-slate-900 font-bold text-sm line-clamp-2">{project.title}</span>
                                                    {project.priority ? (
                                                        <span className="text-xs text-red-500 font-bold mt-1 flex items-center gap-1">
                                                            <span className="material-symbols-outlined text-[14px]">priority_high</span>
                                                            Ưu tiên cao
                                                        </span>
                                                    ) : (
                                                        <span className="text-xs text-slate-500 mt-1">Chuyên ngành: {project.major}</span>
                                                    )}
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="flex items-center gap-3">
                                                    <div className="size-8 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-xs ring-1 ring-primary/10">
                                                        {project.student.split(' ').map(n => n[0]).join('').slice(-2)}
                                                    </div>
                                                    <div className="flex flex-col">
                                                        <span className="text-slate-900 font-medium text-sm">{project.student}</span>
                                                        <span className="text-[10px] text-slate-500">{project.studentCode}</span>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="text-sm text-slate-900 font-medium">{project.mentor}</span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="text-slate-500 text-sm font-medium">{project.submitted}</span>
                                            </td>
                                            <td className="px-6 py-4 text-center whitespace-nowrap">
                                                <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-bold border ${project.statusColor}`}>
                                                    {project.animate && <span className="size-1.5 rounded-full bg-blue-500 animate-pulse" />}
                                                    {project.status}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-right sticky right-0 bg-white group-hover:bg-blue-50/30 transition-colors shadow-[-10px_0_10px_-10px_rgba(0,0,0,0.05)]">
                                                <button
                                                    disabled={project.disabled}
                                                    className={`inline-flex items-center justify-center h-8 px-4 text-xs font-bold rounded-lg transition-all ${project.disabled
                                                        ? 'bg-gray-50 text-gray-400 cursor-not-allowed border border-gray-100'
                                                        : project.status === 'Chờ duyệt'
                                                            ? 'bg-primary text-white hover:bg-primary-dark shadow-sm shadow-primary/20 hover:shadow-md hover:-translate-y-0.5'
                                                            : 'bg-white border border-gray-200 text-slate-900 hover:bg-gray-50 hover:border-primary/50 hover:text-primary'
                                                        }`}
                                                >
                                                    {project.status === 'Chờ duyệt' ? 'Thẩm định' : project.disabled ? 'Thẩm định' : project.status === 'Đã duyệt' ? 'Xem lại' : 'Chi tiết'}
                                                </button>
                                            </td>
                                        </motion.tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                        {/* Pagination */}
                        <div className="px-6 py-4 border-t border-gray-100 flex items-center justify-between bg-white shrink-0">
                            <p className="text-xs text-slate-500 font-medium">
                                Hiển thị <span className="font-bold text-slate-900">1</span> đến <span className="font-bold text-slate-900">5</span> trong tổng số <span className="font-bold text-slate-900">14</span> đề tài
                            </p>
                            <div className="flex items-center gap-2">
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 disabled:opacity-50 transition-all">
                                    <span className="material-symbols-outlined text-sm">chevron_left</span>
                                </button>
                                <button className="size-8 flex items-center justify-center rounded-lg bg-primary text-white text-xs font-bold shadow-md shadow-primary/20">1</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 text-xs font-bold transition-all">2</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 text-xs font-bold transition-all">3</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 disabled:opacity-50 transition-all">
                                    <span className="material-symbols-outlined text-sm">chevron_right</span>
                                </button>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}
