import { motion } from 'framer-motion'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const historyItems = [
    {
        project: 'AI Traffic Control System',
        code: '#CS-2023-084',
        student: 'Nguyễn Văn A',
        date: '24/10/2023',
        type: 'Initial Review',
        result: 'Approved',
        resultColor: 'bg-green-50 text-green-600 border-green-200',
        feedback: 'Excellent work on the AI model. The system architecture is well-designed and the implementation is thorough.',
    },
    {
        project: 'E-commerce Optimization Framework',
        code: '#SE-2023-102',
        student: 'Trần Thị B',
        date: '23/10/2023',
        type: 'Revision Review',
        result: 'Needs Revision',
        resultColor: 'bg-amber-50 text-amber-600 border-amber-200',
        feedback: 'The methodology section needs more detail. Please expand on the data collection process and include more specific metrics.',
    },
    {
        project: 'Blockchain Agri-Supply Chain',
        code: '#IS-2023-055',
        student: 'Lê Văn C',
        date: '22/10/2023',
        type: 'Initial Review',
        result: 'Approved',
        resultColor: 'bg-green-50 text-green-600 border-green-200',
        feedback: 'Great implementation of blockchain technology. The supply chain tracking system is innovative and practical.',
    },
    {
        project: 'Urban Planning AI Model',
        code: '#AI-2023-112',
        student: 'Phạm T',
        date: '20/10/2023',
        type: 'Initial Review',
        result: 'Rejected',
        resultColor: 'bg-red-50 text-red-600 border-red-200',
        feedback: 'The project scope is too broad for a capstone project. The AI model lacks proper validation and testing.',
    },
    {
        project: 'Network Attack Detection',
        code: '#SEC-2023-019',
        student: 'Hoàng Long',
        date: '18/10/2023',
        type: 'Final Review',
        result: 'Approved',
        resultColor: 'bg-green-50 text-green-600 border-green-200',
        feedback: 'Excellent deep learning implementation. The ROC curve and confusion matrix show promising results.',
    },
]

