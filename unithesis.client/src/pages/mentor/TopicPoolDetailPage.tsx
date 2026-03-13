import { useEffect, useState } from 'react'
import { motion } from 'framer-motion'
import { useParams, useNavigate } from 'react-router-dom'
import { apiClient } from '@/lib/apiClient'
import { useAuth } from '@/contexts/AuthContext'
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
}

interface TopicPoolStatisticsDto {
    poolId: string
    poolCode: string
    poolName: string
    totalMentors: number
    totalTopicsCount: number
    activeTopicsCount: number
    registeredTopicsCount: number
    expiredTopicsCount: number
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

export function TopicPoolDetailPage() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { user } = useAuth()

    const [pool, setPool] = useState<TopicPoolDto | null>(null)
    const [stats, setStats] = useState<TopicPoolStatisticsDto | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    // Registration state
    const [registering, setRegistering] = useState<string | null>(null) // projectId being registered
    const [registerSuccess, setRegisterSuccess] = useState<string | null>(null)
    const [registerError, setRegisterError] = useState<string | null>(null)

    useEffect(() => {
        if (!id) return
        setLoading(true)
        Promise.all([
            apiClient.get<TopicPoolDto>(`/api/topic-pools/${id}`),
            apiClient.get<TopicPoolStatisticsDto>(`/api/topic-pools/${id}/statistics`),
        ])
            .then(([poolData, statsData]) => {
                setPool(poolData)
                setStats(statsData)
            })
            .catch((err: Error) => setError(err.message))
            .finally(() => setLoading(false))
    }, [id])

    const handleRegister = async (projectId: string) => {
        if (!user?.id) return
        setRegistering(projectId)
        setRegisterError(null)
        setRegisterSuccess(null)
        try {
            await apiClient.post('/api/topic-pools/registrations', {
                projectId,
                groupId: user.id, // In real use, groupId should come from user's group
                note: null,
            })
            setRegisterSuccess(projectId)
        } catch (err: unknown) {
            setRegisterError(err instanceof Error ? err.message : 'Đăng ký thất bại')
        } finally {
            setRegistering(null)
        }
    }

