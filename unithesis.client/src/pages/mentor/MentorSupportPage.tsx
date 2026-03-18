import { useState } from 'react'
import { motion } from 'framer-motion'
import { Header } from '@/components/layout'
import { CreateTicketModal } from '@/components/support/CreateTicketModal'

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
        id: 'MEN-2001',
        subject: 'Yêu cầu thêm sinh viên vào nhóm hướng dẫn',
        preview: 'Tôi muốn thêm 2 sinh viên mới vào nhóm đồ án của tôi...',
        time: '2 giờ trước',
        status: 'Chưa đọc',
        priority: 'high',
        messages: [
            {
                sender: 'Bạn',
                initials: 'TT',
                time: '2 giờ trước',
                content: 'Xin chào Admin,\n\nTôi là TS. Trần Minh Tuấn, GVHD nhóm SE20-G05. Tôi muốn xin thêm 2 sinh viên mới từ nhóm SE20-G12 (đã giải thể) vào nhóm của tôi.\n\nMã sinh viên: SV2024015 và SV2024022.\n\nNhờ Admin hỗ trợ cập nhật trên hệ thống.\n\nTrân trọng.',
                isAdmin: false,
            },
        ],
    },
    {
        id: 'MEN-2002',
        subject: 'Lỗi hiển thị tiến độ sinh viên',
        preview: 'Biểu đồ tiến độ của nhóm tôi không cập nhật đúng...',
        time: '1 ngày trước',
        status: 'Đã trả lời',
        priority: 'medium',
        messages: [
            {
                sender: 'Bạn',
                initials: 'TT',
                time: '1 ngày trước',
                content: 'Biểu đồ tiến độ trên Dashboard của nhóm SE20-G05 không cập nhật đúng. Sinh viên đã nộp chương 2 nhưng hệ thống vẫn hiển thị "Chưa nộp".\n\nNhờ Admin kiểm tra lại.',
                isAdmin: false,
            },
            {
                sender: 'Admin Support',
                initials: 'AD',
                time: '22 giờ trước',
                content: 'Chào thầy Trần Minh Tuấn,\n\nChúng tôi đã kiểm tra và phát hiện lỗi cache dữ liệu. Hiện tại đã được khắc phục. Thầy vui lòng refresh lại trang để xem kết quả mới.\n\nXin lỗi vì sự bất tiện.\n\nTrân trọng.',
                isAdmin: true,
            },
        ],
    },
    {
        id: 'MEN-2003',
        subject: 'Hỏi về lịch bảo vệ đồ án giữa kỳ',
        preview: 'Tôi muốn xác nhận lại lịch bảo vệ đồ án giữa kỳ...',
        time: '5 ngày trước',
        status: 'Đã giải quyết',
        priority: 'low',
        messages: [
            {
                sender: 'Bạn',
                initials: 'TT',
                time: '5 ngày trước',
                content: 'Tôi muốn xác nhận lại lịch bảo vệ đồ án giữa kỳ cho nhóm SE20-G05. Theo thông báo trước là ngày 15/3 nhưng tôi chưa nhận được email xác nhận.',
                isAdmin: false,
            },
            {
                sender: 'Admin Support',
                initials: 'AD',
                time: '5 ngày trước',
                content: 'Chào thầy,\n\nLịch bảo vệ giữa kỳ nhóm SE20-G05 đã được xác nhận vào ngày 15/03/2026 lúc 14:00 tại phòng A305. Email xác nhận đã được gửi lại.\n\nThầy có thể kiểm tra trong mục "Lịch trình" trên hệ thống.\n\nTrân trọng.',
                isAdmin: true,
            },
            {
                sender: 'Bạn',
                initials: 'TT',
                time: '4 ngày trước',
                content: 'Đã nhận được. Cảm ơn Admin!',
                isAdmin: false,
            },
        ],
    },
]