export function EvaluatorHistoryPage() {
    return (
        <>
            {/* Header */}
            <header className="bg-white border-b border-gray-200 px-8 py-6 shrink-0">
                <div className="max-w-7xl mx-auto w-full flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex flex-col gap-1">
                        <h2 className="text-slate-900 text-2xl font-bold tracking-tight flex items-center gap-2">
                            <span className="material-symbols-outlined text-primary">history</span>
                            Lịch sử thẩm định
                        </h2>
                        <p className="text-slate-500 text-sm">Xem lại các đề tài đã thẩm định và phản hồi của bạn.</p>
                    </div>
                    <div className="flex gap-3">
                        <button className="flex items-center justify-center gap-2 h-10 px-4 rounded-lg border border-gray-200 bg-white text-slate-700 text-sm font-semibold hover:bg-gray-50 transition-colors">
                            <span className="material-symbols-outlined text-[20px]">download</span>
                            <span>Xuất Excel</span>
                        </button>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <div className="max-w-7xl mx-auto w-full p-6 md:p-8 flex flex-col gap-6 flex-1">
                <motion.div variants={container} initial="hidden" animate="show" className="flex flex-col gap-6">
                    {/* Stats */}
                    <motion.div variants={item} className="grid grid-cols-2 md:grid-cols-4 gap-4">
                        <div className="bg-white rounded-xl border border-gray-200 p-5 flex items-center gap-4">
                            <div className="size-12 rounded-xl bg-primary/10 text-primary flex items-center justify-center">
                                <span className="material-symbols-outlined text-2xl">assignment_turned_in</span>
                            </div>
                            <div>
                                <p className="text-2xl font-bold text-slate-900">56</p>
                                <p className="text-xs text-slate-500 font-medium">Tổng đã thẩm định</p>
                            </div>
                        </div>
                        <div className="bg-white rounded-xl border border-gray-200 p-5 flex items-center gap-4">
                            <div className="size-12 rounded-xl bg-green-50 text-green-600 flex items-center justify-center">
                                <span className="material-symbols-outlined text-2xl">check_circle</span>
                            </div>
                            <div>
                                <p className="text-2xl font-bold text-slate-900">42</p>
                                <p className="text-xs text-slate-500 font-medium">Đã duyệt</p>
                            </div>
                        </div>
                        <div className="bg-white rounded-xl border border-gray-200 p-5 flex items-center gap-4">
                            <div className="size-12 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
                                <span className="material-symbols-outlined text-2xl">edit_note</span>
                            </div>
                            <div>
                                <p className="text-2xl font-bold text-slate-900">10</p>
                                <p className="text-xs text-slate-500 font-medium">Cần chỉnh sửa</p>
                            </div>
                        </div>
                        <div className="bg-white rounded-xl border border-gray-200 p-5 flex items-center gap-4">
                            <div className="size-12 rounded-xl bg-red-50 text-red-600 flex items-center justify-center">
                                <span className="material-symbols-outlined text-2xl">cancel</span>
                            </div>
                            <div>
                                <p className="text-2xl font-bold text-slate-900">4</p>
                                <p className="text-xs text-slate-500 font-medium">Từ chối</p>
                            </div>
                        </div>
                    </motion.div>

                    {/* Filters */}
                    <motion.div variants={item} className="bg-white rounded-xl border border-gray-200 p-5 shadow-sm">
                        <div className="grid grid-cols-1 md:grid-cols-12 gap-4 items-end">
                            <div className="md:col-span-3 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Tìm kiếm</label>
                                <div className="relative group">
                                    <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-primary transition-colors">search</span>
                                    <input
                                        className="w-full pl-10 pr-4 py-2.5 rounded-lg border border-gray-200 bg-gray-50 text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all outline-none"
                                        placeholder="Tên đề tài, sinh viên..."
                                        type="text"
                                    />
                                </div>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Thời gian</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option selected>Tháng này</option>
                                    <option>Tuần này</option>
                                    <option>Hôm qua</option>
                                </select>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Kết quả</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option>Đã duyệt</option>
                                    <option>Cần chỉnh sửa</option>
                                    <option>Từ chối</option>
                                </select>
                            </div>
                            <div className="md:col-span-2 flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">Loại</label>
                                <select className="w-full px-3 py-2.5 rounded-lg border border-gray-200 bg-white text-sm font-medium focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none cursor-pointer">
                                    <option>Tất cả</option>
                                    <option>Initial Review</option>
                                    <option>Revision Review</option>
                                    <option>Final Review</option>
                                </select>
                            </div>
                            <div className="md:col-span-3 flex justify-end gap-2">
                                <button className="flex-1 md:flex-none h-[42px] px-4 rounded-lg border border-gray-200 text-slate-500 font-semibold text-sm hover:bg-gray-50 transition-colors flex items-center justify-center gap-2">
                                    <span className="material-symbols-outlined text-[18px]">filter_alt_off</span>
                                    Xóa lọc
                                </button>
                            </div>
                        </div>
                    </motion.div>

                    {/* History Table */}
                    <motion.div variants={item} className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden flex flex-col flex-1">
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="bg-gray-50/80 border-b border-gray-100">
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider w-1/4">Đề tài</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Sinh viên</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider whitespace-nowrap">Ngày thẩm định</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Loại</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-center">Kết quả</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Phản hồi</th>
                                        <th className="px-6 py-4 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-right">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-gray-100">
                                    {historyItems.map((item, idx) => (
                                        <motion.tr
                                            key={idx}
                                            whileHover={{ backgroundColor: 'rgb(249 250 251)' }}
                                            className="group transition-colors"
                                        >
                                            <td className="px-6 py-4">
                                                <div className="flex flex-col">
                                                    <span className="text-slate-900 font-semibold text-sm line-clamp-1">{item.project}</span>
                                                    <span className="text-xs text-slate-500 font-mono mt-1">{item.code}</span>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="text-slate-900 font-medium text-sm">{item.student}</span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="text-slate-500 text-sm font-medium">{item.date}</span>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className="inline-flex items-center px-2.5 py-1 rounded-md text-xs font-medium bg-gray-100 text-gray-700 border border-gray-200">
                                                    {item.type}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-center whitespace-nowrap">
                                                <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-bold border ${item.resultColor}`}>
                                                    {item.result}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4">
                                                <p className="text-sm text-slate-500 line-clamp-2 max-w-xs">{item.feedback}</p>
                                            </td>
                                            <td className="px-6 py-4 text-right whitespace-nowrap">
                                                <button className="inline-flex items-center justify-center h-8 px-4 bg-white border border-gray-200 text-slate-700 text-xs font-bold rounded-lg hover:bg-gray-50 hover:border-primary/50 hover:text-primary transition-all">
                                                    Chi tiết
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
                                Hiển thị <span className="font-bold text-slate-900">1</span> đến <span className="font-bold text-slate-900">5</span> trong tổng số <span className="font-bold text-slate-900">56</span> bản ghi
                            </p>
                            <div className="flex items-center gap-2">
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 disabled:opacity-50 transition-all">
                                    <span className="material-symbols-outlined text-sm">chevron_left</span>
                                </button>
                                <button className="size-8 flex items-center justify-center rounded-lg bg-primary text-white text-xs font-bold shadow-md shadow-primary/20">1</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 text-xs font-bold transition-all">2</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 text-xs font-bold transition-all">...</button>
                                <button className="size-8 flex items-center justify-center rounded-lg border border-gray-200 hover:bg-gray-50 text-slate-500 text-xs font-bold transition-all">12</button>
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
