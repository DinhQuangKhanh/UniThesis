import { useEffect, useState } from 'react'
import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'
import { apiClient } from '@/lib/apiClient'
import { NotificationDropdown } from '@/components/layout'

// ─── Types ───────────────────────────────────────────────────────────────────

interface TopicPoolDto {
    id: string
    code: string
    name: string
    description: string | null
    majorId: number
    statusName: string
    maxActiveTopicsPerMentor: number
    expirationSemesters: number
    createdAt: string
    updatedAt: string | null
}

// ─── Animation variants ───────────────────────────────────────────────────────

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.07 } },
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 },
}

// ─── Component ───────────────────────────────────────────────────────────────

export function TopicPoolsPage() {
    const navigate = useNavigate()
    const [pools, setPools] = useState<TopicPoolDto[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [search, setSearch] = useState('')

    useEffect(() => {
        setLoading(true)
        apiClient
            .get<TopicPoolDto[]>('/api/topic-pools')
            .then(setPools)
            .catch((err: Error) => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    const filtered = pools.filter(p =>
        p.name.toLowerCase().includes(search.toLowerCase()) ||
        p.code.toLowerCase().includes(search.toLowerCase())
    )

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-white border-b border-slate-200 flex-shrink-0 z-50 shadow-sm">
                <div className="flex items-center gap-2 text-slate-800">
                    <span className="text-slate-400 font-medium text-sm">Quản lý</span>
                    <span className="material-symbols-outlined text-sm text-slate-400">chevron_right</span>
                    <h2 className="text-lg font-bold">Kho đề tài</h2>
                </div>
                <div className="flex items-center gap-4">
                    <div className="relative hidden md:block">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-slate-400 text-[20px]">search</span>
                        </div>
                        <input
                            type="text"
                            value={search}
                            onChange={e => setSearch(e.target.value)}
                            className="block w-64 pl-10 pr-3 py-2 rounded-lg bg-slate-100 text-slate-900 placeholder-slate-400 focus:outline-none focus:bg-white focus:ring-1 focus:ring-primary sm:text-sm transition-all border-none"
                            placeholder="Tìm kho đề tài..."
                        />
                    </div>
                    <NotificationDropdown />
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-50">
                <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
                    {/* Title */}
                    <motion.div variants={item} className="flex flex-col gap-1">
                        <h1 className="text-2xl font-bold text-slate-900">Kho đề tài theo chuyên ngành</h1>
                        <p className="text-slate-500 text-sm">
                            Mỗi chuyên ngành có một kho đề tài cố định. Sinh viên đăng ký đề tài từ kho để thực hiện đồ án.
                        </p>
                    </motion.div>

                    {/* Error */}
                    {error && (
                        <motion.div variants={item} className="flex items-center gap-3 bg-red-50 border border-red-200 rounded-xl p-4 text-red-700">
                            <span className="material-symbols-outlined">error</span>
                            <p className="text-sm font-medium">{error}</p>
                        </motion.div>
                    )}

                    {/* Loading skeleton */}
                    {loading ? (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                            {[1, 2, 3].map(i => (
                                <div key={i} className="bg-white rounded-xl border border-slate-200 p-5 animate-pulse space-y-4">
                                    <div className="flex items-center gap-3">
                                        <div className="size-10 rounded-lg bg-slate-200" />
                                        <div className="flex-1 space-y-2">
                                            <div className="h-4 bg-slate-200 rounded w-2/3" />
                                            <div className="h-3 bg-slate-200 rounded w-1/3" />
                                        </div>
                                    </div>
                                    <div className="h-3 bg-slate-200 rounded w-full" />
                                    <div className="h-3 bg-slate-200 rounded w-4/5" />
                                </div>
                            ))}
                        </div>
                    ) : filtered.length === 0 ? (
                        <motion.div variants={item} className="flex flex-col items-center justify-center py-20 text-center">
                            <span className="material-symbols-outlined text-5xl text-slate-300">library_books</span>
                            <p className="text-slate-400 font-medium mt-3">
                                {search ? 'Không tìm thấy kho đề tài nào' : 'Chưa có kho đề tài nào'}
                            </p>
                        </motion.div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                            {filtered.map(pool => (
                                <motion.div
                                    key={pool.id}
                                    variants={item}
                                    className="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-md hover:border-primary/30 transition-all duration-200 flex flex-col overflow-hidden cursor-pointer group"
                                    onClick={() => navigate(`/mentor/topic-pools/${pool.id}`)}
                                >
                                    <div className="p-5 flex-1">
                                        {/* Pool header */}
                                        <div className="flex items-start gap-3 mb-4">
                                            <div className="size-10 rounded-lg bg-primary/10 text-primary flex items-center justify-center flex-shrink-0">
                                                <span className="material-symbols-outlined">library_books</span>
                                            </div>
                                            <div className="flex-1 min-w-0">
                                                <h3 className="text-slate-900 font-bold text-sm leading-snug group-hover:text-primary transition-colors">
                                                    {pool.name}
                                                </h3>
                                                <p className="text-xs font-mono text-slate-400 mt-0.5">{pool.code}</p>
                                            </div>
                                            <span className={`px-2 py-0.5 rounded-full text-xs font-bold border shrink-0 ${pool.statusName === 'Active'
                                                ? 'bg-green-50 text-green-700 border-green-100'
                                                : 'bg-amber-50 text-amber-700 border-amber-100'}`}>
                                                {pool.statusName === 'Active' ? 'Đang mở' : 'Tạm dừng'}
                                            </span>
                                        </div>

                                        {/* Description */}
                                        {pool.description && (
                                            <p className="text-sm text-slate-500 line-clamp-2 mb-4">{pool.description}</p>
                                        )}

                                        {/* Metadata */}
                                        <div className="grid grid-cols-2 gap-3">
                                            <div className="flex items-center gap-2 text-xs text-slate-500">
                                                <span className="material-symbols-outlined text-[16px] text-slate-400">person</span>
                                                <span>Tối đa <strong className="text-slate-700">{pool.maxActiveTopicsPerMentor}</strong> đề tài/GV</span>
                                            </div>
                                            <div className="flex items-center gap-2 text-xs text-slate-500">
                                                <span className="material-symbols-outlined text-[16px] text-slate-400">schedule</span>
                                                <span>Hết hạn sau <strong className="text-slate-700">{pool.expirationSemesters}</strong> HK</span>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Footer */}
                                    <div className="px-5 py-3 bg-slate-50 border-t border-slate-100 flex items-center justify-between">
                                        <span className="text-xs text-slate-400">
                                            Mã chuyên ngành #{pool.majorId}
                                        </span>
                                        <button className="text-primary text-xs font-semibold flex items-center gap-1 group-hover:gap-2 transition-all">
                                            Xem chi tiết
                                            <span className="material-symbols-outlined text-[14px]">arrow_forward</span>
                                        </button>
                                    </div>
                                </motion.div>
                            ))}
                        </div>
                    )}
                </motion.div>
            </div>
        </>
    )
}