    const statCards = stats
        ? [
            { label: 'Tổng đề tài', value: stats.totalTopicsCount, icon: 'description', color: 'blue' },
            { label: 'Còn trống', value: stats.activeTopicsCount, icon: 'inventory_2', color: 'green' },
            { label: 'Đã đăng ký', value: stats.registeredTopicsCount, icon: 'assignment_turned_in', color: 'amber' },
            { label: 'Đã hết hạn', value: stats.expiredTopicsCount, icon: 'archive', color: 'red' },
        ]
        : []

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-white border-b border-slate-200 flex-shrink-0 z-50 shadow-sm">
                <div className="flex items-center gap-2 text-slate-800">
                    <button
                        onClick={() => navigate('/mentor/topic-pools')}
                        className="text-slate-400 hover:text-primary transition-colors"
                    >
                        <span className="material-symbols-outlined">arrow_back</span>
                    </button>
                    <span className="text-slate-300 text-xl font-light">|</span>
                    <span className="text-slate-400 font-medium text-sm">Kho đề tài</span>
                    <span className="material-symbols-outlined text-sm text-slate-400">chevron_right</span>
                    <h2 className="text-lg font-bold truncate max-w-xs">
                        {loading ? 'Đang tải...' : (pool?.name ?? 'Chi tiết kho')}
                    </h2>
                </div>
                <NotificationDropdown />
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-50">
                {/* Error state */}
                {error ? (
                    <div className="flex flex-col items-center justify-center py-20 text-center gap-3">
                        <span className="material-symbols-outlined text-4xl text-red-400">error</span>
                        <p className="text-slate-500 font-medium">{error}</p>
                        <button
                            onClick={() => navigate('/mentor/topic-pools')}
                            className="text-primary text-sm font-semibold hover:underline"
                        >
                            Quay lại danh sách
                        </button>
                    </div>
                ) : (
                    <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">

                        {/* Pool info banner */}
                        {pool && (
                            <motion.div variants={item} className="bg-white rounded-xl border border-slate-200 p-6 flex flex-col md:flex-row items-start md:items-center justify-between gap-4 shadow-sm">
                                <div className="flex items-center gap-4">
                                    <div className="size-14 rounded-xl bg-primary/10 text-primary flex items-center justify-center flex-shrink-0">
                                        <span className="material-symbols-outlined text-2xl">library_books</span>
                                    </div>
                                    <div>
                                        <div className="flex items-center gap-2 mb-1">
                                            <h1 className="text-xl font-bold text-slate-900">{pool.name}</h1>
                                            <span className={`px-2 py-0.5 rounded-full text-xs font-bold border ${pool.statusName === 'Active'
                                                ? 'bg-green-50 text-green-700 border-green-100'
                                                : 'bg-amber-50 text-amber-700 border-amber-100'}`}>
                                                {pool.statusName === 'Active' ? 'Đang mở' : 'Tạm dừng'}
                                            </span>
                                        </div>
                                        <div className="flex items-center gap-4 text-sm text-slate-500">
                                            <span className="font-mono text-xs bg-slate-100 px-2 py-0.5 rounded">{pool.code}</span>
                                            {pool.description && <span className="line-clamp-1 max-w-sm">{pool.description}</span>}
                                        </div>
                                    </div>
                                </div>
                                <div className="flex gap-6 text-center">
                                    <div>
                                        <p className="text-xs text-slate-400 mb-0.5">Tối đa / GV</p>
                                        <p className="text-lg font-bold text-slate-800">{pool.maxActiveTopicsPerMentor}</p>
                                    </div>
                                    <div>
                                        <p className="text-xs text-slate-400 mb-0.5">Hết hạn (HK)</p>
                                        <p className="text-lg font-bold text-slate-800">{pool.expirationSemesters}</p>
                                    </div>
                                </div>
                            </motion.div>
                        )}

                        {/* Statistics */}
                        {loading ? (
                            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                                {[1, 2, 3, 4].map(i => (
                                    <div key={i} className="bg-white rounded-xl border border-slate-200 p-5 animate-pulse">
                                        <div className="h-3 bg-slate-200 rounded w-1/2 mb-3" />
                                        <div className="h-8 bg-slate-200 rounded w-1/3" />
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                                {statCards.map(s => (
                                    <motion.div key={s.label} variants={item} className="bg-white rounded-xl border border-slate-200 p-5 shadow-sm">
                                        <div className="flex items-center justify-between mb-3">
                                            <p className="text-sm text-slate-500 font-medium">{s.label}</p>
                                            <div className={`size-8 rounded-lg bg-${s.color}-50 text-${s.color}-600 flex items-center justify-center`}>
                                                <span className="material-symbols-outlined text-[18px]">{s.icon}</span>
                                            </div>
                                        </div>
                                        <p className="text-3xl font-bold text-slate-900">{s.value}</p>
                                    </motion.div>
                                ))}
                            </div>
                        )}

                        {/* Registration feedback */}
                        {registerSuccess && (
                            <motion.div
                                initial={{ opacity: 0, y: -10 }}
                                animate={{ opacity: 1, y: 0 }}
                                className="flex items-center gap-3 bg-green-50 border border-green-200 rounded-xl p-4 text-green-700"
                            >
                                <span className="material-symbols-outlined">check_circle</span>
                                <p className="text-sm font-medium">Đăng ký thành công! Chờ xác nhận từ quản trị viên.</p>
                            </motion.div>
                        )}
                        {registerError && (
                            <motion.div
                                initial={{ opacity: 0, y: -10 }}
                                animate={{ opacity: 1, y: 0 }}
                                className="flex items-center gap-3 bg-red-50 border border-red-200 rounded-xl p-4 text-red-700"
                            >
                                <span className="material-symbols-outlined">error</span>
                                <p className="text-sm font-medium">{registerError}</p>
                            </motion.div>
                        )}

                        {/* Info panel - for mentors, show pool rules */}
                        {!loading && pool && (
                            <motion.div variants={item} className="bg-blue-50 border border-blue-100 rounded-xl p-5 flex items-start gap-3">
                                <span className="material-symbols-outlined text-blue-500 mt-0.5">info</span>
                                <div className="text-sm text-blue-800 flex-1">
                                    {user?.role === 'mentor' ? (
                                        <>
                                            <p className="font-semibold mb-1">Quy định kho đề tài</p>
                                            <ul className="list-disc list-inside space-y-1 text-blue-700 text-xs">
                                                <li>Mỗi giảng viên tối đa <strong>{pool.maxActiveTopicsPerMentor}</strong> đề tài đang hoạt động trong kho này.</li>
                                                <li>Đề tài chưa được đăng ký sau <strong>{pool.expirationSemesters}</strong> học kỳ sẽ tự động hết hạn.</li>
                                                <li>Đề tài đề xuất vào kho cần được phê duyệt trước khi sinh viên có thể đăng ký.</li>
                                            </ul>
                                        </>
                                    ) : (
                                        <>
                                            <p className="font-semibold mb-1">Hướng dẫn đăng ký đề tài</p>
                                            <ul className="list-disc list-inside space-y-1 text-blue-700 text-xs">
                                                <li>Chọn đề tài phù hợp và nhấn "Đăng ký" để gửi yêu cầu.</li>
                                                <li>Yêu cầu sẽ được xem xét và phê duyệt bởi ban quản lý.</li>
                                                <li>Mỗi nhóm chỉ có thể đăng ký một đề tài tại một thời điểm.</li>
                                            </ul>
                                            {pool.id && (
                                                <button
                                                    onClick={() => handleRegister(pool.id)}
                                                    disabled={registering === pool.id || !!registerSuccess}
                                                    className="mt-3 inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-white text-xs font-semibold hover:bg-primary/90 transition-all disabled:opacity-60 disabled:cursor-not-allowed shadow-sm"
                                                >
                                                    {registering === pool.id ? (
                                                        <><span className="material-symbols-outlined text-[16px] animate-spin">progress_activity</span>Đang xử lý...</>
                                                    ) : (
                                                        <><span className="material-symbols-outlined text-[16px]">how_to_reg</span>Đăng ký đề tài
                                                        </>
                                                    )}
                                                </button>
                                            )}
                                        </>
                                    )}
                                </div>
                            </motion.div>
                        )}
                    </motion.div>
                )}
            </div>
        </>
    )
}
