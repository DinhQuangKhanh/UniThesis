import { useState } from 'react'
import { motion } from 'framer-motion'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const topics = [
    {
        id: 1,
        title: 'Phát triển hệ thống quản lý học tập tích hợp AI (LMS AI)',
        mentor: 'TS. Nguyễn Khắc Hùng',
        major: 'CNTT',
        majorColor: 'blue',
        status: 'available',
        maxStudents: 3,
    },
    {
        id: 2,
        title: 'Hệ thống phân tích cảm xúc người dùng trên mạng xã hội bằng Deep Learning',
        mentor: 'ThS. Lê Thị Thu Hà',
        major: 'KTPM',
        majorColor: 'purple',
        status: 'taken',
        group: 'Nhóm 14 - K62CNTT',
    },
    {
        id: 3,
        title: 'Ứng dụng IoT trong giám sát năng lượng tiêu thụ tại tòa nhà thông minh',
        mentor: 'PGS. TS. Trần Văn Đạo',
        major: 'HTTT',
        majorColor: 'green',
        status: 'available',
        maxStudents: 2,
    },
    {
        id: 4,
        title: 'Phân tích dữ liệu lớn để dự báo xu hướng tiêu dùng thương mại điện tử',
        mentor: 'TS. Đặng Minh Tuấn',
        major: 'KHDL',
        majorColor: 'blue',
        status: 'available',
        maxStudents: 2,
    },
    {
        id: 5,
        title: 'Xây dựng ứng dụng ví điện tử hỗ trợ giao dịch Blockchain doanh nghiệp',
        mentor: 'ThS. Vũ Anh Đức',
        major: 'CNTT',
        majorColor: 'blue',
        status: 'taken',
        group: 'Blockchain Team A',
    },
    {
        id: 6,
        title: 'Tự động hóa quy trình CI/CD cho hạ tầng Microservices trên Cloud',
        mentor: 'TS. Phạm Thanh Sơn',
        major: 'KTPM',
        majorColor: 'purple',
        status: 'available',
        maxStudents: 3,
    },
]

