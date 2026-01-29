import { motion } from 'framer-motion'
import { Header } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: {
        opacity: 1,
        transition: { staggerChildren: 0.1 }
    }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

export function ReportsPage() {
    return (
        <>
            <Header
                title="Báo Cáo Chuyên Sâu Khối CNTT"
                subtitle="Phân tích hiệu suất hệ thống & chất lượng đào tạo"
                actions={
                    <div className="flex items-center gap-4">
                        <div className="hidden md:flex items-center gap-2 bg-slate-100 rounded-md p-1 border border-slate-200">
                            <button className="px-3 py-1.5 text-xs font-medium bg-white text-primary rounded shadow-sm border border-slate-200">
                                Năm học 2023-2024
                            </button>
                            <button className="px-3 py-1.5 text-xs font-medium text-slate-500 hover:text-slate-800 transition-colors">HK1</button>
                            <button className="px-3 py-1.5 text-xs font-medium text-slate-500 hover:text-slate-800 transition-colors">HK2</button>
                        </div>
                        <button className="flex items-center gap-2 bg-primary hover:bg-primary-light text-white px-4 py-2 rounded-md text-sm font-medium transition-colors shadow-sm">
                            <span className="material-symbols-outlined text-[20px]">download</span>
                            <span>Xuất Báo Cáo</span>
                        </button>
                    </div>
                }
            />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="max-w-7xl mx-auto space-y-6"
                >
                    {/* Filter Bar */}
                    <motion.div variants={item} className="bento-card p-4 rounded-md flex flex-wrap gap-4 items-center justify-between">
                        <div className="flex items-center gap-4 flex-1">
                            <div className="relative w-48">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 material-symbols-outlined text-[18px]">calendar_month</span>
                                <select className="w-full pl-10 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-md text-sm text-slate-700 focus:ring-1 focus:ring-primary focus:border-primary">
                                    <option>5 năm gần nhất</option>
                                    <option>3 năm gần nhất</option>
                                    <option>Năm nay</option>
                                </select>
                            </div>
                            <div className="relative w-56">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 material-symbols-outlined text-[18px]">domain</span>
                                <select className="w-full pl-10 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-md text-sm text-slate-700 focus:ring-1 focus:ring-primary focus:border-primary">
                                    <option>Tất cả các Ngành</option>
                                    <option>Kỹ thuật phần mềm</option>
                                    <option>Khoa học máy tính</option>
                                    <option>An toàn thông tin</option>
                                </select>
                            </div>
                            <button className="px-4 py-2 text-sm font-medium text-primary bg-primary/5 hover:bg-primary/10 rounded-md transition-colors border border-primary/10">
                                Áp dụng bộ lọc
                            </button>
                        </div>
                        <div className="flex items-center gap-2 text-xs text-slate-500">
                            <span className="material-symbols-outlined text-[16px]">sync</span>
                            Cập nhật: 10 phút trước
                        </div>
                    </motion.div>

                    {/* Stats Cards */}
                    <motion.div variants={item} className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="bento-card p-5 rounded-md border-l-4 border-l-primary">
                            <p className="text-xs text-slate-500 uppercase font-semibold">Tổng số đề tài</p>
                            <div className="mt-2 flex items-baseline gap-2">
                                <span className="text-2xl font-bold text-slate-800">1,245</span>
                                <span className="text-xs font-medium text-green-600 bg-green-50 px-1.5 py-0.5 rounded">+12%</span>
                            </div>
                        </div>
                        <div className="bento-card p-5 rounded-md border-l-4 border-l-success">
                            <p className="text-xs text-slate-500 uppercase font-semibold">Tỷ lệ hoàn thành</p>
                            <div className="mt-2 flex items-baseline gap-2">
                                <span className="text-2xl font-bold text-slate-800">92.5%</span>
                                <span className="text-xs font-medium text-green-600 bg-green-50 px-1.5 py-0.5 rounded">+1.2%</span>
                            </div>
                        </div>
                    </motion.div>

                    {/* Charts Row */}
                    <motion.div variants={item} className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                        {/* Bar Chart */}
                        <div className="lg:col-span-2 bento-card p-6 rounded-md flex flex-col">
                            <div className="flex justify-between items-center mb-6">
                                <div>
                                    <h3 className="text-slate-800 text-lg font-bold">Tăng Trưởng Số Lượng Đề Tài</h3>
                                    <p className="text-sm text-slate-500">So sánh số lượng đề tài đăng ký qua các năm học</p>
                                </div>
                                <div className="flex items-center gap-2">
                                    <span className="w-3 h-3 rounded-full bg-primary"></span>
                                    <span className="text-xs text-slate-600">Đăng ký</span>
                                    <span className="w-3 h-3 rounded-full bg-slate-300 ml-2"></span>
                                    <span className="text-xs text-slate-600">Hoàn thành</span>
                                </div>
                            </div>
                            <div className="flex-1 min-h-[300px] flex items-end justify-between gap-4 px-4 pb-4 border-b border-l border-slate-100 relative">
                                {[
                                    { year: '2019', h1: '40%', h2: '35%', total: 450 },
                                    { year: '2020', h1: '55%', h2: '50%', total: 620 },
                                    { year: '2021', h1: '65%', h2: '60%', total: 780 },
                                    { year: '2022', h1: '80%', h2: '75%', total: 950 },
                                    { year: '2023', h1: '92%', h2: '88%', total: 1245, isCurrent: true },
                                ].map((d) => (
                                    <div key={d.year} className="group flex flex-col items-center gap-2 w-full z-10 cursor-pointer relative">
                                        <div className="w-full flex justify-center gap-1 items-end h-[200px]">
                                            <motion.div
                                                initial={{ height: 0 }}
                                                animate={{ height: d.h1 }}
                                                transition={{ duration: 0.5, delay: 0.2 }}
                                                className={`w-3 ${d.isCurrent ? 'bg-primary shadow-sm shadow-primary/20' : 'bg-primary/40 group-hover:bg-primary'} rounded-t-sm transition-colors`}
                                            />
                                            <motion.div
                                                initial={{ height: 0 }}
                                                animate={{ height: d.h2 }}
                                                transition={{ duration: 0.5, delay: 0.3 }}
                                                className={`w-3 ${d.isCurrent ? 'bg-slate-300' : 'bg-slate-200 group-hover:bg-slate-300'} rounded-t-sm transition-colors`}
                                            />
                                        </div>
                                        <span className={`text-xs font-medium ${d.isCurrent ? 'font-bold text-primary' : 'text-slate-500'}`}>{d.year}</span>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Donut Chart */}
                        <div className="bento-card p-6 rounded-md flex flex-col">
                            <h3 className="text-slate-800 text-lg font-bold mb-4">Tỷ Lệ Hoàn Thành Theo Ngành</h3>
                            <div className="flex-1 flex flex-col items-center justify-center relative">
                                <div
                                    className="relative w-48 h-48 rounded-full shadow-inner"
                                    style={{ background: 'conic-gradient(#2c6090 0% 45%, #5F8F61 45% 70%, #f59e0b 70% 85%, #ef4444 85% 100%)' }}
                                >
                                    <div className="absolute inset-0 m-auto w-32 h-32 bg-white rounded-full flex flex-col items-center justify-center z-10 shadow-sm">
                                        <span className="text-3xl font-bold text-slate-800">92%</span>
                                        <span className="text-[10px] text-slate-400 uppercase tracking-wide">Trung bình</span>
                                    </div>
                                </div>
                                <div className="w-full mt-8 space-y-3">
                                    <LegendItem color="bg-primary" label="Kỹ thuật phần mềm" value="45%" />
                                    <LegendItem color="bg-success" label="Khoa học máy tính" value="25%" />
                                    <LegendItem color="bg-amber-500" label="An toàn thông tin" value="15%" />
                                    <LegendItem color="bg-red-500" label="Khác" value="15%" />
                                </div>
                            </div>
                        </div>
                    </motion.div>

                    {/* Score Distribution & Export History */}
                    <motion.div variants={item} className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                        {/* Score Distribution */}
                        <div className="bento-card p-6 rounded-md">
                            <div className="flex justify-between items-center mb-6">
                                <h3 className="text-slate-800 text-lg font-bold">Phân Bổ Điểm Số Trung Bình</h3>
                                <button className="text-xs text-primary font-medium hover:underline">Chi tiết</button>
                            </div>
                            <div className="relative h-64 flex items-end gap-1 pt-6 pb-2">
                                <div className="absolute top-[30%] left-0 w-full border-t border-dashed border-red-300 z-0 flex items-center">
                                    <span className="bg-red-50 text-red-500 text-[10px] px-1 rounded -mt-2.5">Avg: 7.8</span>
                                </div>
                                {[
                                    { label: '< 5.0', height: '10%', color: 'bg-red-100 group-hover:bg-red-200', pct: '5%' },
                                    { label: '5.0-6.5', height: '25%', color: 'bg-orange-100 group-hover:bg-orange-200', pct: '15%' },
                                    { label: '6.5-8.0', height: '65%', color: 'bg-primary/20 group-hover:bg-primary/30', pct: '45%' },
                                    { label: '8.0-9.0', height: '45%', color: 'bg-primary shadow-md shadow-primary/20', pct: '25%' },
                                    { label: '> 9.0', height: '20%', color: 'bg-green-100 group-hover:bg-green-200', pct: '10%' },
                                ].map((score) => (
                                    <div key={score.label} className="flex-1 flex flex-col items-center gap-1 group relative h-full justify-end">
                                        <span className="text-xs text-slate-500 opacity-0 group-hover:opacity-100 transition-opacity">{score.pct}</span>
                                        <motion.div
                                            initial={{ height: 0 }}
                                            animate={{ height: score.height }}
                                            transition={{ duration: 0.5 }}
                                            className={`w-full max-w-[40px] ${score.color} rounded-t-sm transition-colors`}
                                        />
                                        <span className="text-xs text-slate-600 font-medium">{score.label}</span>
                                    </div>
                                ))}
                            </div>
                        </div>

                        {/* Export History */}
                        <div className="bento-card rounded-md flex flex-col overflow-hidden">
                            <div className="p-6 border-b border-slate-200 flex justify-between items-center bg-white">
                                <h3 className="text-slate-800 text-lg font-bold">Lịch Sử Xuất Báo Cáo</h3>
                                <button className="flex items-center gap-1 text-xs text-primary font-medium border border-primary/20 bg-primary/5 px-2 py-1 rounded hover:bg-primary/10 transition-colors">
                                    <span className="material-symbols-outlined text-[14px]">settings</span>
                                    Cấu hình
                                </button>
                            </div>
                            <div className="overflow-x-auto flex-1">
                                <table className="w-full text-left text-sm text-slate-500">
                                    <thead className="bg-slate-50 text-xs uppercase font-semibold text-slate-600">
                                        <tr>
                                            <th className="px-6 py-3">Tên báo cáo</th>
                                            <th className="px-6 py-3">Ngày tạo</th>
                                            <th className="px-6 py-3 text-right">Tác vụ</th>
                                        </tr>
                                    </thead>
                                    <tbody className="divide-y divide-slate-100">
                                        {[
                                            { name: 'Tổng kết HK1_2023.pdf', type: 'pdf', date: '15/01/2024', tag: 'Định kỳ • Tháng' },
                                            { name: 'Danh_sach_diem_tot_nghiep.xlsx', type: 'excel', date: '14/01/2024', tag: 'Thủ công • Admin' },
                                            { name: 'Bao_cao_hieu_suat_GV.pdf', type: 'pdf', date: '01/01/2024', tag: 'Định kỳ • Quý' },
                                        ].map((report) => (
                                            <tr key={report.name} className="hover:bg-slate-50 transition-colors group">
                                                <td className="px-6 py-3">
                                                    <div className="flex items-center gap-3">
                                                        <div className={`p-2 ${report.type === 'pdf' ? 'bg-red-50 text-red-600' : 'bg-green-50 text-green-600'} rounded`}>
                                                            <span className="material-symbols-outlined text-[18px]">
                                                                {report.type === 'pdf' ? 'picture_as_pdf' : 'table_view'}
                                                            </span>
                                                        </div>
                                                        <div>
                                                            <p className="text-slate-800 font-medium text-xs">{report.name}</p>
                                                            <p className="text-[10px] text-slate-400">{report.tag}</p>
                                                        </div>
                                                    </div>
                                                </td>
                                                <td className="px-6 py-3 text-xs">{report.date}</td>
                                                <td className="px-6 py-3 text-right">
                                                    <button className="text-slate-400 hover:text-primary transition-colors">
                                                        <span className="material-symbols-outlined text-[18px]">download</span>
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}

function LegendItem({ color, label, value }: { color: string; label: string; value: string }) {
    return (
        <div className="flex justify-between items-center text-sm group hover:bg-slate-50 p-1 rounded cursor-default transition-colors">
            <div className="flex items-center gap-2">
                <div className={`w-3 h-3 rounded-full ${color}`} />
                <span className="text-slate-600">{label}</span>
            </div>
            <span className="font-medium text-slate-800">{value}</span>
        </div>
    )
}
