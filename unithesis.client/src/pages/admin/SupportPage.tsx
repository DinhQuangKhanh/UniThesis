import { useState } from 'react'
import { motion } from 'framer-motion'
import { Header } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.1 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const tickets = [
    {
        id: 'TICK-4922',
        sender: 'Trần Thị B (SV)',
        subject: 'Yêu cầu đổi tên đề tài (Quá hạn)',
        preview: 'Xin chào, Em muốn xin đổi tên đề tài vì lý do cá nhân...',
        time: '3 giờ trước',
        status: 'Chưa đọc',
        priority: 'high',
    },
    {
        id: 'TICK-4921',
        sender: 'Nguyễn Văn H (GV)',
        subject: 'Cập nhật tiến độ hướng dẫn',
        preview: 'Thông báo: 3 sinh viên của tôi đã hoàn thành chương 1...',
        time: '5 giờ trước',
        status: 'Đang xử lý',
        priority: 'medium',
    },
    {
        id: 'TICK-4920',
        sender: 'Hệ Thống',
        subject: 'Cảnh báo: quá tải server lúc 14:00',
        preview: 'Hệ thống ghi nhận lượng truy cập cao bất thường...',
        time: '10 giờ trước',
        status: 'Đã giải quyết',
        priority: 'low',
    },
]

