import { useState } from 'react'
import { motion } from 'framer-motion'
import { Header } from '@/components/layout'
import { CreateSemesterModal } from '@/components/admin/CreateSemesterModal'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.1 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

export function SemestersPage() {
    const [isModalOpen, setIsModalOpen] = useState(false)
    return (
        <>
            <Header
                variant="navy"
                title="Quản lý kỳ học"
                breadcrumb={[{ label: 'Quản lý kỳ học' }]}
                searchPlaceholder="Tìm kiếm kỳ học..."
            />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="max-w-7xl mx-auto space-y-6"
                >
                    {/* Header */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h2 className="text-2xl font-bold text-slate-800 tracking-tight">Danh Sách Kỳ Học</h2>
                            <p className="text-sm text-slate-500 mt-1">Quản lý các kỳ bảo vệ đồ án, tiến độ và mốc thời gian.</p>
                        </div>
                        <div className="flex gap-3">
                            <button className="flex items-center gap-2 bg-white hover:bg-slate-50 text-slate-600 border border-slate-300 px-4 py-2.5 rounded-md shadow-sm transition-all font-medium text-sm">
                                <span className="material-symbols-outlined text-[20px]">filter_list</span>
                                Bộ lọc
                            </button>
                            <button onClick={() => setIsModalOpen(true)} className="flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-4 py-2.5 rounded-md shadow-sm transition-all font-medium text-sm">
                                <span className="material-symbols-outlined text-[20px]">add</span>
                                Tạo kỳ học mới
                            </button>
                        </div>
                    </motion.div>

                    {/* Semester Cards */}
                    <div className="space-y-6">
                        {/* Active Semester */}
                        <motion.div variants={item} className="bento-card rounded-lg overflow-hidden transition-all hover:shadow-md border-l-4 border-l-primary">
                            <div className="p-6">
                                <div className="flex flex-col lg:flex-row justify-between lg:items-start gap-4 mb-6">
                                    <div className="flex items-start gap-4">
                                        <div className="w-12 h-12 rounded-lg bg-primary/10 flex items-center justify-center text-primary shrink-0 border border-primary/20">
                                            <span className="material-symbols-outlined text-[28px]">calendar_month</span>
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-3 flex-wrap">
                                                <h3 className="text-xl font-bold text-slate-800">Spring 2024</h3>
                                                <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-green-100 text-green-700 border border-green-200">
                                                    <span className="w-1.5 h-1.5 rounded-full bg-green-600"></span>
                                                    Active
                                                </span>
                                            </div>
                                            <p className="text-sm text-slate-500 mt-1">Thời gian: <span className="font-medium text-slate-700">02/01/2024 - 15/05/2024</span></p>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        <button className="p-2 text-slate-400 hover:text-primary hover:bg-slate-100 rounded-full transition-colors" title="Cấu hình">
                                            <span className="material-symbols-outlined">settings</span>
                                        </button>
                                        <button className="p-2 text-slate-400 hover:text-primary hover:bg-slate-100 rounded-full transition-colors" title="Thêm hành động">
                                            <span className="material-symbols-outlined">more_vert</span>
                                        </button>
                                    </div>
                                </div>

                                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 border-t border-slate-100 pt-6">
                                    {/* Stats */}
                                    <div className="lg:col-span-4 space-y-4">
                                        <p className="text-xs font-semibold uppercase text-slate-400 tracking-wider flex items-center gap-2">
                                            <span className="material-symbols-outlined text-[16px]">bar_chart</span>
                                            Thống kê
                                        </p>
                                        <div className="grid grid-cols-2 gap-3">
                                            <div className="p-3 bg-slate-50 rounded border border-slate-100">
                                                <p className="text-xs text-slate-500 mb-1">Tổng đề tài</p>
                                                <div className="flex items-end gap-2">
                                                    <p className="text-lg font-bold text-slate-800">856</p>
                                                    <span className="text-[10px] text-green-600 font-medium mb-1">▲ 12%</span>
                                                </div>
                                            </div>
                                            <div className="p-3 bg-slate-50 rounded border border-slate-100">
                                                <p className="text-xs text-slate-500 mb-1">Sinh viên</p>
                                                <p className="text-lg font-bold text-slate-800">2,450</p>
                                            </div>
                                            <div className="p-3 bg-slate-50 rounded border border-slate-100">
                                                <p className="text-xs text-slate-500 mb-1">Giảng viên</p>
                                                <p className="text-lg font-bold text-slate-800">142</p>
                                            </div>
                                            <div className="p-3 bg-orange-50 rounded border border-orange-100">
                                                <p className="text-xs text-orange-600 mb-1">Chờ duyệt</p>
                                                <p className="text-lg font-bold text-orange-700">45</p>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Timeline */}
                                    <div className="lg:col-span-8 flex flex-col justify-between">
                                        <div>
                                            <div className="flex justify-between items-end mb-4">
                                                <p className="text-xs font-semibold uppercase text-slate-400 tracking-wider flex items-center gap-2">
                                                    <span className="material-symbols-outlined text-[16px]">timeline</span>
                                                    Tiến độ hiện tại
                                                </p>
                                                <span className="text-sm font-bold text-primary bg-primary/5 px-3 py-1 rounded border border-primary/10">Giai đoạn: Thẩm Định</span>
                                            </div>
                                            <div className="relative pt-4 pb-8 px-2">
                                                <div className="absolute top-1/2 left-0 w-full h-1 bg-slate-100 -translate-y-1/2 rounded-full z-0"></div>
                                                <div className="absolute top-1/2 left-0 w-[42%] h-1 bg-gradient-to-r from-blue-400 to-primary -translate-y-1/2 rounded-full z-0"></div>
                                                <div className="relative z-10 flex justify-between w-full">
                                                    <TimelineStep icon="check" label="Đăng ký" status="completed" info="Hoàn tất 15/01" />
                                                    <TimelineStep icon="sync" label="Thẩm định" status="current" info="Đang diễn ra" />
                                                    <TimelineStep icon="science" label="Thực hiện" status="pending" info="Dự kiến 01/03" />
                                                    <TimelineStep icon="school" label="Bảo vệ" status="pending" info="Dự kiến 15/05" />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="bg-blue-50 border border-blue-100 rounded-md p-3 flex items-start gap-3 mt-2">
                                            <span className="material-symbols-outlined text-blue-600 text-[20px] shrink-0 mt-0.5">info</span>
                                            <div>
                                                <p className="text-sm font-semibold text-blue-800">Sự kiện sắp tới: Công bố kết quả thẩm định đợt 1</p>
                                                <p className="text-xs text-blue-600 mt-0.5">Hạn chót cập nhật điểm: 20/02/2024 (còn 5 ngày)</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </motion.div>

                        {/* Closed Semester */}
                        <motion.div variants={item} className="bento-card rounded-lg overflow-hidden transition-all hover:shadow-md opacity-90 hover:opacity-100">
                            <div className="p-6">
                                <div className="flex flex-col lg:flex-row justify-between lg:items-start gap-4 mb-6">
                                    <div className="flex items-start gap-4">
                                        <div className="w-12 h-12 rounded-lg bg-slate-100 flex items-center justify-center text-slate-500 shrink-0 border border-slate-200">
                                            <span className="material-symbols-outlined text-[28px]">history</span>
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-3">
                                                <h3 className="text-xl font-bold text-slate-700">Fall 2023</h3>
                                                <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-slate-100 text-slate-600 border border-slate-200">
                                                    Closed
                                                </span>
                                            </div>
                                            <p className="text-sm text-slate-500 mt-1">Thời gian: <span className="font-medium text-slate-600">05/09/2023 - 30/12/2023</span></p>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        <button className="text-sm font-medium text-primary hover:text-primary-light transition-colors hover:underline">
                                            Xem báo cáo tổng kết
                                        </button>
                                    </div>
                                </div>

                                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 border-t border-slate-100 pt-6">
                                    <div className="lg:col-span-4 space-y-4">
                                        <p className="text-xs font-semibold uppercase text-slate-400 tracking-wider">Kết quả kỳ học</p>
                                        <div className="grid grid-cols-2 gap-3">
                                            <div className="p-3 bg-slate-50 rounded border border-slate-100">
                                                <p className="text-xs text-slate-500 mb-1">Tỷ lệ hoàn thành</p>
                                                <p className="text-lg font-bold text-green-600">98.5%</p>
                                            </div>
                                            <div className="p-3 bg-slate-50 rounded border border-slate-100">
                                                <p className="text-xs text-slate-500 mb-1">Tổng đề tài</p>
                                                <p className="text-lg font-bold text-slate-700">742</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="lg:col-span-8">
                                        <div className="flex justify-between items-end mb-4">
                                            <p className="text-xs font-semibold uppercase text-slate-400 tracking-wider">Trạng thái tiến độ</p>
                                            <span className="text-sm font-medium text-slate-600 flex items-center gap-1">
                                                <span className="material-symbols-outlined text-[18px] text-green-600">check_circle</span>
                                                Đã hoàn thành toàn bộ
                                            </span>
                                        </div>
                                        <div className="relative pt-4 pb-2 px-2">
                                            <div className="absolute top-1/2 left-0 w-full h-1 bg-green-500/20 -translate-y-1/2 rounded-full z-0"></div>
                                            <div className="absolute top-1/2 left-0 w-full h-1 bg-green-600 -translate-y-1/2 rounded-full z-0"></div>
                                            <div className="relative z-10 flex justify-between w-full">
                                                <div className="w-4 h-4 rounded-full bg-green-600 ring-2 ring-white"></div>
                                                <div className="w-4 h-4 rounded-full bg-green-600 ring-2 ring-white"></div>
                                                <div className="w-4 h-4 rounded-full bg-green-600 ring-2 ring-white"></div>
                                                <div className="w-4 h-4 rounded-full bg-green-600 ring-2 ring-white"></div>
                                            </div>
                                            <div className="flex justify-between w-full mt-3 text-[10px] text-slate-400 font-medium uppercase tracking-wide">
                                                <span>Đăng ký</span>
                                                <span>Thẩm định</span>
                                                <span>Thực hiện</span>
                                                <span>Bảo vệ</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </motion.div>

                        {/* Compact Semester Card */}
                        <motion.div variants={item} className="bento-card rounded-lg overflow-hidden transition-all hover:shadow-md opacity-80 hover:opacity-100">
                            <div className="p-6 flex flex-col md:flex-row justify-between items-center gap-4">
                                <div className="flex items-start gap-4">
                                    <div className="w-10 h-10 rounded-lg bg-slate-100 flex items-center justify-center text-slate-400 shrink-0 border border-slate-200">
                                        <span className="material-symbols-outlined text-[24px]">history</span>
                                    </div>
                                    <div>
                                        <div className="flex items-center gap-3">
                                            <h3 className="text-lg font-bold text-slate-700">Summer 2023</h3>
                                            <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold bg-slate-100 text-slate-600 border border-slate-200">
                                                Closed
                                            </span>
                                        </div>
                                        <p className="text-xs text-slate-500 mt-1">Thời gian: 05/05/2023 - 25/08/2023 • 320 Đề tài</p>
                                    </div>
                                </div>
                                <button className="text-sm font-medium text-slate-500 hover:text-primary transition-colors flex items-center gap-1">
                                    Chi tiết
                                    <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
                                </button>
                            </div>
                        </motion.div>
                    </div>
                </motion.div>
            </div>
            <CreateSemesterModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
        </>
    )
}

