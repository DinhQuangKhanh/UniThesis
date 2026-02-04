import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'

export function EvaluatorReviewPage() {
    const navigate = useNavigate()
    const [verdict, setVerdict] = useState('')
    const [showModal, setShowModal] = useState(false)
    const [feedback, setFeedback] = useState('')

    return (
        <div className="flex h-full flex-col lg:flex-row">
            {/* Main Content */}
            <div className="flex-1 flex flex-col min-w-0">
                <motion.header initial={{ opacity: 0, y: -20 }} animate={{ opacity: 1, y: 0 }} className="bg-white border-b border-gray-200 px-6 py-4 shrink-0">
                    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div className="flex items-center gap-4">
                            <button className="size-10 rounded-xl border border-gray-200 flex items-center justify-center hover:bg-gray-50">
                                <span className="material-symbols-outlined text-slate-500">arrow_back</span>
                            </button>
                            <div>
                                <div className="flex items-center gap-2">
                                    <span className="text-xs font-mono font-bold text-slate-500 bg-gray-100 px-2 py-0.5 rounded">#CS-2023-084</span>
                                    <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[10px] font-bold bg-blue-50 text-blue-600 border border-blue-100">
                                        <span className="size-1.5 rounded-full bg-blue-500 animate-pulse" />Đang thẩm định
                                    </span>
                                </div>
                                <h1 className="text-lg font-bold text-slate-900 mt-1">Hệ thống điều khiển giao thông thông minh AI</h1>
                                <p className="text-xs text-slate-500 mt-0.5"><span className="font-medium">Nguyễn Văn A</span> • K15 • Mentor: Dr. Emily Tran</p>
                            </div>
                        </div>
                        <div className="flex items-center gap-2">
                            <button onClick={() => navigate('/evaluator/similarity')} className="flex items-center gap-2 h-10 px-4 rounded-lg border border-gray-200 bg-white text-slate-700 text-sm font-semibold hover:bg-gray-50">
                                <span className="material-symbols-outlined text-[20px]">compare</span>Kiểm tra trùng lặp
                            </button>
                            <button className="flex items-center gap-2 h-10 px-4 rounded-lg border border-gray-200 bg-white text-slate-700 text-sm font-semibold hover:bg-gray-50">
                                <span className="material-symbols-outlined text-[20px]">download</span>Tải xuống
                            </button>
                        </div>
                    </div>
                </motion.header>

                <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="bg-gray-50 border-b border-gray-200 px-6 py-4">
                    <div className="flex flex-col md:flex-row gap-6">
                        <div className="flex-1">
                            <h3 className="text-xs font-bold text-slate-500 uppercase mb-2">Mô tả</h3>
                            <p className="text-sm text-slate-700">Hệ thống điều khiển giao thông thông minh sử dụng AI để tối ưu hóa luồng giao thông tại các nút giao thông phức tạp.</p>
                        </div>
                        <div className="md:w-1/3">
                            <h3 className="text-xs font-bold text-slate-500 uppercase mb-2">Mục tiêu</h3>
                            <ul className="text-sm text-slate-700 space-y-1">
                                <li className="flex items-start gap-2"><span className="material-symbols-outlined text-primary text-[16px]">check_circle</span>Phát triển thuật toán AI</li>
                                <li className="flex items-start gap-2"><span className="material-symbols-outlined text-primary text-[16px]">check_circle</span>Mô phỏng và đánh giá</li>
                                <li className="flex items-start gap-2"><span className="material-symbols-outlined text-primary text-[16px]">check_circle</span>Tích hợp hệ thống</li>
                            </ul>
                        </div>
                    </div>
                </motion.div>

                <div className="flex-1 bg-gray-100 p-6 overflow-auto">
                    <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} transition={{ delay: 0.2 }} className="max-w-4xl mx-auto bg-white rounded-xl shadow-lg border border-gray-200 overflow-hidden">
                        <div className="bg-gray-50 border-b border-gray-200 px-4 py-3 flex items-center justify-between">
                            <div className="flex items-center gap-2">
                                <button className="size-8 rounded-lg hover:bg-gray-200 flex items-center justify-center"><span className="material-symbols-outlined text-slate-600 text-[20px]">remove</span></button>
                                <span className="text-sm font-medium text-slate-600 min-w-[4rem] text-center">100%</span>
                                <button className="size-8 rounded-lg hover:bg-gray-200 flex items-center justify-center"><span className="material-symbols-outlined text-slate-600 text-[20px]">add</span></button>
                                <div className="h-6 w-px bg-gray-300 mx-2" />
                                <span className="text-sm text-slate-500">Trang <span className="font-medium text-slate-900">1</span> / <span className="font-medium text-slate-900">24</span></span>
                            </div>
                            <div className="flex items-center gap-2">
                                <button className="size-8 rounded-lg hover:bg-gray-200 flex items-center justify-center"><span className="material-symbols-outlined text-slate-600 text-[20px]">fullscreen</span></button>
                                <button className="size-8 rounded-lg hover:bg-gray-200 flex items-center justify-center"><span className="material-symbols-outlined text-slate-600 text-[20px]">print</span></button>
                            </div>
                        </div>
                        <div className="aspect-[8.5/11] bg-white flex items-center justify-center p-12">
                            <div className="text-center">
                                <span className="material-symbols-outlined text-6xl text-gray-300 mb-4 block">description</span>
                                <p className="text-lg font-semibold text-slate-700 mb-2">Tài liệu đồ án</p>
                                <p className="text-sm text-slate-500">AI_Traffic_Control_v1.2.pdf</p>
                                <p className="text-xs text-slate-400 mt-2">24 trang • 2.4 MB</p>
                            </div>
                        </div>
                    </motion.div>
                </div>
            </div>

            {/* Right Sidebar */}
            <motion.aside initial={{ opacity: 0, x: 100 }} animate={{ opacity: 1, x: 0 }} className="w-full lg:w-[380px] bg-white border-l border-gray-200 flex flex-col shrink-0">
                <div className="px-6 py-5 border-b border-gray-200">
                    <h2 className="text-lg font-bold text-slate-900 flex items-center gap-2"><span className="material-symbols-outlined text-primary">rate_review</span>Thẩm định đề tài</h2>
                    <p className="text-xs text-slate-500 mt-1">Đưa ra quyết định và phản hồi</p>
                </div>

                <div className="flex-1 overflow-y-auto p-6 flex flex-col gap-6">
                    <div>
                        <h3 className="text-xs font-bold text-slate-500 uppercase mb-3">Quyết định</h3>
                        <div className="grid grid-cols-3 gap-2">
                            {[{ v: 'approved', l: 'Duyệt', c: 'green', i: 'check_circle' }, { v: 'revision', l: 'Chỉnh sửa', c: 'amber', i: 'edit_note' }, { v: 'rejected', l: 'Từ chối', c: 'red', i: 'cancel' }].map(({ v, l, c, i }) => (
                                <button key={v} onClick={() => setVerdict(v)} className={`flex flex-col items-center justify-center p-4 rounded-xl border-2 transition-all ${verdict === v ? `border-${c}-500 bg-${c}-50` : `border-gray-200 hover:border-${c}-300`}`}>
                                    <span className={`material-symbols-outlined text-2xl mb-1 ${verdict === v ? `text-${c}-600` : 'text-gray-400'}`}>{i}</span>
                                    <span className={`text-xs font-bold ${verdict === v ? `text-${c}-600` : 'text-slate-500'}`}>{l}</span>
                                </button>
                            ))}
                        </div>
                    </div>

                    <div>
                        <h3 className="text-xs font-bold text-slate-500 uppercase mb-3">Phản hồi</h3>
                        <textarea value={feedback} onChange={(e) => setFeedback(e.target.value)} className="w-full h-32 px-4 py-3 rounded-xl border border-gray-200 bg-gray-50 text-sm resize-none focus:ring-2 focus:ring-primary/20 focus:border-primary focus:bg-white outline-none" placeholder="Nhập phản hồi..." />
                    </div>

                    <div>
                        <h3 className="text-xs font-bold text-slate-500 uppercase mb-3">Mẫu nhanh</h3>
                        <div className="flex flex-wrap gap-2">
                            {['Ứng dụng cao', 'Bổ sung methodology', 'Mở rộng LR', 'Cấu trúc tốt'].map(t => (
                                <button key={t} onClick={() => setFeedback(f => f + t + '. ')} className="px-3 py-1.5 rounded-full text-xs font-medium bg-gray-100 text-slate-600 hover:bg-primary/10 hover:text-primary">{t}</button>
                            ))}
                        </div>
                    </div>

                    <div>
                        <h3 className="text-xs font-bold text-slate-500 uppercase mb-3">Phiên bản</h3>
                        <div className="relative pl-4 space-y-3 before:absolute before:left-[7px] before:top-2 before:bottom-2 before:w-0.5 before:bg-gray-200">
                            {[{ v: 'v1.2', d: '24/10/2023', s: 'Current', c: true }, { v: 'v1.1', d: '22/10/2023', s: 'Revised', c: false }, { v: 'v1.0', d: '20/10/2023', s: 'Initial', c: false }].map(r => (
                                <div key={r.v} className="relative pl-6">
                                    <div className={`absolute left-0 top-1 size-3 rounded-full border-2 ${r.c ? 'bg-primary border-primary' : 'bg-white border-gray-300'}`} />
                                    <div className="flex items-center justify-between">
                                        <div><p className="text-sm font-bold text-slate-900">{r.v}</p><p className="text-xs text-slate-500">{r.d}</p></div>
                                        <span className={`px-2 py-0.5 rounded-full text-[10px] font-bold ${r.c ? 'bg-green-50 text-green-600' : 'bg-gray-100 text-gray-600'}`}>{r.s}</span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>

                <div className="px-6 py-4 border-t border-gray-200 flex gap-3">
                    <button className="flex-1 h-11 rounded-xl border border-gray-200 text-slate-700 font-semibold text-sm hover:bg-gray-50">Lưu nháp</button>
                    <button disabled={!verdict} className="flex-1 h-11 rounded-xl bg-primary text-white font-semibold text-sm hover:bg-primary-dark shadow-lg shadow-primary/20 disabled:opacity-50 disabled:cursor-not-allowed">Gửi thẩm định</button>
                </div>
            </motion.aside>

            <AnimatePresence>
                {showModal && (
                    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4" onClick={() => setShowModal(false)}>
                        <motion.div initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} onClick={e => e.stopPropagation()} className="bg-white rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden">
                            <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                                <div className="flex items-center gap-3">
                                    <div className="size-10 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center"><span className="material-symbols-outlined">compare</span></div>
                                    <div><h3 className="text-lg font-bold text-slate-900">Phân tích trùng lặp</h3><p className="text-xs text-slate-500">Kết quả kiểm tra sáo chép</p></div>
                                </div>
                                <button onClick={() => setShowModal(false)} className="size-8 rounded-lg hover:bg-gray-100 flex items-center justify-center"><span className="material-symbols-outlined text-slate-500">close</span></button>
                            </div>
                            <div className="p-6 space-y-4">
                                <div className="flex items-center gap-4 p-4 bg-gray-50 rounded-xl">
                                    <div className="relative size-16 shrink-0">
                                        <svg className="size-full -rotate-90" viewBox="0 0 36 36"><path className="text-gray-200" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="currentColor" strokeWidth="4" /><path className="text-green-500" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="currentColor" strokeDasharray="18, 100" strokeLinecap="round" strokeWidth="4" /></svg>
                                        <div className="absolute inset-0 flex items-center justify-center"><span className="text-lg font-bold text-slate-900">18%</span></div>
                                    </div>
                                    <div><p className="text-sm font-bold text-green-600 mb-1">Mức độ tương đồng thấp</p><p className="text-xs text-slate-500">Tài liệu có mức độ tương đồng 18%, đây là mức chấp nhận được.</p></div>
                                </div>
                                <div className="space-y-2">
                                    {[{ n: 'IEEE Paper: AI Traffic', s: 'ieee.org', p: '8%', c: 'amber' }, { n: 'Thesis 2022: Smart City', s: 'Internal', p: '6%', c: 'green' }, { n: 'Wikipedia: Traffic', s: 'wikipedia.org', p: '4%', c: 'green' }].map(i => (
                                        <div key={i.n} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                                            <div className="flex items-center gap-3"><span className="material-symbols-outlined text-slate-400">article</span><div><p className="text-sm font-medium text-slate-900">{i.n}</p><p className="text-xs text-slate-500">{i.s}</p></div></div>
                                            <span className={`text-xs font-bold text-${i.c}-600 bg-${i.c}-50 px-2 py-1 rounded`}>{i.p}</span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                            <div className="px-6 py-4 border-t border-gray-200 flex justify-end"><button onClick={() => setShowModal(false)} className="h-10 px-6 rounded-lg bg-primary text-white font-semibold text-sm hover:bg-primary-dark">Đóng</button></div>
                        </motion.div>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    )
}
