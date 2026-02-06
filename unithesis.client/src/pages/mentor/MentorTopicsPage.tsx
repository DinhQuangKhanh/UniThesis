import { useState } from 'react'
import { motion } from 'framer-motion'
import { RegisterTopicModal } from '@/components/mentor/RegisterTopicModal'
import { NotificationDropdown } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const topics = [
    { code: 'PROJ-2024-001', name: 'Ứng dụng AI trong chẩn đoán hình ảnh Y Khoa', field: 'Khoa học máy tính', icon: 'medical_services', iconColor: 'blue', type: 'reserved', status: 'pending', date: '12/10/2023' },
    { code: 'PROJ-2024-045', name: 'Xây dựng hệ thống E-commerce Local cho nông sản', field: 'Kỹ thuật phần mềm', icon: 'shopping_cart', iconColor: 'emerald', type: 'public', status: 'approved', date: '10/10/2023' },
    { code: 'PROJ-2024-089', name: 'Phát triển Robot hỗ trợ người già tại nhà', field: 'IoT & Robotics', icon: 'smart_toy', iconColor: 'rose', type: 'public', status: 'revision', date: '05/10/2023' },
    { code: 'PROJ-2024-102', name: 'Hệ thống quản lý chuỗi cung ứng Blockchain', field: 'Hệ thống thông tin', icon: 'account_balance', iconColor: 'purple', type: 'public', status: 'approved', date: '01/10/2023' },
    { code: 'PROJ-2024-115', name: 'Giải pháp Cloud Native cho doanh nghiệp SME', field: 'Mạng máy tính', icon: 'cloud', iconColor: 'orange', type: 'reserved', status: 'pending', date: '28/09/2023' },
]

