import { motion, AnimatePresence } from 'framer-motion'

interface RegisterTopicModalProps {
    isOpen: boolean
    onClose: () => void
}

export function RegisterTopicModal({ isOpen, onClose }: RegisterTopicModalProps) {
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
                        className="bg-white w-full max-w-5xl max-h-[90vh] rounded-xl shadow-2xl overflow-hidden flex flex-col"
                    >
                        {/* Header */}
                        <div className="px-8 py-5 border-b border-slate-100 flex items-center justify-between bg-white shrink-0">
                            <div className="flex items-center gap-4">
                                <nav className="flex text-sm font-medium text-slate-500">
                                    <span className="hover:text-slate-900">Đề tài</span>
                                    <span className="mx-2 text-slate-400">/</span>
                                    <span className="text-slate-900 font-bold">Đăng ký mới</span>
                                </nav>
                            </div>
                            <button onClick={onClose} className="text-slate-400 hover:text-slate-600 transition-colors p-1 hover:bg-slate-100 rounded-lg">
                                <span className="material-symbols-outlined">close</span>
                            </button>
                        </div>

                        {/* Content */}
                        <div className="flex-1 overflow-y-auto p-8">
                            <div className="mb-6">
                                <h1 className="text-2xl font-extrabold text-slate-900 tracking-tight">Đăng Ký Đề Tài Mới</h1>
                                <p className="text-slate-500 mt-1">Điền thông tin chi tiết để gửi đề tài lên hội đồng thẩm định xét duyệt.</p>
                            </div>

                            <form onSubmit={(e) => e.preventDefault()} className="space-y-10">
                                {/* Section 1: Thông tin cơ bản */}
                                <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }} className="space-y-6">
                                    <div className="flex items-center gap-3 pb-2 border-b border-slate-100">
                                        <div className="bg-blue-50 text-primary p-1.5 rounded-md">
                                            <span className="material-symbols-outlined text-[20px]">info</span>
                                        </div>
                                        <h3 className="text-lg font-bold text-slate-800">Thông tin cơ bản</h3>
                                    </div>
                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                        <div className="space-y-2 md:col-span-2">
                                            <label className="block text-sm font-semibold text-slate-700">
                                                Tên đề tài (Tiếng Việt) <span className="text-red-500">*</span>
                                            </label>
                                            <input
                                                type="text"
                                                className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm py-2.5"
                                                placeholder="Nhập tên đề tài đầy đủ bằng tiếng Việt..."
                                            />
                                        </div>
                                        <div className="space-y-2 md:col-span-2">
                                            <label className="block text-sm font-semibold text-slate-700">Tên đề tài (Tiếng Anh)</label>
                                            <input
                                                type="text"
                                                className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm py-2.5"
                                                placeholder="Enter project name in English..."
                                            />
                                        </div>
                                        <div className="space-y-2">
                                            <label className="block text-sm font-semibold text-slate-700">
                                                Chuyên ngành <span className="text-red-500">*</span>
                                            </label>
                                            <select className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm py-2.5 text-slate-700">
                                                <option disabled selected value="">Chọn chuyên ngành phù hợp</option>
                                                <option value="se">Kỹ thuật phần mềm (SE)</option>
                                                <option value="ai">Trí tuệ nhân tạo (AI)</option>
                                                <option value="is">An toàn thông tin (IS)</option>
                                                <option value="its">Hệ thống thông tin (ITS)</option>
                                                <option value="gd">Thiết kế đồ họa (GD)</option>
                                            </select>
                                        </div>
                                        <div className="space-y-2">
                                            <label className="block text-sm font-semibold text-slate-700">Số lượng sinh viên tối đa</label>
                                            <select className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm py-2.5 text-slate-700">
                                                <option value="4">4 Sinh viên (Tiêu chuẩn)</option>
                                                <option value="5">5 Sinh viên</option>
                                                <option value="3">3 Sinh viên</option>
                                            </select>
                                        </div>
                                    </div>
                                </motion.section>

                                {/* Section 2: Nội dung & Yêu cầu */}
                                <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }} className="space-y-6">
                                    <div className="flex items-center gap-3 pb-2 border-b border-slate-100">
                                        <div className="bg-indigo-50 text-indigo-600 p-1.5 rounded-md">
                                            <span className="material-symbols-outlined text-[20px]">description</span>
                                        </div>
                                        <h3 className="text-lg font-bold text-slate-800">Nội dung &amp; Yêu cầu</h3>
                                    </div>
                                    <div className="space-y-6">
                                        <div className="space-y-2">
                                            <label className="block text-sm font-semibold text-slate-700">
                                                Mục tiêu đề tài <span className="text-red-500">*</span>
                                            </label>
                                            <textarea
                                                rows={4}
                                                className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm leading-relaxed"
                                                placeholder="Mô tả mục tiêu chính mà sinh viên cần đạt được..."
                                            />
                                            <p className="text-xs text-slate-500 text-right">0/500 ký tự</p>
                                        </div>
                                        <div className="space-y-2">
                                            <label className="block text-sm font-semibold text-slate-700">
                                                Phạm vi &amp; Công nghệ sử dụng <span className="text-red-500">*</span>
                                            </label>
                                            <textarea
                                                rows={3}
                                                className="block w-full rounded-lg border-slate-300 shadow-sm focus:border-primary focus:ring-primary sm:text-sm leading-relaxed"
                                                placeholder="Liệt kê các công nghệ, giới hạn phạm vi nghiên cứu..."
                                            />
                                        </div>
                                    </div>
                                </motion.section>

                                {/* Section 3: Hình thức đăng ký */}
                                <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }} className="space-y-6">
                                    <div className="flex items-center gap-3 pb-2 border-b border-slate-100">
                                        <div className="bg-amber-50 text-amber-600 p-1.5 rounded-md">
                                            <span className="material-symbols-outlined text-[20px]">alt_route</span>
                                        </div>
                                        <h3 className="text-lg font-bold text-slate-800">Hình thức đăng ký</h3>
                                    </div>
                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                        <label className="relative flex items-start p-4 cursor-pointer rounded-xl border-2 border-primary bg-blue-50/30 shadow-sm transition-all hover:border-primary hover:bg-blue-50">
                                            <div className="flex items-center h-5">
                                                <input type="radio" name="project_flow" defaultChecked className="focus:ring-primary h-4 w-4 text-primary border-gray-300" />
                                            </div>
                                            <div className="ml-3">
                                                <div className="flex items-center gap-2">
                                                    <span className="material-symbols-outlined text-primary text-[20px]">public</span>
                                                    <span className="block text-sm font-bold text-slate-900">Đưa vào kho đề tài chung</span>
                                                </div>
                                                <p className="mt-1 text-xs text-slate-600">Đề tài sẽ được công khai. Sinh viên có thể xem và gửi yêu cầu đăng ký tham gia.</p>
                                            </div>
                                        </label>
                                        <label className="relative flex items-start p-4 cursor-pointer rounded-xl border border-slate-200 bg-white hover:border-primary/50 hover:bg-slate-50 transition-all">
                                            <div className="flex items-center h-5">
                                                <input type="radio" name="project_flow" className="focus:ring-primary h-4 w-4 text-primary border-gray-300" />
                                            </div>
                                            <div className="ml-3">
                                                <div className="flex items-center gap-2">
                                                    <span className="material-symbols-outlined text-slate-500 text-[20px]">group_add</span>
                                                    <span className="block text-sm font-bold text-slate-900">Chỉ định nhóm sinh viên</span>
                                                </div>
                                                <p className="mt-1 text-xs text-slate-600">Dành cho các nhóm đã chốt trước. Bạn sẽ nhập mã sinh viên để gán trực tiếp.</p>
                                            </div>
                                        </label>
                                    </div>
                                </motion.section>

                                {/* Section 4: Tài liệu */}
                                <motion.section initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.4 }} className="space-y-6">
                                    <div className="flex items-center gap-3 pb-2 border-b border-slate-100">
                                        <div className="bg-emerald-50 text-emerald-600 p-1.5 rounded-md">
                                            <span className="material-symbols-outlined text-[20px]">attachment</span>
                                        </div>
                                        <h3 className="text-lg font-bold text-slate-800">Tài liệu mô tả</h3>
                                    </div>
                                    <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-slate-300 border-dashed rounded-xl hover:bg-slate-50 transition-colors cursor-pointer group">
                                        <div className="space-y-1 text-center">
                                            <div className="mx-auto size-12 bg-slate-100 rounded-full flex items-center justify-center text-slate-400 group-hover:text-primary group-hover:bg-blue-50 transition-colors">
                                                <span className="material-symbols-outlined text-[24px]">cloud_upload</span>
                                            </div>
                                            <div className="flex text-sm text-slate-600 justify-center">
                                                <label className="relative cursor-pointer bg-transparent rounded-md font-medium text-primary hover:text-primary/80">
                                                    <span>Tải lên file</span>
                                                    <input type="file" className="sr-only" />
                                                </label>
                                                <p className="pl-1">hoặc kéo thả vào đây</p>
                                            </div>
                                            <p className="text-xs text-slate-500">PDF, DOCX, XLSX tối đa 10MB</p>
                                        </div>
                                    </div>
                                </motion.section>
                            </form>
                        </div>

                        {/* Footer */}
                        <div className="px-8 py-5 bg-slate-50 border-t border-slate-200 flex items-center justify-end gap-4 shrink-0">
                            <button
                                type="button"
                                onClick={onClose}
                                className="px-5 py-2.5 rounded-lg border border-slate-300 bg-white text-slate-700 text-sm font-semibold shadow-sm hover:bg-slate-50 transition-all"
                            >
                                Hủy bỏ
                            </button>
                            <button
                                type="submit"
                                className="px-6 py-2.5 rounded-lg bg-primary hover:bg-primary/90 text-white text-sm font-semibold shadow-md shadow-blue-900/10 transition-all flex items-center gap-2"
                            >
                                <span className="material-symbols-outlined text-[18px]">send</span>
                                Gửi phê duyệt
                            </button>
                        </div>
                    </motion.div>
                </motion.div>
            )}
        </AnimatePresence>
    )
}