export function SupportPage() {
    const [selectedTicket, setSelectedTicket] = useState(tickets[0])

    return (
        <>
            <Header title="Trung Tâm Hỗ Trợ & Yêu Cầu" showSearch={false} />

            <div className="flex-1 overflow-hidden p-8 bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="max-w-7xl mx-auto flex flex-col h-full"
                >
                    {/* Stats */}
                    <motion.div variants={item} className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6 shrink-0">
                        <StatCard icon="confirmation_number" iconColor="text-slate-600" iconBg="bg-slate-100" value={156} label="Tổng Ticket" />
                        <StatCard icon="priority_high" iconColor="text-error" iconBg="bg-error/10" value={12} label="Chưa xử lý" valueColor="text-error" />
                        <StatCard icon="schedule" iconColor="text-blue-500" iconBg="bg-blue-50" value={28} label="Đang xử lý" />
                        <StatCard icon="check_circle" iconColor="text-success" iconBg="bg-success/10" value={116} label="Đã giải quyết" />
                    </motion.div>

                    {/* Main Content */}
                    <motion.div variants={item} className="flex-1 grid grid-cols-1 lg:grid-cols-12 gap-6 min-h-0">
                        {/* Ticket List */}
                        <div className="lg:col-span-4 bento-card rounded-md overflow-hidden flex flex-col">
                            <div className="p-4 border-b border-slate-200 shrink-0">
                                <div className="flex items-center gap-2 mb-3">
                                    <button className="text-xs font-medium px-2.5 py-1 bg-primary text-white rounded-full">Tất cả</button>
                                    <button className="text-xs font-medium px-2.5 py-1 text-slate-500 hover:bg-slate-100 rounded-full transition-colors">Chưa đọc</button>
                                    <button className="text-xs font-medium px-2.5 py-1 text-slate-500 hover:bg-slate-100 rounded-full transition-colors">High Priority</button>
                                </div>
                                <div className="relative">
                                    <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-slate-400 text-[18px]">search</span>
                                    <input
                                        className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary bg-white placeholder-slate-400"
                                        placeholder="Tìm kiếm ticket..."
                                        type="text"
                                    />
                                </div>
                            </div>
                            <div className="flex-1 overflow-y-auto custom-scrollbar divide-y divide-slate-100">
                                {tickets.map((ticket) => (
                                    <div
                                        key={ticket.id}
                                        onClick={() => setSelectedTicket(ticket)}
                                        className={`p-4 cursor-pointer transition-colors ${selectedTicket.id === ticket.id ? 'bg-primary/5 border-l-4 border-l-primary' : 'hover:bg-slate-50'
                                            }`}
                                    >
                                        <div className="flex items-start gap-3">
                                            <div className={`w-2 h-2 mt-2 rounded-full shrink-0 ${ticket.priority === 'high' ? 'bg-error' : ticket.priority === 'medium' ? 'bg-yellow-500' : 'bg-slate-300'
                                                }`} />
                                            <div className="flex-1 min-w-0">
                                                <div className="flex items-center gap-2 mb-0.5">
                                                    <span className="font-bold text-slate-800 text-sm">{ticket.sender}</span>
                                                    <span className="bg-primary text-white text-[10px] font-bold px-1.5 rounded">{ticket.id}</span>
                                                </div>
                                                <h4 className="text-sm font-semibold text-slate-800 truncate">{ticket.subject}</h4>
                                                <p className="text-xs text-slate-500 line-clamp-1 mt-0.5">{ticket.preview}</p>
                                                <div className="flex items-center justify-between mt-2">
                                                    <span className="text-[10px] text-slate-400">{ticket.time}</span>
                                                    <span className={`text-[10px] font-medium px-1.5 py-0.5 rounded ${ticket.status === 'Chưa đọc'
                                                            ? 'bg-error/10 text-error'
                                                            : ticket.status === 'Đang xử lý'
                                                                ? 'bg-blue-50 text-blue-600'
                                                                : 'bg-success/10 text-success'
                                                        }`}>
                                                        {ticket.status}
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Ticket Detail */}
                        <div className="lg:col-span-8 bento-card rounded-md overflow-hidden flex flex-col">
                            <div className="p-4 border-b border-slate-200 flex items-center justify-between bg-slate-50/50 shrink-0">
                                <div className="flex items-center gap-3">
                                    <span className="bg-primary text-white text-xs font-bold px-2 py-0.5 rounded">{selectedTicket.id}</span>
                                    <h3 className="font-bold text-slate-800 text-lg truncate">{selectedTicket.subject}</h3>
                                </div>
                                <div className="flex items-center gap-2">
                                    <select className="text-xs bg-white border border-slate-200 rounded-md px-2 py-1.5 focus:ring-1 focus:ring-primary">
                                        <option>Chưa đọc</option>
                                        <option>Đang xử lý</option>
                                        <option>Đã giải quyết</option>
                                        <option>Đã đóng</option>
                                    </select>
                                    <button className="p-1.5 text-slate-400 hover:text-primary hover:bg-slate-100 rounded transition-colors">
                                        <span className="material-symbols-outlined text-[20px]">more_vert</span>
                                    </button>
                                </div>
                            </div>

                            {/* Messages */}
                            <div className="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar">
                                {/* User Message */}
                                <div className="flex gap-3">
                                    <div className="w-8 h-8 rounded-full bg-slate-200 shrink-0 flex items-center justify-center text-slate-500 text-xs font-bold">TB</div>
                                    <div className="flex-1">
                                        <div className="flex items-center gap-2 mb-1">
                                            <span className="font-semibold text-slate-800 text-sm">Trần Thị B</span>
                                            <span className="text-[10px] text-slate-400">• 3 giờ trước</span>
                                        </div>
                                        <div className="bg-slate-100 rounded-xl rounded-tl-none p-4 text-sm text-slate-700">
                                            <p>Xin chào Admin,</p>
                                            <p className="mt-2">Em là sinh viên lớp SE20A, mã số SV2024001. Em muốn xin phép đổi tên đề tài đồ án tốt nghiệp từ "Ứng dụng quản lý bán hàng" sang "Ứng dụng thương mại điện tử đa nền tảng".</p>
                                            <p className="mt-2">Lý do: Phạm vi đề tài ban đầu quá hẹp, sau khi trao đổi với GVHD, chúng em quyết định mở rộng để phù hợp hơn.</p>
                                            <p className="mt-2">Em gửi kèm đơn xin đổi tên có xác nhận của GVHD.</p>
                                            <p className="mt-2">Xin cảm ơn!</p>
                                        </div>
                                        <div className="mt-2 inline-flex items-center gap-2 p-2 bg-slate-50 border border-slate-200 rounded-lg cursor-pointer hover:bg-slate-100 transition-colors">
                                            <span className="material-symbols-outlined text-error text-[18px]">picture_as_pdf</span>
                                            <span className="text-xs font-medium text-slate-700">don_xin_doi_ten.pdf</span>
                                            <span className="material-symbols-outlined text-slate-400 text-[16px]">download</span>
                                        </div>
                                    </div>
                                </div>

                                {/* Admin Response */}
                                <div className="flex gap-3 flex-row-reverse">
                                    <div className="w-8 h-8 rounded-full bg-primary shrink-0 flex items-center justify-center text-white text-xs font-bold">AD</div>
                                    <div className="flex-1 flex flex-col items-end">
                                        <div className="flex items-center gap-2 mb-1">
                                            <span className="text-[10px] text-slate-400">2 giờ trước •</span>
                                            <span className="font-semibold text-slate-800 text-sm">Admin Support</span>
                                        </div>
                                        <div className="bg-primary text-white rounded-xl rounded-tr-none p-4 text-sm max-w-[80%]">
                                            <p>Chào bạn Trần Thị B,</p>
                                            <p className="mt-2">Yêu cầu đổi tên đề tài của bạn đã được tiếp nhận. Chúng tôi sẽ chuyển đến phòng Đào tạo để xem xét.</p>
                                            <p className="mt-2">Bạn sẽ nhận được thông báo kết quả trong vòng 2-3 ngày làm việc.</p>
                                            <p className="mt-2">Trân trọng.</p>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Reply Box */}
                            <div className="p-4 border-t border-slate-200 bg-white shrink-0">
                                <div className="relative">
                                    <textarea
                                        className="w-full h-24 p-3 pr-12 text-sm bg-slate-50 border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary resize-none"
                                        placeholder="Nhập nội dung phản hồi..."
                                    />
                                    <div className="absolute bottom-3 right-3 flex gap-1">
                                        <button className="p-1.5 text-slate-400 hover:text-primary transition-colors">
                                            <span className="material-symbols-outlined text-[20px]">attach_file</span>
                                        </button>
                                        <button className="p-1.5 bg-primary text-white rounded hover:bg-primary-light transition-colors">
                                            <span className="material-symbols-outlined text-[20px]">send</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}

function StatCard({
    icon,
    iconColor,
    iconBg,
    value,
    label,
    valueColor = 'text-slate-800',
}: {
    icon: string
    iconColor: string
    iconBg: string
    value: number
    label: string
    valueColor?: string
}) {
    return (
        <motion.div
            whileHover={{ scale: 1.02 }}
            className="bento-card p-5 rounded-md"
        >
            <div className={`w-8 h-8 rounded-md ${iconBg} ${iconColor} flex items-center justify-center mb-2`}>
                <span className="material-symbols-outlined text-[20px]">{icon}</span>
            </div>
            <h3 className={`text-2xl font-bold ${valueColor} mt-1`}>{value}</h3>
            <p className="text-xs text-slate-500 font-medium mt-0.5">{label}</p>
        </motion.div>
    )
}
