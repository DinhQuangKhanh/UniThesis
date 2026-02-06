import { motion } from 'framer-motion'
import { NotificationDropdown } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const files = [
    { name: 'Bao-cao-khao-sat.pdf', size: '2.4 MB', time: '2 ngày trước', type: 'pdf', color: 'red' },
    { name: 'Nghiên cứu lý thuyết.docx', size: '1.1 MB', time: '1 tuần trước', type: 'doc', color: 'blue' },
    { name: 'Sơ đồ hệ thống.png', size: '850 KB', time: '12/10/2023', type: 'image', color: 'green' },
]

const expectedResults = [
    { title: 'Mô hình AI nhận dạng', icon: 'data_object', color: 'blue', description: 'Mô hình có khả năng nhận diện ký tự tiếng Việt với độ chính xác trên 95% và phân loại tài liệu vào 10 chuyên mục chính.' },
    { title: 'Ứng dụng Web hoàn chỉnh', icon: 'web', color: 'green', description: 'Giao diện thân thiện, hỗ trợ đa nền tảng, tích hợp đầy đủ các chức năng quản lý, tìm kiếm và phân tích báo cáo.' },
    { title: 'Báo cáo khoa học', icon: 'history_edu', color: 'orange', description: 'Cuốn báo cáo chi tiết về quy trình nghiên cứu, thiết kế hệ thống và đánh giá hiệu năng của mô hình AI.' },
]