export function MentorTopicsPage() {
    const [isModalOpen, setIsModalOpen] = useState(false)

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'approved':
                return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-emerald-50 text-emerald-700 border border-emerald-200"><span className="size-1.5 rounded-full bg-emerald-500" />Đã duyệt</span>
            case 'revision':
                return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-rose-50 text-rose-700 border border-rose-200"><span className="size-1.5 rounded-full bg-rose-500" />Yêu cầu sửa</span>
            default:
                return <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-amber-50 text-amber-700 border border-amber-200"><span className="size-1.5 rounded-full bg-amber-500" />Chờ duyệt</span>
        }
    }

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-white border-b border-slate-200 flex-shrink-0 z-50 shadow-sm">
                <nav className="flex text-sm font-medium text-slate-500">
                    <a href="#" className="hover:text-slate-900">UniThesis</a>
                    <span className="mx-2 text-slate-400">/</span>
                    <span className="text-slate-900 font-bold">Danh sách đề tài</span>
                </nav>
                <NotificationDropdown role="mentor" />
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-100">
                <motion.div variants={container} initial="hidden" animate="show" className="max-w-[1400px] mx-auto space-y-6">
                    {/* Title & Actions */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h2 className="text-2xl font-bold text-slate-900">Kho Đề Tài Của Mentor</h2>
                            <p className="text-slate-500 mt-1 text-sm">Quản lý và theo dõi trạng thái thẩm định các đề tài hướng dẫn.</p>
                        </div>
                        <button
                            onClick={() => setIsModalOpen(true)}
                            className="inline-flex items-center justify-center gap-2 bg-primary hover:bg-primary/90 text-white px-4 py-2 rounded-lg font-semibold shadow-sm transition-all text-sm"
                        >
                            <span className="material-symbols-outlined text-[20px]">add</span>
                            <span>Đề xuất mới</span>
                        </button>
                    </motion.div>

                    {/* Search & Filter */}
                    <motion.div variants={item} className="bg-white p-4 rounded-xl border border-slate-200 shadow-sm flex flex-col md:flex-row gap-4 items-center justify-between">
                        <div className="relative w-full md:w-96">
                            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                <span className="material-symbols-outlined text-slate-400 text-[20px]">search</span>
                            </div>
                            <input
                                type="text"
                                className="block w-full pl-10 pr-3 py-2 border border-slate-200 rounded-lg bg-slate-50 text-slate-900 placeholder-slate-400 focus:outline-none focus:bg-white focus:ring-1 focus:ring-primary focus:border-primary sm:text-sm transition-all"
                                placeholder="Tìm kiếm theo mã hoặc tên đề tài..."
                            />
                        </div>
                        <div className="flex items-center gap-3 w-full md:w-auto">
                            <div className="flex items-center gap-2 min-w-[140px]">
                                <span className="text-sm text-slate-500 font-medium whitespace-nowrap">Kỳ học:</span>
                                <select className="form-select block w-full pl-3 pr-10 py-1.5 text-sm border-slate-200 focus:ring-primary focus:border-primary rounded-lg bg-slate-50">
                                    <option>Fall 2024</option>
                                    <option>Summer 2024</option>
                                    <option>Spring 2024</option>
                                    <option>Tất cả</option>
                                </select>
                            </div>
                            <button className="flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-slate-600 bg-white border border-slate-200 rounded-lg hover:bg-slate-50 transition-colors whitespace-nowrap">
                                <span className="material-symbols-outlined text-[18px]">filter_list</span>
                                Bộ lọc khác
                            </button>
                        </div>
                    </motion.div>

                    {/* Topics Table */}
                    <motion.div variants={item} className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead className="bg-slate-50 border-b border-slate-200 text-slate-500 text-xs uppercase font-bold tracking-wider">
                                    <tr>
                                        <th className="px-6 py-4 w-32">Mã Đề Tài</th>
                                        <th className="px-6 py-4 min-w-[300px]">Tên Đề Tài</th>
                                        <th className="px-6 py-4 w-40">Loại Đề Tài</th>
                                        <th className="px-6 py-4 w-40 text-center">Thẩm Định</th>
                                        <th className="px-6 py-4 w-40">Ngày Gửi</th>
                                        <th className="px-6 py-4 w-20 text-right"></th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100">
                                    {topics.map((topic) => (
                                        <tr key={topic.code} className="hover:bg-slate-50 transition-colors group">
                                            <td className="px-6 py-4 text-sm font-medium text-slate-500 font-mono">{topic.code}</td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-start gap-3">
                                                    <div className={`mt-0.5 bg-${topic.iconColor}-100 text-${topic.iconColor}-600 rounded p-1.5 flex-shrink-0`}>
                                                        <span className="material-symbols-outlined text-[18px]">{topic.icon}</span>
                                                    </div>
                                                    <div>
                                                        <p className="text-sm font-bold text-slate-900 group-hover:text-primary transition-colors cursor-pointer">{topic.name}</p>
                                                        <p className="text-xs text-slate-500 mt-0.5">Lĩnh vực: {topic.field}</p>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${topic.type === 'reserved' ? 'bg-indigo-50 text-indigo-700 border border-indigo-100' : 'bg-slate-100 text-slate-700 border border-slate-200'
                                                    }`}>
                                                    {topic.type === 'reserved' ? 'Chốt sẵn' : 'Trong kho'}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-center">{getStatusBadge(topic.status)}</td>
                                            <td className="px-6 py-4 text-sm text-slate-600">{topic.date}</td>
                                            <td className="px-6 py-4 text-right">
                                                <button className="size-8 rounded-full hover:bg-slate-100 flex items-center justify-center text-slate-400 hover:text-slate-600 transition-colors">
                                                    <span className="material-symbols-outlined text-[20px]">more_vert</span>
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                        <div className="bg-white border-t border-slate-200 px-6 py-4 flex items-center justify-between">
                            <p className="text-sm text-slate-500">
                                Hiển thị <span className="font-medium text-slate-900">1</span> đến <span className="font-medium text-slate-900">5</span> trong số <span className="font-medium text-slate-900">24</span> đề tài
                            </p>
                            <div className="flex items-center gap-2">
                                <button className="px-3 py-1.5 text-sm font-medium text-slate-400 bg-white border border-slate-200 rounded-md" disabled>Trước</button>
                                <button className="px-3 py-1.5 text-sm font-medium text-slate-600 bg-white border border-slate-200 rounded-md hover:bg-slate-50 transition-colors">Sau</button>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>

            {/* Modal */}
            <RegisterTopicModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </>
    )
}
