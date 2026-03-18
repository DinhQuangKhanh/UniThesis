import { useEffect, useState } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
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

interface PoolTopicItemDto {
    id: string
    code: string
    nameVi: string
    nameEn: string
    description: string | null
    technologies: string | null
    majorId: number
    majorName: string
    majorCode: string
    poolStatus: number
    poolStatusName: string
    maxStudents: number
    mentorName: string
    mentorId: string
    createdAt: string
}

interface GetPoolTopicsResult {
    items: PoolTopicItemDto[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
}

interface MentorSummaryDto {
    mentorId: string
    fullName: string
}

interface PoolTopicDetailDto {
    id: string
    code: string
    nameVi: string
    nameEn: string
    nameAbbr: string
    description: string
    objectives: string
    scope: string | null
    technologies: string | null
    expectedResults: string | null
    majorId: number
    majorName: string
    majorCode: string
    poolStatus: number
    poolStatusName: string
    maxStudents: number
    mentors: MentorSummaryDto[]
    createdAt: string
    updatedAt: string | null
}

// ─── Helpers ─────────────────────────────────────────────────────────────────

const POOL_STATUS_LABELS: Record<number, { label: string; cls: string }> = {
    0: { label: 'Còn trống', cls: 'bg-green-50 text-green-700 border-green-100' },
    1: { label: 'Đã giữ chỗ', cls: 'bg-amber-50 text-amber-700 border-amber-100' },
    2: { label: 'Đã giao', cls: 'bg-blue-50 text-blue-700 border-blue-100' },
    3: { label: 'Hết hạn', cls: 'bg-red-50 text-red-700 border-red-100' },
}

const container = { hidden: { opacity: 0 }, show: { opacity: 1, transition: { staggerChildren: 0.06 } } }
const item = { hidden: { opacity: 0, y: 16 }, show: { opacity: 1, y: 0 } }

// ─── ProposeTopicModal ────────────────────────────────────────────────────────

interface ProposeTopicModalProps {
    poolId: string
    onClose: () => void
    onSuccess: () => void
}

function ProposeTopicModal({ poolId, onClose, onSuccess }: ProposeTopicModalProps) {
    const [form, setForm] = useState({
        nameVi: '', nameEn: '', nameAbbr: '', description: '',
        objectives: '', scope: '', technologies: '', expectedResults: '', maxStudents: 5,
    })
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const set = (field: string, val: string | number) =>
        setForm(prev => ({ ...prev, [field]: val }))

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setSubmitting(true)
        setError(null)
        try {
            await apiClient.post(`/api/topic-pools/${poolId}/propose`, {
                nameVi: form.nameVi, nameEn: form.nameEn, nameAbbr: form.nameAbbr,
                description: form.description, objectives: form.objectives,
                scope: form.scope || null, technologies: form.technologies || null,
                expectedResults: form.expectedResults || null, maxStudents: form.maxStudents,
            })
            onSuccess()
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Đề xuất thất bại')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
            <motion.div
                initial={{ opacity: 0, scale: 0.95 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.95 }}
                className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto"
            >
                <div className="sticky top-0 bg-white border-b border-slate-100 px-6 py-4 flex items-center justify-between z-10">
                    <div className="flex items-center gap-3">
                        <div className="size-9 rounded-lg bg-primary/10 text-primary flex items-center justify-center">
                            <span className="material-symbols-outlined text-[18px]">add_circle</span>
                        </div>
                        <div>
                            <h2 className="font-bold text-slate-900 text-base">Đề xuất đề tài mới</h2>
                            <p className="text-xs text-slate-400">Đề tài sẽ được lưu vào kho ngay sau khi gửi</p>
                        </div>
                    </div>
                    <button onClick={onClose} className="text-slate-400 hover:text-slate-600 transition-colors">
                        <span className="material-symbols-outlined">close</span>
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    {error && (
                        <div className="flex items-center gap-2 bg-red-50 border border-red-200 rounded-lg p-3 text-red-700 text-sm">
                            <span className="material-symbols-outlined text-[18px]">error</span>
                            {error}
                        </div>
                    )}

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="md:col-span-2">
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Tên đề tài (Tiếng Việt) *</label>
                            <input required value={form.nameVi} onChange={e => set('nameVi', e.target.value)}
                                className="input-field w-full" placeholder="Ví dụ: Hệ thống quản lý luận văn tốt nghiệp" />
                        </div>
                        <div className="md:col-span-2">
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Tên đề tài (Tiếng Anh) *</label>
                            <input required value={form.nameEn} onChange={e => set('nameEn', e.target.value)}
                                className="input-field w-full" placeholder="E.g., Thesis Management System" />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Tên viết tắt *</label>
                            <input required value={form.nameAbbr} onChange={e => set('nameAbbr', e.target.value)}
                                className="input-field w-full" placeholder="Ví dụ: TMS" />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Số sinh viên tối đa *</label>
                            <input type="number" min={1} max={5} required value={form.maxStudents}
                                onChange={e => set('maxStudents', parseInt(e.target.value))}
                                className="input-field w-full" />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-slate-600 mb-1">Mô tả *</label>
                        <textarea required rows={3} value={form.description} onChange={e => set('description', e.target.value)}
                            className="input-field w-full resize-none" placeholder="Mô tả tổng quan về đề tài..." />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-slate-600 mb-1">Mục tiêu *</label>
                        <textarea required rows={3} value={form.objectives} onChange={e => set('objectives', e.target.value)}
                            className="input-field w-full resize-none" placeholder="Mục tiêu cần đạt được..." />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Phạm vi (tùy chọn)</label>
                            <textarea rows={2} value={form.scope} onChange={e => set('scope', e.target.value)}
                                className="input-field w-full resize-none" placeholder="Phạm vi nghiên cứu..." />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-slate-600 mb-1">Công nghệ (tùy chọn)</label>
                            <textarea rows={2} value={form.technologies} onChange={e => set('technologies', e.target.value)}
                                className="input-field w-full resize-none" placeholder="React, .NET, SQL Server..." />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-slate-600 mb-1">Kết quả dự kiến (tùy chọn)</label>
                        <textarea rows={2} value={form.expectedResults} onChange={e => set('expectedResults', e.target.value)}
                            className="input-field w-full resize-none" placeholder="Sản phẩm, báo cáo..." />
                    </div>

                    <div className="flex justify-end gap-3 pt-2">
                        <button type="button" onClick={onClose}
                            className="px-4 py-2 rounded-lg border border-slate-200 text-slate-600 text-sm hover:bg-slate-50 transition-colors">
                            Hủy
                        </button>
                        <button type="submit" disabled={submitting}
                            className="inline-flex items-center gap-2 px-5 py-2 rounded-lg bg-primary text-white text-sm font-semibold hover:bg-primary/90 transition-all disabled:opacity-60">
                            {submitting
                                ? <><span className="material-symbols-outlined text-[16px] animate-spin">progress_activity</span>Đang gửi...</>
                                : <><span className="material-symbols-outlined text-[16px]">send</span>Gửi đề xuất</>
                            }
                        </button>
                    </div>
                </form>
            </motion.div>
        </div>
    )
}

// ─── TopicDetailModal ─────────────────────────────────────────────────────────

interface TopicDetailModalProps {
    topicId: string
    onClose: () => void
}

function TopicDetailModal({ topicId, onClose }: TopicDetailModalProps) {
    const [detail, setDetail] = useState<PoolTopicDetailDto | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        apiClient.get<PoolTopicDetailDto>(`/api/topic-pools/topics/${topicId}`)
            .then(setDetail)
            .catch((err: Error) => setError(err.message))
            .finally(() => setLoading(false))
    }, [topicId])

    const stat = detail ? POOL_STATUS_LABELS[detail.poolStatus] : null

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
            <motion.div
                initial={{ opacity: 0, scale: 0.95 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.95 }}
                className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto"
            >
                {/* Header */}
                <div className="sticky top-0 bg-white border-b border-slate-100 px-6 py-4 flex items-center justify-between z-10">
                    <div className="flex items-center gap-3">
                        <div className="size-9 rounded-lg bg-primary/10 text-primary flex items-center justify-center">
                            <span className="material-symbols-outlined text-[18px]">description</span>
                        </div>
                        <div>
                            <h2 className="font-bold text-slate-900 text-base">
                                {loading ? 'Đang tải...' : detail?.nameVi ?? 'Chi tiết đề tài'}
                            </h2>
                            {detail && <p className="text-xs font-mono text-slate-400">{detail.code}</p>}
                        </div>
                    </div>
                    <button onClick={onClose} className="text-slate-400 hover:text-slate-600 transition-colors">
                        <span className="material-symbols-outlined">close</span>
                    </button>
                </div>

                <div className="p-6 space-y-5">
                    {loading && (
                        <div className="space-y-4 animate-pulse">
                            <div className="h-4 bg-slate-200 rounded w-3/4" />
                            <div className="h-4 bg-slate-200 rounded w-1/2" />
                            <div className="h-20 bg-slate-100 rounded-lg" />
                        </div>
                    )}
                    {error && (
                        <div className="flex items-center gap-2 bg-red-50 border border-red-200 rounded-lg p-3 text-red-700 text-sm">
                            <span className="material-symbols-outlined text-[18px]">error</span>
                            {error}
                        </div>
                    )}
                    {detail && (
                        <>
                            {/* Subtitle (EN) + status */}
                            <div className="flex items-center gap-2 flex-wrap">
                                <p className="text-sm text-slate-500 italic">{detail.nameEn}</p>
                                {stat && (
                                    <span className={`px-2 py-0.5 rounded-full text-xs font-bold border ${stat.cls}`}>
                                        {stat.label}
                                    </span>
                                )}
                            </div>

                            {/* Grid info */}
                            <div className="grid grid-cols-2 gap-3 text-sm">
                                <div className="flex items-start gap-2">
                                    <span className="material-symbols-outlined text-slate-400 text-[16px] mt-0.5">school</span>
                                    <div>
                                        <p className="text-xs text-slate-400">Chuyên ngành</p>
                                        <p className="font-medium text-slate-800">{detail.majorName}</p>
                                    </div>
                                </div>
                                <div className="flex items-start gap-2">
                                    <span className="material-symbols-outlined text-slate-400 text-[16px] mt-0.5">group</span>
                                    <div>
                                        <p className="text-xs text-slate-400">Tối đa sinh viên</p>
                                        <p className="font-medium text-slate-800">{detail.maxStudents} SV</p>
                                    </div>
                                </div>
                            </div>

                            {/* Sections */}
                            {[
                                { label: 'Mô tả', value: detail.description, icon: 'info' },
                                { label: 'Mục tiêu', value: detail.objectives, icon: 'flag' },
                                { label: 'Phạm vi', value: detail.scope, icon: 'explore' },
                                { label: 'Kết quả dự kiến', value: detail.expectedResults, icon: 'verified' },
                            ].map(s => s.value && (
                                <div key={s.label}>
                                    <p className="flex items-center gap-1.5 text-xs font-semibold text-slate-500 mb-1.5">
                                        <span className="material-symbols-outlined text-[14px]">{s.icon}</span>
                                        {s.label}
                                    </p>
                                    <p className="text-sm text-slate-700 leading-relaxed bg-slate-50 rounded-lg p-3">{s.value}</p>
                                </div>
                            ))}

                            {detail.technologies && (
                                <div>
                                    <p className="flex items-center gap-1.5 text-xs font-semibold text-slate-500 mb-1.5">
                                        <span className="material-symbols-outlined text-[14px]">code</span>
                                        Công nghệ
                                    </p>
                                    <div className="flex flex-wrap gap-2">
                                        {detail.technologies.split(',').map(t => (
                                            <span key={t} className="px-2.5 py-1 bg-primary/10 text-primary rounded-full text-xs font-medium">
                                                {t.trim()}
                                            </span>
                                        ))}
                                    </div>
                                </div>
                            )}

                            {/* Mentors */}
                            {detail.mentors.length > 0 && (
                                <div>
                                    <p className="flex items-center gap-1.5 text-xs font-semibold text-slate-500 mb-2">
                                        <span className="material-symbols-outlined text-[14px]">person</span>
                                        Giảng viên hướng dẫn
                                    </p>
                                    <div className="flex flex-col gap-2">
                                        {detail.mentors.map(m => (
                                            <div key={m.mentorId} className="flex items-center gap-2 text-sm text-slate-700">
                                                <div className="size-8 rounded-full bg-primary/10 text-primary flex items-center justify-center text-xs font-bold">
                                                    {m.fullName.charAt(0)}
                                                </div>
                                                {m.fullName}
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </motion.div>
        </div>
    )
}

// ─── TopicPoolDetailPage ──────────────────────────────────────────────────────

export function TopicPoolDetailPage() {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { user } = useAuth()

    const [pool, setPool] = useState<TopicPoolDto | null>(null)
    const [stats, setStats] = useState<TopicPoolStatisticsDto | null>(null)
    const [topics, setTopics] = useState<PoolTopicItemDto[]>([])
    const [topicsMeta, setTopicsMeta] = useState({ totalCount: 0, page: 1, totalPages: 1 })
    const [loading, setLoading] = useState(true)
    const [topicsLoading, setTopicsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)

    // Filters
    const [search, setSearch] = useState('')
    const [statusFilter, setStatusFilter] = useState<string>('')
    const [sortBy, setSortBy] = useState<string>('newest')
    const [page, setPage] = useState(1)

    // Modals
    const [selectedTopicId, setSelectedTopicId] = useState<string | null>(null)
    const [showProposeModal, setShowProposeModal] = useState(false)
    const [proposeSuccess, setProposeSuccess] = useState(false)

    // Load pool + stats once
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

    // Load topics whenever filter/page changes
    useEffect(() => {
        if (!pool) return
        setTopicsLoading(true)
        const params = new URLSearchParams({
            MajorId: pool.majorId.toString(),
            Page: page.toString(),
            PageSize: '10',
            SortBy: sortBy,
        })
        if (search.trim()) params.set('Search', search.trim())
        if (statusFilter) params.set('PoolStatus', statusFilter)

        apiClient.get<GetPoolTopicsResult>(`/api/topic-pools/topics?${params}`)
            .then(data => {
                setTopics(data.items)
                setTopicsMeta({ totalCount: data.totalCount, page: data.page, totalPages: data.totalPages })
            })
            .catch(() => { /* keep previous list */ })
            .finally(() => setTopicsLoading(false))
    }, [pool, page, search, statusFilter, sortBy])

    const statCards = stats
        ? [
            { label: 'Tổng đề tài', value: stats.totalTopicsCount, icon: 'description', color: 'blue' },
            { label: 'Còn trống', value: stats.activeTopicsCount, icon: 'inventory_2', color: 'green' },
            { label: 'Đã đăng ký', value: stats.registeredTopicsCount, icon: 'assignment_turned_in', color: 'amber' },
            { label: 'Đã hết hạn', value: stats.expiredTopicsCount, icon: 'archive', color: 'red' },
        ] : []

    const isMentor = user?.role === 'mentor'

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-white border-b border-slate-200 flex-shrink-0 z-50 shadow-sm">
                <div className="flex items-center gap-2 text-slate-800">
                    <button onClick={() => navigate('/mentor/topic-pools')} className="text-slate-400 hover:text-primary transition-colors">
                        <span className="material-symbols-outlined">arrow_back</span>
                    </button>
                    <span className="text-slate-300 text-xl font-light">|</span>
                    <span className="text-slate-400 font-medium text-sm">Kho đề tài</span>
                    <span className="material-symbols-outlined text-sm text-slate-400">chevron_right</span>
                    <h2 className="text-lg font-bold truncate max-w-xs">
                        {loading ? 'Đang tải...' : (pool?.name ?? 'Chi tiết kho')}
                    </h2>
                </div>
                <div className="flex items-center gap-3">
                    {isMentor && pool && (
                        <button
                            onClick={() => setShowProposeModal(true)}
                            className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary text-white text-sm font-semibold hover:bg-primary/90 transition-all shadow-sm"
                        >
                            <span className="material-symbols-outlined text-[16px]">add_circle</span>
                            Đề xuất đề tài mới
                        </button>
                    )}
                    <NotificationDropdown />
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-50">
                {error ? (
                    <div className="flex flex-col items-center justify-center py-20 text-center gap-3">
                        <span className="material-symbols-outlined text-4xl text-red-400">error</span>
                        <p className="text-slate-500 font-medium">{error}</p>
                        <button onClick={() => navigate('/mentor/topic-pools')} className="text-primary text-sm font-semibold hover:underline">
                            Quay lại danh sách
                        </button>
                    </div>
                ) : (
                    <motion.div variants={container} initial="hidden" animate="show" className="space-y-6">

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
                                            <span className={`px-2 py-0.5 rounded-full text-xs font-bold border ${pool.statusName === 'Active' ? 'bg-green-50 text-green-700 border-green-100' : 'bg-amber-50 text-amber-700 border-amber-100'}`}>
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

                        {/* Propose success banner */}
                        <AnimatePresence>
                            {proposeSuccess && (
                                <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0 }}
                                    className="flex items-center gap-3 bg-green-50 border border-green-200 rounded-xl p-4 text-green-700">
                                    <span className="material-symbols-outlined">check_circle</span>
                                    <p className="text-sm font-medium">Đề xuất đề tài thành công! Đề tài đã được thêm vào kho.</p>
                                    <button onClick={() => setProposeSuccess(false)} className="ml-auto text-green-500 hover:text-green-700">
                                        <span className="material-symbols-outlined text-[18px]">close</span>
                                    </button>
                                </motion.div>
                            )}
                        </AnimatePresence>

                        {/* ── Topics section ─────────────────────────────────────────────────── */}
                        <motion.div variants={item} className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
                            {/* Section header + filters */}
                            <div className="px-6 py-4 border-b border-slate-100 flex flex-col md:flex-row md:items-center gap-3">
                                <div className="flex items-center gap-2 flex-1">
                                    <span className="material-symbols-outlined text-primary text-[20px]">list_alt</span>
                                    <h2 className="font-bold text-slate-800">Danh sách đề tài</h2>
                                    <span className="text-xs text-slate-400 font-medium ml-1">({topicsMeta.totalCount})</span>
                                </div>
                                <div className="flex items-center gap-2 flex-wrap">
                                    {/* Search */}
                                    <div className="relative">
                                        <span className="material-symbols-outlined absolute left-2.5 top-1/2 -translate-y-1/2 text-slate-400 text-[16px]">search</span>
                                        <input value={search} onChange={e => { setSearch(e.target.value); setPage(1) }}
                                            className="pl-8 pr-3 py-1.5 rounded-lg bg-slate-100 text-sm text-slate-800 border-none focus:outline-none focus:ring-1 focus:ring-primary w-48"
                                            placeholder="Tìm đề tài..." />
                                    </div>
                                    {/* Status filter */}
                                    <select value={statusFilter} onChange={e => { setStatusFilter(e.target.value); setPage(1) }}
                                        className="py-1.5 pl-2 pr-6 rounded-lg bg-slate-100 text-sm text-slate-700 border-none focus:outline-none">
                                        <option value="">Tất cả trạng thái</option>
                                        <option value="0">Còn trống</option>
                                        <option value="1">Đã giữ chỗ</option>
                                        <option value="2">Đã giao</option>
                                        <option value="3">Hết hạn</option>
                                    </select>
                                    {/* Sort */}
                                    <select value={sortBy} onChange={e => setSortBy(e.target.value)}
                                        className="py-1.5 pl-2 pr-6 rounded-lg bg-slate-100 text-sm text-slate-700 border-none focus:outline-none">
                                        <option value="newest">Mới nhất</option>
                                        <option value="name">Tên</option>
                                        <option value="mentor">Giảng viên</option>
                                    </select>
                                </div>
                            </div>

                            {/* Topics list */}
                            {topicsLoading ? (
                                <div className="divide-y divide-slate-100">
                                    {[1, 2, 3, 4].map(i => (
                                        <div key={i} className="px-6 py-4 animate-pulse flex items-center gap-4">
                                            <div className="size-8 flex-shrink-0 rounded-lg bg-slate-200" />
                                            <div className="flex-1 space-y-2">
                                                <div className="h-4 bg-slate-200 rounded w-2/3" />
                                                <div className="h-3 bg-slate-100 rounded w-1/3" />
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            ) : topics.length === 0 ? (
                                <div className="flex flex-col items-center justify-center py-16 text-center">
                                    <span className="material-symbols-outlined text-4xl text-slate-300">description</span>
                                    <p className="text-slate-400 font-medium mt-3 text-sm">Chưa có đề tài nào trong kho này</p>
                                    {isMentor && (
                                        <button onClick={() => setShowProposeModal(true)}
                                            className="mt-4 inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-primary/10 text-primary text-sm font-semibold hover:bg-primary/20 transition-all">
                                            <span className="material-symbols-outlined text-[16px]">add_circle</span>
                                            Đề xuất đề tài đầu tiên
                                        </button>
                                    )}
                                </div>
                            ) : (
                                <>
                                    <div className="divide-y divide-slate-100">
                                        {topics.map(t => {
                                            const st = POOL_STATUS_LABELS[t.poolStatus] ?? POOL_STATUS_LABELS[0]
                                            return (
                                                <div key={t.id} className="px-6 py-4 hover:bg-slate-50 transition-colors flex items-center gap-4 group">
                                                    <div className="size-9 flex-shrink-0 rounded-lg bg-primary/10 text-primary flex items-center justify-center">
                                                        <span className="material-symbols-outlined text-[18px]">description</span>
                                                    </div>
                                                    <div className="flex-1 min-w-0">
                                                        <p className="font-semibold text-sm text-slate-800 truncate">{t.nameVi}</p>
                                                        <div className="flex items-center gap-3 mt-0.5 text-xs text-slate-400">
                                                            <span className="font-mono">{t.code}</span>
                                                            <span className="flex items-center gap-1">
                                                                <span className="material-symbols-outlined text-[12px]">person</span>
                                                                {t.mentorName}
                                                            </span>
                                                            <span className="flex items-center gap-1">
                                                                <span className="material-symbols-outlined text-[12px]">group</span>
                                                                {t.maxStudents} SV
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div className="flex items-center gap-3 flex-shrink-0">
                                                        <span className={`px-2 py-0.5 rounded-full text-xs font-bold border ${st.cls}`}>
                                                            {st.label}
                                                        </span>
                                                        <button
                                                            onClick={() => setSelectedTopicId(t.id)}
                                                            className="text-primary text-xs font-semibold opacity-0 group-hover:opacity-100 transition-opacity flex items-center gap-1"
                                                        >
                                                            Xem chi tiết
                                                            <span className="material-symbols-outlined text-[13px]">arrow_forward</span>
                                                        </button>
                                                    </div>
                                                </div>
                                            )
                                        })}
                                    </div>

                                    {/* Pagination */}
                                    {topicsMeta.totalPages > 1 && (
                                        <div className="px-6 py-4 border-t border-slate-100 flex items-center justify-between text-sm text-slate-500">
                                            <span>Hiển thị {topics.length}/{topicsMeta.totalCount} đề tài</span>
                                            <div className="flex items-center gap-2">
                                                <button
                                                    disabled={page === 1}
                                                    onClick={() => setPage(p => p - 1)}
                                                    className="px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors text-xs"
                                                >
                                                    Trước
                                                </button>
                                                <span className="font-medium">{page} / {topicsMeta.totalPages}</span>
                                                <button
                                                    disabled={page >= topicsMeta.totalPages}
                                                    onClick={() => setPage(p => p + 1)}
                                                    className="px-3 py-1.5 rounded-lg border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors text-xs"
                                                >
                                                    Sau
                                                </button>
                                            </div>
                                        </div>
                                    )}
                                </>
                            )}
                        </motion.div>
                    </motion.div>
                )}
            </div>

            {/* Modals */}
            <AnimatePresence>
                {selectedTopicId && (
                    <TopicDetailModal
                        key="detail"
                        topicId={selectedTopicId}
                        onClose={() => setSelectedTopicId(null)}
                    />
                )}
                {showProposeModal && pool && (
                    <ProposeTopicModal
                        key="propose"
                        poolId={pool.id}
                        onClose={() => setShowProposeModal(false)}
                        onSuccess={() => {
                            setShowProposeModal(false)
                            setProposeSuccess(true)
                            setPage(1) // Refresh topic list
                        }}
                    />
                )}
            </AnimatePresence>
        </>
    )
}