export function StudentMyTopicPage() {
    return (
        <>
            {/* Header */}
            <header className="bg-primary h-16 flex items-center justify-between px-8 shrink-0 z-50 sticky top-0 shadow-md">
                <div className="flex items-center gap-4 flex-1 max-w-xl">
                    <div className="relative w-full group">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-blue-200/70 group-focus-within:text-white transition-colors">search</span>
                        </div>
                        <input
                            className="block w-full pl-10 pr-3 py-2 border-none rounded-lg leading-5 bg-white/10 text-white placeholder-blue-200/70 focus:outline-none focus:bg-white/20 focus:ring-1 focus:ring-blue-300 transition-all sm:text-sm h-10"
                            placeholder="Tìm kiếm thông tin đề tài..."
                            type="text"
                        />
                    </div>
                </div>
                <div className="flex items-center gap-6">
                    <NotificationDropdown role="student" isNavy={true} />
                    <div className="h-8 w-[1px] bg-white/20" />
                    <div className="flex items-center gap-3 text-white">
                        <span className="text-sm font-medium">Hệ thống thẩm định ĐT</span>
                        <span className="material-symbols-outlined">help</span>
                    </div>
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8">
                <motion.div variants={container} initial="hidden" animate="show" className="max-w-[1200px] mx-auto flex flex-col gap-6">
                    {/* Breadcrumb */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <div className="flex items-center gap-2 text-sm text-[#58698d] mb-1">
                                <span>Hệ thống</span>
                                <span className="material-symbols-outlined text-sm">chevron_right</span>
                                <span className="text-primary font-semibold">Đề tài của tôi</span>
                            </div>
                            <h2 className="text-2xl font-bold text-[#101319]">Chi tiết Nội dung Đề tài</h2>
                        </div>
                    </motion.div>

                    {/* Topic Header Card */}
                    <motion.section variants={item} className="bg-white rounded-xl border border-[#e9ecf1] shadow-sm overflow-hidden">
                        <div className="p-8 border-b border-[#e9ecf1]">
                            <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-6">
                                <div className="max-w-4xl">
                                    <div className="flex items-center gap-2 mb-3">
                                        <span className="bg-blue-100 text-primary px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider">
                                            Mã đề tài: DT2023-CNTT01
                                        </span>
                                        <span className="bg-green-100 text-green-600 px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider">
                                            Đang thực hiện
                                        </span>
                                    </div>
                                    <h1 className="text-2xl font-extrabold text-[#101319] leading-tight mb-4">
                                        Xây dựng hệ thống quản lý thư viện số tích hợp công nghệ AI nhận dạng tài liệu
                                    </h1>
                                    <div className="flex flex-wrap gap-6 text-sm">
                                        <div className="flex items-center gap-3">
                                            <div className="h-10 w-10 rounded-full bg-gray-100 flex items-center justify-center text-primary">
                                                <span className="material-symbols-outlined">person</span>
                                            </div>
                                            <div>
                                                <p className="text-[#58698d] text-xs">Giảng viên hướng dẫn</p>
                                                <p className="font-bold text-[#101319]">TS. Trần Minh Tuấn</p>
                                            </div>
                                        </div>
                                        <div className="flex items-center gap-3">
                                            <div className="h-10 w-10 rounded-full bg-gray-100 flex items-center justify-center text-primary">
                                                <span className="material-symbols-outlined">calendar_today</span>
                                            </div>
                                            <div>
                                                <p className="text-[#58698d] text-xs">Thời gian bắt đầu</p>
                                                <p className="font-bold text-[#101319]">05/09/2023</p>
                                            </div>
                                        </div>
                                        <div className="flex items-center gap-3">
                                            <div className="h-10 w-10 rounded-full bg-gray-100 flex items-center justify-center text-primary">
                                                <span className="material-symbols-outlined">event_available</span>
                                            </div>
                                            <div>
                                                <p className="text-[#58698d] text-xs">Hạn nộp bản cuối</p>
                                                <p className="font-bold text-[#101319]">15/12/2023</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </motion.section>

                    {/* Main Content Grid */}
                    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                        {/* Left Column - Topic Details */}
                        <motion.div variants={item} className="lg:col-span-2 space-y-6">
                            <div className="bg-white rounded-xl border border-[#e9ecf1] shadow-sm flex flex-col">
                                <div className="p-5 border-b border-[#e9ecf1] flex items-center justify-between">
                                    <h3 className="font-bold text-[#101319] flex items-center gap-2">
                                        <span className="material-symbols-outlined text-primary">description</span>
                                        Mô tả chi tiết đề tài
                                    </h3>
                                </div>
                                <div className="p-8 space-y-8">
                                    {/* Objective */}
                                    <div>
                                        <h4 className="text-sm font-bold text-primary uppercase tracking-wider mb-3 flex items-center gap-2">
                                            <span className="w-1.5 h-6 bg-primary rounded-full" />
                                            1. Mục tiêu đề tài
                                        </h4>
                                        <div className="text-[#101319] text-sm leading-relaxed space-y-3 pl-3.5">
                                            <p>Nghiên cứu và phát triển một hệ thống quản lý thư viện số hiện đại, có khả năng tự động hóa việc phân loại và nhận dạng tài liệu thông qua công nghệ Trí tuệ nhân tạo (AI).</p>
                                            <ul className="list-disc pl-5 space-y-2 text-[#58698d]">
                                                <li>Tối ưu hóa quy trình nhập liệu và số hóa tài liệu giấy thông qua OCR.</li>
                                                <li>Xây dựng công cụ tìm kiếm thông minh dựa trên ngữ nghĩa nội dung tài liệu.</li>
                                                <li>Nâng cao trải nghiệm người dùng trong việc quản lý và mượn trả tài liệu trực tuyến.</li>
                                            </ul>
                                        </div>
                                    </div>

                                    {/* Scope */}
                                    <div>
                                        <h4 className="text-sm font-bold text-primary uppercase tracking-wider mb-3 flex items-center gap-2">
                                            <span className="w-1.5 h-6 bg-primary rounded-full" />
                                            2. Phạm vi nghiên cứu
                                        </h4>
                                        <div className="text-[#101319] text-sm leading-relaxed pl-3.5">
                                            <p className="mb-3">Đề tài tập trung triển khai tại Thư viện trung tâm của Trường Đại học, tập trung vào các mảng:</p>
                                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                                <div className="p-3 bg-gray-50 rounded-lg border border-gray-100">
                                                    <p className="font-bold text-xs mb-1">Công nghệ sử dụng</p>
                                                    <p className="text-xs text-[#58698d]">Python, TensorFlow/Keras cho AI; ReactJS cho Frontend; Node.js cho Backend.</p>
                                                </div>
                                                <div className="p-3 bg-gray-50 rounded-lg border border-gray-100">
                                                    <p className="font-bold text-xs mb-1">Đối tượng tài liệu</p>
                                                    <p className="text-xs text-[#58698d]">Sách chuyên khảo, Luận văn tốt nghiệp, Tạp chí khoa học định dạng PDF và hình ảnh.</p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Expected Results */}
                                    <div>
                                        <h4 className="text-sm font-bold text-primary uppercase tracking-wider mb-3 flex items-center gap-2">
                                            <span className="w-1.5 h-6 bg-primary rounded-full" />
                                            3. Kết quả dự kiến
                                        </h4>
                                        <div className="grid grid-cols-1 gap-4 pl-3.5">
                                            {expectedResults.map((result) => (
                                                <div key={result.title} className="flex items-start gap-4 p-4 border border-[#e9ecf1] rounded-xl hover:bg-gray-50 transition-colors">
                                                    <div className={`bg-${result.color}-50 p-2 rounded-lg text-${result.color}-600`}>
                                                        <span className="material-symbols-outlined">{result.icon}</span>
                                                    </div>
                                                    <div>
                                                        <p className="text-sm font-bold text-[#101319]">{result.title}</p>
                                                        <p className="text-xs text-[#58698d] mt-1">{result.description}</p>
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </motion.div>

                        {/* Right Column - Files */}
                        <motion.div variants={item} className="lg:col-span-1 space-y-6">
                            <div className="bg-white rounded-xl border border-[#e9ecf1] shadow-sm flex flex-col">
                                <div className="p-5 border-b border-[#e9ecf1] flex items-center justify-between">
                                    <h3 className="font-bold text-[#101319] flex items-center gap-2">
                                        <span className="material-symbols-outlined text-primary">cloud_upload</span>
                                        Tài liệu đã tải lên
                                    </h3>
                                    <button className="text-xs font-bold text-primary hover:underline">Tất cả</button>
                                </div>
                                <div className="p-4 flex flex-col gap-3">
                                    {files.map((file) => (
                                        <div key={file.name} className="flex items-center gap-3 p-3 bg-[#f6f7f8] rounded-lg hover:bg-gray-100 transition-colors group cursor-pointer">
                                            <div className={`p-2 bg-${file.color}-100 text-${file.color}-600 rounded`}>
                                                <span className="material-symbols-outlined text-xl">
                                                    {file.type === 'pdf' ? 'description' : file.type === 'doc' ? 'article' : 'image'}
                                                </span>
                                            </div>
                                            <div className="flex-1 min-w-0">
                                                <p className="text-xs font-bold text-[#101319] truncate">{file.name}</p>
                                                <p className="text-[10px] text-[#58698d]">{file.size} • {file.time}</p>
                                            </div>
                                            <span className="material-symbols-outlined text-lg text-gray-400 group-hover:text-primary">download</span>
                                        </div>
                                    ))}
                                    <button className="w-full mt-2 py-3 border-2 border-dashed border-gray-200 rounded-lg flex flex-col items-center justify-center gap-1 hover:border-primary hover:bg-primary/5 transition-all text-[#58698d] hover:text-primary">
                                        <span className="material-symbols-outlined">add_circle</span>
                                        <span className="text-xs font-bold">Tải lên tài liệu mới</span>
                                    </button>
                                </div>
                            </div>
                        </motion.div>
                    </div>

                    {/* Footer */}
                    <footer className="mt-12 pt-6 border-t border-[#e9ecf1] flex flex-col md:flex-row justify-between items-center text-[#58698d] text-sm pb-8">
                        <p>© 2023 University Thesis Management System.</p>
                        <div className="flex gap-4 mt-2 md:mt-0">
                            <a className="hover:text-primary" href="#">Quy định bảo mật</a>
                            <a className="hover:text-primary" href="#">Điều khoản sử dụng</a>
                        </div>
                    </footer>
                </motion.div>
            </div>
        </>
    )
}
