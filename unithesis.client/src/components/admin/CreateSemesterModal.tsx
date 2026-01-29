import { motion, AnimatePresence } from 'framer-motion'

interface CreateSemesterModalProps {
    isOpen: boolean
    onClose: () => void
}

export function CreateSemesterModal({ isOpen, onClose }: CreateSemesterModalProps) {
    return (
        <AnimatePresence>
            {isOpen && (
                <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 backdrop-blur-sm p-4"
                    onClick={onClose}
                >
                    <motion.div
                        initial={{ opacity: 0, scale: 0.95, y: 20 }}
                        animate={{ opacity: 1, scale: 1, y: 0 }}
                        exit={{ opacity: 0, scale: 0.95, y: 20 }}
                        transition={{ type: 'spring', damping: 25, stiffness: 300 }}
                        onClick={(e) => e.stopPropagation()}
                        className="bg-white w-full max-w-4xl max-h-[90vh] rounded-xl shadow-2xl overflow-hidden flex flex-col"
                    >
                        {/* Header */}
                        <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between bg-white shrink-0">
                            <div>
                                <h2 className="text-xl font-bold text-slate-800">Tạo Kỳ Học Mới</h2>
                                <p className="text-sm text-slate-500">Thiết lập thời gian và đối tượng tham gia đồ án</p>
                            </div>
                            <button onClick={onClose} className="text-slate-400 hover:text-slate-600 transition-colors p-1 hover:bg-slate-100 rounded-lg">
                                <span className="material-symbols-outlined">close</span>
                            </button>
                        </div>

                        {/* Content */}
                        <div className="flex-1 overflow-y-auto p-6 space-y-8">
                            {/* Section 1: General Info */}
                            <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
                                <div className="flex items-center gap-2 mb-4">
                                    <span className="w-8 h-8 rounded bg-blue-50 text-primary flex items-center justify-center font-bold text-sm">1</span>
                                    <h3 className="font-bold text-slate-700">Thông tin chung</h3>
                                </div>
                                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                                    <div className="md:col-span-1">
                                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Tên kỳ học <span className="text-red-500">*</span></label>
                                        <input className="w-full border border-slate-200 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary text-sm outline-none transition-all" placeholder="VD: Summer 2024" type="text" />
                                    </div>
                                    <div>
                                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Ngày bắt đầu <span className="text-red-500">*</span></label>
                                        <input className="w-full border border-slate-200 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary text-sm outline-none transition-all" type="date" />
                                    </div>
                                    <div>
                                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Ngày kết thúc <span className="text-red-500">*</span></label>
                                        <input className="w-full border border-slate-200 rounded-md px-3 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary text-sm outline-none transition-all" type="date" />
                                    </div>
                                </div>
                            </motion.section>

                            {/* Section 2: Timeline */}
                            <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
                                <div className="flex items-center gap-2 mb-4">
                                    <span className="w-8 h-8 rounded bg-blue-50 text-primary flex items-center justify-center font-bold text-sm">2</span>
                                    <h3 className="font-bold text-slate-700">Thiết lập giai đoạn (Timeline)</h3>
                                </div>
                                <div className="bg-slate-50 rounded-lg p-4 border border-slate-100">
                                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                                        {[
                                            { label: '1. Đăng ký', color: 'text-primary' },
                                            { label: '2. Thẩm định', color: 'text-orange-600' },
                                            { label: '3. Thực hiện', color: 'text-emerald-600' },
                                            { label: '4. Bảo vệ', color: 'text-purple-600' },
                                        ].map((phase) => (
                                            <div key={phase.label} className="bg-white p-3 rounded border border-slate-200 shadow-sm hover:shadow-md transition-shadow">
                                                <p className={`text-xs font-bold ${phase.color} uppercase mb-2`}>{phase.label}</p>
                                                <input className="w-full border-none p-0 text-sm focus:ring-0 bg-transparent" type="date" placeholder="Từ ngày" />
                                                <div className="h-px bg-slate-100 my-1" />
                                                <input className="w-full border-none p-0 text-sm focus:ring-0 bg-transparent" type="date" placeholder="Đến ngày" />
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </motion.section>

                            {/* Section 3: Participants */}
                            <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
                                <div className="flex items-center gap-2 mb-4">
                                    <span className="w-8 h-8 rounded bg-blue-50 text-primary flex items-center justify-center font-bold text-sm">3</span>
                                    <div className="flex items-center gap-2">
                                        <h3 className="font-bold text-slate-700">Đối tượng tham gia</h3>
                                        <span className="bg-primary text-white text-[10px] px-1.5 py-0.5 rounded font-bold uppercase tracking-tight">Quan trọng</span>
                                    </div>
                                </div>
                                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                                    {/* Upload Excel */}
                                    <div className="border-2 border-dashed border-slate-200 rounded-xl p-6 hover:border-primary/50 transition-colors group cursor-pointer">
                                        <div className="flex flex-col items-center text-center">
                                            <div className="w-12 h-12 rounded-full bg-slate-100 flex items-center justify-center text-slate-400 group-hover:bg-primary/10 group-hover:text-primary transition-colors mb-3">
                                                <span className="material-symbols-outlined text-3xl">upload_file</span>
                                            </div>
                                            <h4 className="font-bold text-slate-800 mb-1">Tải danh sách Excel</h4>
                                            <p className="text-xs text-slate-500 mb-4 px-4">Tải lên file .xlsx chứa danh sách MSSV đủ điều kiện làm đồ án kỳ này.</p>
                                            <button className="w-full py-2 px-4 bg-white border border-slate-300 rounded-md text-sm font-semibold text-slate-700 hover:bg-slate-50 transition-colors">Chọn tệp tin</button>
                                            <a className="mt-3 text-[11px] text-primary hover:underline flex items-center gap-1" href="#">
                                                <span className="material-symbols-outlined text-[14px]">download</span> Tải file mẫu
                                            </a>
                                        </div>
                                    </div>

                                    {/* Filter */}
                                    <div className="border border-slate-200 rounded-xl p-6 bg-slate-50/50">
                                        <h4 className="font-bold text-slate-800 mb-3 flex items-center gap-2">
                                            <span className="material-symbols-outlined text-[20px] text-primary">filter_alt</span>
                                            Lọc theo điều kiện
                                        </h4>
                                        <div className="space-y-4">
                                            <div>
                                                <label className="block text-xs font-semibold text-slate-500 uppercase mb-1.5">Khóa & Ngành</label>
                                                <div className="grid grid-cols-2 gap-2">
                                                    <select className="text-xs border border-slate-200 rounded-md px-2 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none">
                                                        <option>Tất cả Khóa</option>
                                                        <option>K2020</option>
                                                        <option>K2021</option>
                                                    </select>
                                                    <select className="text-xs border border-slate-200 rounded-md px-2 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none">
                                                        <option>Tất cả Ngành</option>
                                                        <option>CNTT</option>
                                                        <option>Kỹ thuật phần mềm</option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div>
                                                <label className="block text-xs font-semibold text-slate-500 uppercase mb-1.5">Điểm tích lũy (GPA) tối thiểu</label>
                                                <div className="flex items-center gap-3">
                                                    <input className="w-full text-xs border border-slate-200 rounded-md px-2 py-2 focus:ring-2 focus:ring-primary/20 focus:border-primary outline-none" placeholder="VD: 2.5" step="0.1" type="number" />
                                                    <span className="text-xs text-slate-400 font-medium whitespace-nowrap">/ 4.0</span>
                                                </div>
                                            </div>
                                            <div className="pt-2">
                                                <button className="w-full py-2 px-4 bg-primary/10 text-primary border border-primary/20 rounded-md text-xs font-bold hover:bg-primary/20 transition-all">
                                                    Xem trước danh sách (245 SV)
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                {/* Info Note */}
                                <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.4 }} className="mt-4 p-3 bg-blue-50 border border-blue-100 rounded-md flex items-start gap-3">
                                    <span className="material-symbols-outlined text-blue-600 text-[20px] mt-0.5">verified</span>
                                    <p className="text-xs text-blue-800 leading-relaxed">
                                        <strong>Lưu ý:</strong> Sau khi xác nhận, hệ thống sẽ tự động gắn thẻ
                                        <span className="px-1.5 py-0.5 bg-blue-100 border border-blue-200 rounded text-[10px] font-bold mx-1">ĐỦ ĐIỀU KIỆN LÀM ĐỒ ÁN</span>
                                        cho các sinh viên thuộc danh sách trên. Chỉ những SV này mới có quyền đăng ký đề tài trong kỳ học.
                                    </p>
                                </motion.div>
                            </motion.section>
                        </div>

                        {/* Footer */}
                        <div className="px-6 py-4 border-t border-slate-100 flex items-center justify-end gap-3 bg-slate-50/50 shrink-0">
                            <button onClick={onClose} className="px-4 py-2 text-sm font-semibold text-slate-600 hover:text-slate-800 transition-colors">Hủy bỏ</button>
                            <button className="px-6 py-2 bg-primary text-white rounded-md text-sm font-bold shadow-lg shadow-primary/20 hover:bg-primary/90 transition-all">
                                Khởi tạo kỳ học
                            </button>
                        </div>
                    </motion.div>
                </motion.div>
            )}
        </AnimatePresence>
    )
}
