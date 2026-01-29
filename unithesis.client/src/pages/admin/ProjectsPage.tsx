import { motion } from 'framer-motion'
import { Header } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

export function ProjectsPage() {
    return (
        <>
            <Header title="Quản Lý Tổng Đề Tài" />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="max-w-7xl mx-auto flex flex-col h-full"
                >
                    {/* Admin Notice */}
                    <motion.div variants={item} className="bg-blue-50 border border-blue-100 rounded-lg p-4 mb-6 flex items-start gap-3">
                        <span className="material-symbols-outlined text-blue-600 shrink-0">info</span>
                        <p className="text-sm text-blue-700">
                            <span className="font-semibold">Lưu ý:</span> Admin chỉ có quyền xem và xuất báo cáo. Quyền phê duyệt, chỉnh sửa đề tài thuộc về Hội đồng và GVHD.{' '}
                            <a href="#" className="text-primary font-semibold hover:underline">Tìm hiểu thêm</a>
                        </p>
                    </motion.div>

                    {/* Filters */}
                    <motion.div variants={item} className="flex flex-wrap items-center gap-4 mb-6">
                        <div className="flex-1 min-w-0 flex flex-wrap gap-3">
                            <select className="bg-white border border-slate-200 text-sm rounded-md pl-3 pr-8 py-2 focus:ring-1 focus:ring-primary focus:border-primary">
                                <option>Spring 2024</option>
                                <option>Fall 2023</option>
                                <option>Summer 2023</option>
                            </select>
                            <select className="bg-white border border-slate-200 text-sm rounded-md pl-3 pr-8 py-2 focus:ring-1 focus:ring-primary focus:border-primary">
                                <option>Tất cả trạng thái</option>
                                <option>Chờ duyệt</option>
                                <option>Đã duyệt</option>
                                <option>Đã hoàn thành</option>
                            </select>
                            <select className="bg-white border border-slate-200 text-sm rounded-md pl-3 pr-8 py-2 focus:ring-1 focus:ring-primary focus:border-primary">
                                <option>Tất cả chuyên ngành</option>
                                <option>Kỹ thuật PM</option>
                                <option>KHMT</option>
                                <option>ATTT</option>
                            </select>
                            <select className="bg-white border border-slate-200 text-sm rounded-md pl-3 pr-8 py-2 focus:ring-1 focus:ring-primary focus:border-primary">
                                <option>Tất cả mentor</option>
                                <option>TS. Lê Thị Bích</option>
                                <option>ThS. Phạm Văn Minh</option>
                            </select>
                        </div>
                        <div className="flex items-center gap-2">
                            <button className="flex items-center gap-2 bg-primary hover:bg-primary-light text-white px-4 py-2 rounded-md text-sm font-medium transition-colors shadow-sm">
                                <span className="material-symbols-outlined text-[18px]">download</span>
                                Xuất Báo Cáo
                            </button>
                        </div>
                    </motion.div>

                    {/* Table */}
                    <motion.div variants={item} className="bento-card rounded-md overflow-hidden bg-white flex flex-col flex-1 min-h-0">
                        <div className="overflow-auto flex-1">
                            <table className="w-full text-left text-sm text-slate-600">
                                <thead className="bg-slate-50 text-xs uppercase font-bold text-slate-500 border-b border-slate-200 sticky top-0 z-10">
                                    <tr>
                                        <th className="px-6 py-4 w-24">ID</th>
                                        <th className="px-6 py-4">Tên Đề Tài & Sinh Viên</th>
                                        <th className="px-6 py-4">Chuyên Ngành</th>
                                        <th className="px-6 py-4">Mentor / GVHD</th>
                                        <th className="px-6 py-4">Trạng thái</th>
                                        <th className="px-6 py-4">Kỳ học</th>
                                        <th className="px-6 py-4 text-right w-20">Chi tiết</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100">
                                    <ProjectRow
                                        id="#PROJ-241"
                                        title="Hệ thống quản lý chuỗi cung ứng bằng Blockchain"
                                        students={['Nguyễn Văn An', 'Trần Thị B']}
                                        major="Kỹ thuật PM"
                                        majorColor="bg-blue-50 text-blue-700 border-blue-100"
                                        mentor="TS. Lê Thị Bích"
                                        mentorAvatar="https://lh3.googleusercontent.com/aida-public/AB6AXuAVLWRq7X6rhNLONqAo4Ou3t_Yq24D93xalGopZG_vTL5pmrXIEfQZ_qdoBNxlfRbSpt9jIceOrReAqOq5ehxchSXjj_AUE4PSjjEsv4xvURmpEUW4E4CO7P4eCPBL01LW-VcHJSy4GJ3zrV21J1ut0pWiO_qgid_2kE-iK9gDgSi0O6QPDvyaK_4qi_q2CkcbqwT-l4zEBf2rAu-8KyAmT4PBgijmeHGEMQjWJkUJMqBlwaKbOWo51SrAWwYTYWKupke-yK_nsrddB"
                                        status="pending"
                                        semester="Spring 2024"
                                    />
                                    <ProjectRow
                                        id="#PROJ-239"
                                        title="Ứng dụng AI trong chẩn đoán hình ảnh y tế"
                                        students={['Lê Hoàng C', 'Phạm D']}
                                        major="KHMT"
                                        majorColor="bg-purple-50 text-purple-700 border-purple-100"
                                        mentor="ThS. Phạm Văn Minh"
                                        status="approved"
                                        semester="Spring 2024"
                                    />
                                    <ProjectRow
                                        id="#PROJ-198"
                                        title="Phát triển hệ thống phát hiện xâm nhập mạng"
                                        students={['Võ E']}
                                        major="ATTT"
                                        majorColor="bg-red-50 text-red-700 border-red-100"
                                        mentor="TS. Nguyễn Thị F"
                                        mentorAvatar="https://avatar.iran.liara.run/public"
                                        status="completed"
                                        semester="Fall 2023"
                                    />
                                    <ProjectRow
                                        id="#PROJ-240"
                                        title="Nền tảng thương mại điện tử đa kênh"
                                        students={['Đinh G', 'Hoàng H', 'Ngô I']}
                                        major="Kỹ thuật PM"
                                        majorColor="bg-blue-50 text-blue-700 border-blue-100"
                                        mentor="ThS. Trần K"
                                        status="pending"
                                        semester="Spring 2024"
                                    />
                                    <ProjectRow
                                        id="#PROJ-195"
                                        title="Phân tích dữ liệu lớn trong dự báo thời tiết"
                                        students={['Bùi L', 'Đỗ M']}
                                        major="KHMT"
                                        majorColor="bg-purple-50 text-purple-700 border-purple-100"
                                        mentor="TS. Lê Thị Bích"
                                        mentorAvatar="https://lh3.googleusercontent.com/aida-public/AB6AXuAVLWRq7X6rhNLONqAo4Ou3t_Yq24D93xalGopZG_vTL5pmrXIEfQZ_qdoBNxlfRbSpt9jIceOrReAqOq5ehxchSXjj_AUE4PSjjEsv4xvURmpEUW4E4CO7P4eCPBL01LW-VcHJSy4GJ3zrV21J1ut0pWiO_qgid_2kE-iK9gDgSi0O6QPDvyaK_4qi_q2CkcbqwT-l4zEBf2rAu-8KyAmT4PBgijmeHGEMQjWJkUJMqBlwaKbOWo51SrAWwYTYWKupke-yK_nsrddB"
                                        status="completed"
                                        semester="Fall 2023"
                                    />
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination */}
                        <div className="p-4 border-t border-slate-200 flex items-center justify-between bg-white shrink-0">
                            <span className="text-sm text-slate-500 hidden sm:inline">
                                Hiển thị <span className="font-medium text-slate-900">1-5</span> trên <span className="font-medium text-slate-900">856</span> đề tài
                            </span>
                            <div className="flex gap-1 w-full sm:w-auto justify-center sm:justify-end">
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm disabled:opacity-50 transition-colors">Trước</button>
                                <button className="px-3 py-1 bg-primary text-white rounded text-sm hover:bg-primary-light transition-colors">1</button>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">2</button>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">3</button>
                                <span className="px-2 py-1 text-slate-400 hidden sm:inline">...</span>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">Sau</button>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}

function ProjectRow({
    id,
    title,
    students,
    major,
    majorColor,
    mentor,
    mentorAvatar,
    status,
    semester,
}: {
    id: string
    title: string
    students: string[]
    major: string
    majorColor: string
    mentor: string
    mentorAvatar?: string
    status: 'pending' | 'approved' | 'completed'
    semester: string
}) {
    const statusConfig = {
        pending: { label: 'Chờ duyệt', color: 'bg-yellow-50 text-yellow-700 border-yellow-200', icon: 'schedule' },
        approved: { label: 'Đã duyệt', color: 'bg-blue-50 text-blue-700 border-blue-200', icon: 'check' },
        completed: { label: 'Đã hoàn thành', color: 'bg-success/10 text-success border-success/20', icon: 'verified' },
    }

    const s = statusConfig[status]

    return (
        <motion.tr whileHover={{ backgroundColor: 'rgb(248 250 252)' }} className="transition-colors group">
            <td className="px-6 py-4 font-mono text-xs font-bold text-primary">{id}</td>
            <td className="px-6 py-4">
                <div>
                    <span className="text-slate-800 font-bold line-clamp-2 leading-snug">{title}</span>
                    <p className="text-xs text-slate-500 mt-1 flex items-center gap-1">
                        <span className="material-symbols-outlined text-[14px]">group</span>
                        {students.join(', ')}
                    </p>
                </div>
            </td>
            <td className="px-6 py-4">
                <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium border ${majorColor}`}>
                    {major}
                </span>
            </td>
            <td className="px-6 py-4">
                <div className="flex items-center gap-2">
                    {mentorAvatar ? (
                        <div className="w-6 h-6 rounded-full bg-slate-200 bg-cover bg-center" style={{ backgroundImage: `url("${mentorAvatar}")` }} />
                    ) : (
                        <div className="w-6 h-6 rounded-full bg-slate-100 flex items-center justify-center">
                            <span className="material-symbols-outlined text-slate-400 text-[14px]">person</span>
                        </div>
                    )}
                    <span className="text-sm">{mentor}</span>
                </div>
            </td>
            <td className="px-6 py-4">
                <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium border ${s.color}`}>
                    <span className="material-symbols-outlined text-[14px]">{s.icon}</span>
                    {s.label}
                </span>
            </td>
            <td className="px-6 py-4 text-xs text-slate-600">{semester}</td>
            <td className="px-6 py-4 text-right">
                <button className="p-1.5 text-slate-400 hover:text-primary hover:bg-primary/5 rounded transition-colors opacity-0 group-hover:opacity-100" title="Xem chi tiết">
                    <span className="material-symbols-outlined text-[20px]">visibility</span>
                </button>
            </td>
        </motion.tr>
    )
}