export function StudentTopicsPage() {
    const [favorites, setFavorites] = useState<number[]>([])

    const toggleFavorite = (id: number) => {
        setFavorites((prev) =>
            prev.includes(id) ? prev.filter((f) => f !== id) : [...prev, id]
        )
    }

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
                            placeholder="Tìm kiếm đề tài đề xuất..."
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
                    {/* Page Header */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h2 className="text-2xl font-bold text-primary">Kho Đề Tài Mentor Đề Xuất</h2>
                            <p className="text-[#58698d] text-sm mt-1">
                                Danh sách các đề tài do Giảng viên đề xuất cho sinh viên đăng ký thực hiện.
                            </p>
                        </div>
                        <div className="flex items-center gap-3">
                            <button className="flex items-center gap-2 bg-white border border-[#e9ecf1] px-4 py-2 rounded-lg text-sm font-semibold text-[#101319] hover:bg-gray-50 transition-colors">
                                <span className="material-symbols-outlined text-xl">bookmark</span>
                                Quan tâm ({favorites.length})
                            </button>
                        </div>
                    </motion.div>

                    {/* Filters */}
                    <motion.div variants={item} className="bg-white p-5 rounded-xl border border-[#e9ecf1] shadow-sm">
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                            <div className="flex flex-col gap-1.5 md:col-span-2">
                                <label className="text-xs font-bold text-[#58698d] uppercase tracking-wider">Chuyên ngành</label>
                                <select className="form-select w-full border-[#e9ecf1] rounded-lg text-sm focus:ring-primary focus:border-primary">
                                    <option>Tất cả chuyên ngành</option>
                                    <option>Công nghệ thông tin</option>
                                    <option>Hệ thống thông tin</option>
                                    <option>Kỹ thuật phần mềm</option>
                                    <option>Khoa học dữ liệu</option>
                                </select>
                            </div>
                            <div className="flex flex-col gap-1.5">
                                <label className="text-xs font-bold text-[#58698d] uppercase tracking-wider">Trạng thái</label>
                                <select className="form-select w-full border-[#e9ecf1] rounded-lg text-sm focus:ring-primary focus:border-primary">
                                    <option>Tất cả trạng thái</option>
                                    <option>Còn trống</option>
                                    <option>Đã có nhóm</option>
                                </select>
                            </div>
                            <div className="flex items-end">
                                <button className="w-full bg-[#f6f7f8] hover:bg-gray-200 text-[#101319] font-bold py-2 px-4 rounded-lg text-sm transition-colors flex items-center justify-center gap-2">
                                    <span className="material-symbols-outlined text-lg">filter_alt_off</span>
                                    Xóa bộ lọc
                                </button>
                            </div>
                        </div>
                    </motion.div>

                    {/* Topics Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                        {topics.map((topic) => (
                            <motion.div
                                key={topic.id}
                                variants={item}
                                className="bg-white rounded-xl border border-[#e9ecf1] shadow-sm hover:shadow-md transition-all group overflow-hidden flex flex-col relative"
                            >
                                <div className="absolute top-0 right-0">
                                    <div className="bg-primary/10 text-primary text-[10px] font-bold px-3 py-1 rounded-bl-lg border-l border-b border-primary/20">
                                        ĐỀ TÀI TỪ KHO
                                    </div>
                                </div>
                                <div className="p-6 flex-1">
                                    <div className="flex justify-between items-start mb-4">
                                        <span className={`bg-${topic.majorColor}-50 text-${topic.majorColor}-600 px-2.5 py-0.5 rounded text-[10px] font-bold uppercase tracking-wider border border-${topic.majorColor}-100`}>
                                            {topic.major}
                                        </span>
                                        <span className={`px-2.5 py-0.5 rounded text-[10px] font-bold uppercase tracking-wider italic ${topic.status === 'available'
                                                ? 'bg-green-50 text-green-600 border border-green-100'
                                                : 'bg-gray-50 text-gray-400 border border-gray-100'
                                            }`}>
                                            {topic.status === 'available' ? 'Còn trống' : 'Đã có nhóm'}
                                        </span>
                                    </div>
                                    <h3 className="text-lg font-bold text-[#101319] group-hover:text-primary transition-colors leading-tight mb-3">
                                        {topic.title}
                                    </h3>
                                    <div className="flex flex-col gap-2 mt-auto">
                                        <div className="flex items-center gap-2 text-sm text-[#58698d]">
                                            <span className="material-symbols-outlined text-lg">school</span>
                                            <span className="font-medium italic">Mentor: {topic.mentor}</span>
                                        </div>
                                        <div className="flex items-center gap-2 text-xs text-[#58698d]">
                                            <span className="material-symbols-outlined text-base text-gray-400">info</span>
                                            <span>
                                                {topic.status === 'available'
                                                    ? `Số lượng SV tối đa: ${topic.maxStudents} sinh viên`
                                                    : `Nhóm: ${topic.group}`}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="px-6 py-4 bg-gray-50/50 border-t border-[#e9ecf1] flex gap-3">
                                    <button
                                        onClick={() => toggleFavorite(topic.id)}
                                        className={`bg-white border py-2 px-3 rounded-lg transition-colors flex items-center justify-center ${favorites.includes(topic.id)
                                                ? 'border-red-200 bg-red-50 text-red-500'
                                                : 'border-[#e9ecf1] text-[#58698d] hover:text-red-500 hover:border-red-200 hover:bg-red-50'
                                            }`}
                                        title="Quan tâm"
                                    >
                                        <span className={`material-symbols-outlined text-xl ${favorites.includes(topic.id) ? 'fill-1' : ''}`}>favorite</span>
                                    </button>
                                    <button className="flex-1 bg-white border border-[#e9ecf1] text-[#101319] py-2 rounded-lg text-xs font-bold hover:bg-gray-50 transition-colors flex items-center justify-center gap-1.5">
                                        <span className="material-symbols-outlined text-base">visibility</span>
                                        Chi tiết
                                    </button>
                                    {topic.status === 'available' ? (
                                        <button className="flex-1 bg-primary text-white py-2 rounded-lg text-xs font-bold hover:bg-primary-light transition-colors flex items-center justify-center gap-1.5">
                                            <span className="material-symbols-outlined text-base">app_registration</span>
                                            Đăng ký đề tài
                                        </button>
                                    ) : (
                                        <button className="flex-1 bg-gray-200 text-gray-500 py-2 rounded-lg text-xs font-bold cursor-not-allowed flex items-center justify-center gap-1.5" disabled>
                                            <span className="material-symbols-outlined text-base">app_registration</span>
                                            Đã đăng ký
                                        </button>
                                    )}
                                </div>
                            </motion.div>
                        ))}
                    </div>

                    {/* Pagination */}
                    <motion.div variants={item} className="flex items-center justify-between bg-white px-6 py-4 rounded-xl border border-[#e9ecf1] shadow-sm mt-4">
                        <div className="text-sm text-[#58698d]">
                            Hiển thị <span className="font-bold text-[#101319]">1-6</span> trên <span className="font-bold text-[#101319]">45</span> đề tài
                        </div>
                        <div className="flex gap-2">
                            <button className="p-2 rounded-lg border border-[#e9ecf1] hover:bg-gray-50 text-[#58698d] transition-colors disabled:opacity-50" disabled>
                                <span className="material-symbols-outlined text-xl">chevron_left</span>
                            </button>
                            <button className="h-10 w-10 rounded-lg bg-primary text-white font-bold text-sm">1</button>
                            <button className="h-10 w-10 rounded-lg border border-[#e9ecf1] hover:bg-gray-50 text-[#58698d] font-bold text-sm transition-colors">2</button>
                            <button className="h-10 w-10 rounded-lg border border-[#e9ecf1] hover:bg-gray-50 text-[#58698d] font-bold text-sm transition-colors">3</button>
                            <span className="px-2 self-center text-[#58698d]">...</span>
                            <button className="h-10 w-10 rounded-lg border border-[#e9ecf1] hover:bg-gray-50 text-[#58698d] font-bold text-sm transition-colors">8</button>
                            <button className="p-2 rounded-lg border border-[#e9ecf1] hover:bg-gray-50 text-[#58698d] transition-colors">
                                <span className="material-symbols-outlined text-xl">chevron_right</span>
                            </button>
                        </div>
                    </motion.div>

                    {/* Footer */}
                    <div className="mt-8 pt-6 border-t border-[#e9ecf1] flex flex-col md:flex-row justify-between items-center text-[#58698d] text-sm pb-8">
                        <p>© 2023 University Thesis Management System.</p>
                        <div className="flex gap-4 mt-2 md:mt-0">
                            <a className="hover:text-primary transition-colors" href="#">Quy định bảo mật</a>
                            <a className="hover:text-primary transition-colors" href="#">Điều khoản sử dụng</a>
                        </div>
                    </div>
                </motion.div>
            </div>
        </>
    )
}