function TimelineStep({ icon, label, status, info }: { icon: string; label: string; status: 'completed' | 'current' | 'pending'; info: string }) {
    const isCurrent = status === 'current'
    const isPending = status === 'pending'

    return (
        <div className={`flex flex-col items-center gap-2 group cursor-pointer ${isPending ? 'opacity-60' : ''}`}>
            <div
                className={`rounded-full flex items-center justify-center ring-4 ring-white transition-transform group-hover:scale-110 ${isCurrent
                    ? 'w-10 h-10 bg-white border-4 border-primary text-primary shadow-lg shadow-primary/20'
                    : status === 'completed'
                        ? 'w-8 h-8 bg-primary text-white shadow-sm'
                        : 'w-8 h-8 bg-slate-100 border-2 border-slate-300 text-slate-400'
                    }`}
            >
                <span className={`material-symbols-outlined ${isCurrent ? 'text-[20px]' : 'text-[16px]'}`}>{icon}</span>
            </div>
            <div className="text-center">
                <p className={`text-xs font-bold uppercase ${isCurrent ? 'text-sm text-primary' : isPending ? 'text-slate-500' : 'text-slate-700'}`}>{label}</p>
                {isCurrent ? (
                    <p className="text-[10px] text-primary font-medium bg-primary/5 px-2 py-0.5 rounded mt-0.5">{info}</p>
                ) : (
                    <p className="text-[10px] text-slate-400">{info}</p>
                )}
            </div>
        </div>
    )
}