export function MentorSupportPage() {
    const [selectedTicket, setSelectedTicket] = useState(tickets[0])
    const [newMessage, setNewMessage] = useState('')
    const [filter, setFilter] = useState('all')
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

    const filteredTickets = tickets.filter((t) => {
        if (filter === 'unread') return t.status === 'Chưa đọc'
        if (filter === 'resolved') return t.status === 'Đã giải quyết'
        return true
    })

    return (
        <>
            <Header title="Hỗ Trợ & Liên Hệ Admin" showSearch={false} />

            <div className="flex-1 overflow-hidden p-8 bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="flex flex-col h-full"
                >
                    {/* Stats */}
                    <motion.div variants={item} className="grid grid-cols-3 gap-4 mb-6 shrink-0">
                        <StatCard icon="confirmation_number" iconColor="text-slate-600" iconBg="bg-slate-100" value={tickets.length} label="Tổng yêu cầu" />
                        <StatCard icon="mark_email_unread" iconColor="text-error" iconBg="bg-error/10" value={tickets.filter(t => t.status === 'Chưa đọc').length} label="Chờ phản hồi" valueColor="text-error" />
                        <StatCard icon="check_circle" iconColor="text-success" iconBg="bg-success/10" value={tickets.filter(t => t.status === 'Đã giải quyết').length} label="Đã giải quyết" />
                    </motion.div>

                    {/* Main Content */}
                    <motion.div variants={item} className="flex-1 grid grid-cols-1 lg:grid-cols-12 gap-6 min-h-0">
                        {/* Ticket List */}
                        <div className="lg:col-span-4 bento-card rounded-md overflow-hidden flex flex-col">
                            <div className="p-4 border-b border-slate-200 shrink-0">
                                <div className="flex items-center gap-2 mb-3">
                                    <button onClick={() => setFilter('all')} className={`text-xs font-medium px-2.5 py-1 rounded-full transition-colors ${filter === 'all' ? 'bg-primary text-white' : 'text-slate-500 hover:bg-slate-100'}`}>Tất cả</button>
                                    <button onClick={() => setFilter('unread')} className={`text-xs font-medium px-2.5 py-1 rounded-full transition-colors ${filter === 'unread' ? 'bg-primary text-white' : 'text-slate-500 hover:bg-slate-100'}`}>Chờ phản hồi</button>
                                    <button onClick={() => setFilter('resolved')} className={`text-xs font-medium px-2.5 py-1 rounded-full transition-colors ${filter === 'resolved' ? 'bg-primary text-white' : 'text-slate-500 hover:bg-slate-100'}`}>Đã xong</button>
                                </div>
                                <div className="relative">
                                    <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-slate-400 text-[18px]">search</span>
                                    <input
                                        className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary bg-white placeholder-slate-400"
                                        placeholder="Tìm kiếm yêu cầu..."
                                        type="text"
                                    />
                                </div>
                            </div>
                            <div className="flex-1 overflow-y-auto custom-scrollbar divide-y divide-slate-100">
                                {filteredTickets.map((ticket) => (
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
                                                    <span className="bg-primary text-white text-[10px] font-bold px-1.5 rounded">{ticket.id}</span>
                                                </div>
                                                <h4 className="text-sm font-semibold text-slate-800 truncate">{ticket.subject}</h4>
                                                <p className="text-xs text-slate-500 line-clamp-1 mt-0.5">{ticket.preview}</p>
                                                <div className="flex items-center justify-between mt-2">
                                                    <span className="text-[10px] text-slate-400">{ticket.time}</span>
                                                    <span className={`text-[10px] font-medium px-1.5 py-0.5 rounded ${ticket.status === 'Chưa đọc'
                                                        ? 'bg-error/10 text-error'
                                                        : ticket.status === 'Đã trả lời'
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
                            {/* New ticket button */}
                            <div className="p-3 border-t border-slate-200 shrink-0">
                                <button 
                                    onClick={() => setIsCreateModalOpen(true)}
                                    className="w-full flex items-center justify-center gap-2 py-2.5 bg-primary text-white text-sm font-medium rounded-lg hover:bg-primary-dark transition-colors"
                                >
                                    <span className="material-symbols-outlined text-[18px]">add</span>
                                    Tạo yêu cầu mới
                                </button>
                            </div>
                        </div>

                        {/* Chat Detail */}
                        <div className="lg:col-span-8 bento-card rounded-md overflow-hidden flex flex-col">
                            <div className="p-4 border-b border-slate-200 flex items-center justify-between bg-slate-50/50 shrink-0">
                                <div className="flex items-center gap-3">
                                    <span className="bg-primary text-white text-xs font-bold px-2 py-0.5 rounded">{selectedTicket.id}</span>
                                    <h3 className="font-bold text-slate-800 text-lg truncate">{selectedTicket.subject}</h3>
                                </div>
                                <span className={`text-xs font-medium px-2.5 py-1 rounded-full ${selectedTicket.status === 'Chưa đọc'
                                    ? 'bg-error/10 text-error'
                                    : selectedTicket.status === 'Đã trả lời'
                                        ? 'bg-blue-50 text-blue-600'
                                        : 'bg-success/10 text-success'
                                    }`}>
                                    {selectedTicket.status}
                                </span>
                            </div>

                            {/* Messages */}
                            <div className="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar">
                                {selectedTicket.messages.map((msg, idx) => (
                                    <div key={idx} className={`flex gap-3 ${msg.isAdmin ? 'flex-row-reverse' : ''}`}>
                                        <div className={`w-8 h-8 rounded-full shrink-0 flex items-center justify-center text-xs font-bold ${msg.isAdmin ? 'bg-primary text-white' : 'bg-slate-200 text-slate-500'
                                            }`}>
                                            {msg.initials}
                                        </div>
                                        <div className={`flex-1 ${msg.isAdmin ? 'flex flex-col items-end' : ''}`}>
                                            <div className="flex items-center gap-2 mb-1">
                                                {msg.isAdmin ? (
                                                    <>
                                                        <span className="text-[10px] text-slate-400">• {msg.time}</span>
                                                        <span className="font-semibold text-slate-800 text-sm">{msg.sender}</span>
                                                    </>
                                                ) : (
                                                    <>
                                                        <span className="font-semibold text-slate-800 text-sm">{msg.sender}</span>
                                                        <span className="text-[10px] text-slate-400">• {msg.time}</span>
                                                    </>
                                                )}
                                            </div>
                                            <div className={`rounded-xl p-4 text-sm max-w-[80%] whitespace-pre-line ${msg.isAdmin
                                                ? 'bg-primary text-white rounded-tr-none'
                                                : 'bg-slate-100 text-slate-700 rounded-tl-none'
                                                }`}>
                                                {msg.content}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>

                            {/* Reply Box */}
                            <div className="p-4 border-t border-slate-200 bg-white shrink-0">
                                <div className="relative">
                                    <textarea
                                        value={newMessage}
                                        onChange={(e) => setNewMessage(e.target.value)}
                                        className="w-full h-24 p-3 pr-12 text-sm bg-slate-50 border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary resize-none"
                                        placeholder="Nhập nội dung tin nhắn cho Admin..."
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

            <CreateTicketModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onCreated={() => {
                    // Refetch data here later
                }}
            />
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
