import { useState, useEffect, useCallback } from 'react'
import { motion } from 'framer-motion'
import { Header } from '@/components/layout'
import { CreateSemesterModal } from '@/components/admin/CreateSemesterModal'
import { apiClient } from '@/lib/apiClient'

// ---- Types matching backend SemesterDto ----
interface SemesterPhaseDto {
    id: number
    name: string
    type: string
    startDate: string
    endDate: string
    order: number
    status: string
    durationDays: number
}

interface SemesterDto {
    id: number
    name: string
    code: string
    startDate: string
    endDate: string
    status: string
    academicYear: string
    description: string | null
    createdAt: string
    updatedAt: string | null
    phases: SemesterPhaseDto[]
}

// ---- Helpers ----
function formatDate(iso: string) {
    return new Date(iso).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function statusBadge(status: string) {
    switch (status) {
        case 'Active':
            return { bg: 'bg-green-100 text-green-700 border-green-200', dot: 'bg-green-600', label: 'Đang hoạt động' }
        case 'Closed':
            return { bg: 'bg-slate-100 text-slate-600 border-slate-200', dot: '', label: 'Đã đóng' }
        default:
            return { bg: 'bg-amber-100 text-amber-700 border-amber-200', dot: 'bg-amber-500', label: status }
    }
}

function phaseIcon(type: string) {
    switch (type) {
        case 'Registration': return 'edit_note'
        case 'Evaluation': return 'fact_check'
        case 'Implementation': return 'science'
        case 'Defense': return 'school'
        default: return 'event'
    }
}

function phaseStatus(status: string): 'completed' | 'current' | 'pending' {
    if (status === 'Completed') return 'completed'
    if (status === 'Active' || status === 'InProgress') return 'current'
    return 'pending'
}

// ---- Animations ----
const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.1 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

// ---- Page ----
export function SemestersPage() {
    const [isModalOpen, setIsModalOpen] = useState(false)
    const [semesters, setSemesters] = useState<SemesterDto[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    const fetchSemesters = useCallback(async () => {
        try {
            setLoading(true)
            setError(null)
            const data = await apiClient.get<SemesterDto[]>('/api/admin/semesters')
            setSemesters(data)
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Không thể tải danh sách kỳ học.')
        } finally {
            setLoading(false)
        }
    }, [])

    useEffect(() => {
        fetchSemesters()
    }, [fetchSemesters])

    const handleCreated = () => {
        fetchSemesters()
    }

    return (
        <>
            <Header
                variant="navy"
                title="Quản lý kỳ học"
                breadcrumb={[{ label: 'Quản lý kỳ học' }]}
                searchPlaceholder="Tìm kiếm kỳ học..."
            />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="space-y-6"
                >
                    {/* Header */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h2 className="text-2xl font-bold text-slate-800 tracking-tight">Danh Sách Kỳ Học</h2>
                            <p className="text-sm text-slate-500 mt-1">Quản lý các kỳ bảo vệ đồ án, tiến độ và mốc thời gian.</p>
                        </div>
                        <div className="flex gap-3">
                            <button className="flex items-center gap-2 bg-white hover:bg-slate-50 text-slate-600 border border-slate-300 px-4 py-2.5 rounded-md shadow-sm transition-all font-medium text-sm">
                                <span className="material-symbols-outlined text-[20px]">filter_list</span>
                                Bộ lọc
                            </button>
                            <button onClick={() => setIsModalOpen(true)} className="flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-4 py-2.5 rounded-md shadow-sm transition-all font-medium text-sm">
                                <span className="material-symbols-outlined text-[20px]">add</span>
                                Tạo kỳ học mới
                            </button>
                        </div>
                    </motion.div>

                    {/* Loading */}
                    {loading && (
                        <motion.div variants={item} className="flex flex-col items-center justify-center py-16 gap-3">
                            <span className="material-symbols-outlined animate-spin text-4xl text-primary">progress_activity</span>
                            <p className="text-sm text-slate-500">Đang tải danh sách kỳ học...</p>
                        </motion.div>
                    )}

                    {/* Error */}
                    {error && !loading && (
                        <motion.div variants={item} className="p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
                            <span className="material-symbols-outlined text-red-600 text-[20px] mt-0.5">error</span>
                            <div>
                                <p className="text-sm text-red-800 font-semibold">Lỗi tải dữ liệu</p>
                                <p className="text-xs text-red-600 mt-1">{error}</p>
                                <button onClick={fetchSemesters} className="mt-2 text-xs font-semibold text-red-700 hover:text-red-900 underline">Thử lại</button>
                            </div>
                        </motion.div>
                    )}

                    {/* Empty state */}
                    {!loading && !error && semesters.length === 0 && (
                        <motion.div variants={item} className="flex flex-col items-center justify-center py-16 gap-4 text-center">
                            <div className="w-16 h-16 rounded-full bg-slate-100 flex items-center justify-center">
                                <span className="material-symbols-outlined text-4xl text-slate-400">calendar_month</span>
                            </div>
                            <div>
                                <p className="text-lg font-bold text-slate-700">Chưa có kỳ học nào</p>
                                <p className="text-sm text-slate-500 mt-1">Bấm "Tạo kỳ học mới" để bắt đầu.</p>
                            </div>
                        </motion.div>
                    )}

                    {/* Semester Cards */}
                    {!loading && !error && semesters.length > 0 && (
                        <div className="space-y-6">
                            {semesters.map((semester) => {
                                const badge = statusBadge(semester.status)
                                const isActive = semester.status === 'Active'
                                const currentPhase = semester.phases.find(p => p.status === 'Active' || p.status === 'InProgress')

                                return (
                                    <motion.div
                                        key={semester.id}
                                        variants={item}
                                        className={`bento-card rounded-lg overflow-hidden transition-all hover:shadow-md ${isActive ? 'border-l-4 border-l-primary' : 'opacity-90 hover:opacity-100'}`}
                                    >
                                        <div className="p-6">
                                            {/* Card Header */}
                                            <div className="flex flex-col lg:flex-row justify-between lg:items-start gap-4 mb-6">
                                                <div className="flex items-start gap-4">
                                                    <div className={`w-12 h-12 rounded-lg flex items-center justify-center shrink-0 border ${isActive ? 'bg-primary/10 text-primary border-primary/20' : 'bg-slate-100 text-slate-500 border-slate-200'}`}>
                                                        <span className="material-symbols-outlined text-[28px]">{isActive ? 'calendar_month' : 'history'}</span>
                                                    </div>
                                                    <div>
                                                        <div className="flex items-center gap-3 flex-wrap">
                                                            <h3 className={`text-xl font-bold ${isActive ? 'text-slate-800' : 'text-slate-700'}`}>{semester.name}</h3>
                                                            <span className={`inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold border ${badge.bg}`}>
                                                                {badge.dot && <span className={`w-1.5 h-1.5 rounded-full ${badge.dot}`}></span>}
                                                                {badge.label}
                                                            </span>
                                                            <span className="text-xs text-slate-400 font-mono">{semester.code}</span>
                                                        </div>
                                                        <p className="text-sm text-slate-500 mt-1">
                                                            Thời gian: <span className="font-medium text-slate-700">{formatDate(semester.startDate)} - {formatDate(semester.endDate)}</span>
                                                            {semester.academicYear && <span className="ml-2 text-xs text-slate-400">• Năm học: {semester.academicYear}</span>}
                                                        </p>
                                                        {semester.description && (
                                                            <p className="text-xs text-slate-400 mt-1">{semester.description}</p>
                                                        )}
                                                    </div>
                                                </div>
                                                <div className="flex items-center gap-2">
                                                    <button className="p-2 text-slate-400 hover:text-primary hover:bg-slate-100 rounded-full transition-colors" title="Cấu hình">
                                                        <span className="material-symbols-outlined">settings</span>
                                                    </button>
                                                    <button className="p-2 text-slate-400 hover:text-primary hover:bg-slate-100 rounded-full transition-colors" title="Thêm hành động">
                                                        <span className="material-symbols-outlined">more_vert</span>
                                                    </button>
                                                </div>
                                            </div>

                                            {/* Phases Timeline */}
                                            {semester.phases.length > 0 && (
                                                <div className="border-t border-slate-100 pt-6">
                                                    <div className="flex justify-between items-end mb-4">
                                                        <p className="text-xs font-semibold uppercase text-slate-400 tracking-wider flex items-center gap-2">
                                                            <span className="material-symbols-outlined text-[16px]">timeline</span>
                                                            Tiến độ giai đoạn
                                                        </p>
                                                        {currentPhase && (
                                                            <span className="text-sm font-bold text-primary bg-primary/5 px-3 py-1 rounded border border-primary/10">
                                                                Giai đoạn: {currentPhase.name}
                                                            </span>
                                                        )}
                                                    </div>
                                                    <div className="relative pt-4 pb-8 px-2">
                                                        {/* Track background */}
                                                        <div className="absolute top-1/2 left-0 w-full h-1 bg-slate-100 -translate-y-1/2 rounded-full z-0"></div>
                                                        {/* Track progress */}
                                                        {(() => {
                                                            const total = semester.phases.length
                                                            let pct = 0
                                                            if (total > 1) {
                                                                const currentIndex = semester.phases.findIndex(p => p.status === 'Active' || p.status === 'InProgress')
                                                                if (currentIndex !== -1) {
                                                                    pct = (currentIndex / (total - 1)) * 100
                                                                } else {
                                                                    const completedCount = semester.phases.filter(p => p.status === 'Completed').length
                                                                    if (completedCount === total) pct = 100
                                                                    else if (completedCount > 0) pct = Math.round(((completedCount - 1) / (total - 1)) * 100)
                                                                }
                                                            } else if (total === 1) {
                                                                const completedCount = semester.phases.filter(p => p.status === 'Completed').length
                                                                pct = completedCount === 1 ? 100 : 0
                                                            }
                                                            return (
                                                                <div
                                                                    className={`absolute top-1/2 left-0 h-1 -translate-y-1/2 rounded-full z-0 transition-all duration-1000 ease-in-out ${isActive ? 'bg-green-600 shadow-[0_0_8px_rgba(22,163,74,0.5)]' : 'bg-green-600'}`}
                                                                    style={{ width: `${pct}%` }}
                                                                ></div>
                                                            )
                                                        })()}
                                                        <div className="relative z-10 flex justify-between w-full">
                                                            {semester.phases.map((phase) => (
                                                                <TimelineStep
                                                                    key={phase.id}
                                                                    icon={phaseIcon(phase.type)}
                                                                    label={phase.name}
                                                                    status={phaseStatus(phase.status)}
                                                                    info={phase.status === 'Completed'
                                                                        ? `Hoàn tất ${formatDate(phase.endDate)}`
                                                                        : phaseStatus(phase.status) === 'current'
                                                                            ? 'Đang diễn ra'
                                                                            : `Dự kiến ${formatDate(phase.startDate)}`
                                                                    }
                                                                />
                                                            ))}
                                                        </div>
                                                    </div>
                                                </div>
                                            )}
                                        </div>
                                    </motion.div>
                                )
                            })}
                        </div>
                    )}
                </motion.div>
            </div>
            <CreateSemesterModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} onCreated={handleCreated} />
        </>
    )
}

function TimelineStep({ icon, label, status, info }: { icon: string; label: string; status: 'completed' | 'current' | 'pending'; info: string }) {
    const isCurrent = status === 'current'
    const isPending = status === 'pending'
    const isCompleted = status === 'completed'

    return (
        <div className={`flex flex-col items-center gap-2 group cursor-pointer ${isPending ? 'opacity-60' : ''}`}>
            <div className="relative flex items-center justify-center">
                {/* Pulsing ring for current phase */}
                {isCurrent && (
                    <div className="absolute inset-0 rounded-full bg-green-500/40 animate-ping"></div>
                )}

                <div
                    className={`relative z-10 rounded-full flex items-center justify-center ring-4 ring-white transition-all duration-300 group-hover:scale-110 ${isCurrent
                        ? 'w-10 h-10 bg-white border-[3px] border-green-600 text-green-600 shadow-[0_0_15px_rgba(22,163,74,0.3)]'
                        : isCompleted
                            ? 'w-8 h-8 bg-gradient-to-br from-green-500 to-green-700 text-white shadow-md'
                            : 'w-8 h-8 bg-slate-50 border-2 border-slate-200 text-slate-400'
                        }`}
                >
                    {isCompleted ? (
                        <span className="material-symbols-outlined text-[16px] font-bold">check</span>
                    ) : (
                        <span className={`material-symbols-outlined ${isCurrent ? 'text-[20px] font-bold' : 'text-[16px]'}`}>{icon}</span>
                    )}
                </div>
            </div>
            <div className="text-center">
                <p className={`text-xs font-bold uppercase mt-1 ${isCurrent ? 'text-sm text-green-700' : isPending ? 'text-slate-500' : 'text-slate-700'}`}>{label}</p>
                {isCurrent ? (
                    <p className="text-[10px] text-green-700 font-medium bg-green-50 px-2 py-0.5 rounded mt-0.5 border border-green-200">{info}</p>
                ) : (
                    <p className="text-[10px] text-slate-400">{info}</p>
                )}
            </div>
        </div>
    )
}
