import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { NotificationDropdown } from '@/components/layout'
import { studentGroupService, type MentorGroupDto } from '@/lib/studentGroupService'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.08 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

export function MentorGroupsPage() {
    const navigate = useNavigate()
    const [groups, setGroups] = useState<MentorGroupDto[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        studentGroupService.getMentorGroups()
            .then(setGroups)
            .catch(err => setError(err.message))
            .finally(() => setLoading(false))
    }, [])

    return (
        <>
            {/* Header */}
            <header className="h-16 flex items-center justify-between px-8 bg-slate-800 border-b border-slate-700 flex-shrink-0 z-50 shadow-md">
                <div className="flex items-center gap-4">
                    <div className="flex items-center gap-2 text-white">
                        <span className="text-slate-400 font-medium text-sm">Quản lý</span>
                        <span className="material-symbols-outlined text-sm text-slate-500">chevron_right</span>
                        <h2 className="text-lg font-bold">Nhóm của tôi</h2>
                    </div>
                </div>
                <div className="flex items-center gap-4">
                    <div className="relative hidden md:block">
                        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                            <span className="material-symbols-outlined text-slate-400 text-[20px]">search</span>
                        </div>
                        <input
                            type="text"
                            className="block w-64 pl-10 pr-3 py-2 border-none rounded-lg bg-slate-700 text-white placeholder-slate-400 focus:outline-none focus:bg-slate-600 focus:ring-1 focus:ring-primary/50 sm:text-sm transition-all"
                            placeholder="Tìm kiếm nhóm, đề tài..."
                        />
                    </div>
                    <NotificationDropdown role="mentor" isNavy={true} />
                </div>
            </header>

            {/* Content */}
            <div className="flex-1 overflow-y-auto p-8 bg-slate-100">
                <motion.div variants={container} initial="hidden" animate="show" className="space-y-8">
                    {/* Title */}
                    <motion.div variants={item} className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                        <div>
                            <h1 className="text-2xl font-bold text-slate-900">Danh sách nhóm hướng dẫn</h1>
                            <p className="text-slate-500 text-sm mt-1">Quản lý tiến độ và theo dõi các nhóm sinh viên</p>
                        </div>
                    </motion.div>

                    {/* Loading State */}
                    {loading && (
                        <div className="flex items-center justify-center py-12">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary" />
                        </div>
                    )}

                    {/* Error State */}
                    {error && (
                        <div className="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
                            <span className="material-symbols-outlined text-red-400 text-3xl mb-2">error</span>
                            <p className="text-red-600 text-sm">{error}</p>
                        </div>
                    )}

                    {/* Empty State */}
                    {!loading && !error && groups.length === 0 && (
                        <div className="bg-white rounded-xl border border-slate-200 p-12 text-center">
                            <span className="material-symbols-outlined text-slate-300 text-5xl mb-3">group_off</span>
                            <h3 className="text-lg font-bold text-slate-700 mb-1">Chưa có nhóm nào</h3>
                            <p className="text-slate-500 text-sm">Bạn chưa được phân công hướng dẫn nhóm nào trong học kỳ này.</p>
                        </div>
                    )}

                    {/* Groups Grid */}
                    {!loading && groups.length > 0 && (
                        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                            {groups.map((group) => (
                                <motion.div
                                    key={group.groupId}
                                    variants={item}
                                    className="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 flex flex-col overflow-hidden"
                                >
                                    <div className="p-5 flex-1">
                                        <div className="flex justify-between items-start mb-3">
                                            <div>
                                                <h3 className="text-lg font-bold text-slate-900">{group.groupName ?? group.groupCode}</h3>
                                                <span className="inline-flex items-center gap-1 text-xs text-slate-500 mt-1">
                                                    <span className="material-symbols-outlined text-[14px]">tag</span>
                                                    {group.groupCode}
                                                </span>
                                            </div>
                                            <span className={`text-xs font-bold px-2 py-1 rounded-full ${
                                                group.groupStatus === 'Active' ? 'bg-green-50 text-green-700' :
                                                group.groupStatus === 'Completed' ? 'bg-blue-50 text-blue-700' :
                                                'bg-gray-50 text-gray-700'
                                            }`}>
                                                {group.groupStatus === 'Active' ? 'Hoạt động' :
                                                 group.groupStatus === 'Completed' ? 'Hoàn thành' : group.groupStatus}
                                            </span>
                                        </div>
                                        {group.projectName && (
                                            <h4 className="font-medium text-slate-800 text-sm mb-4 line-clamp-2 h-10">{group.projectName}</h4>
                                        )}
                                        {!group.projectName && (
                                            <p className="text-sm text-slate-400 italic mb-4 h-10">Chưa có đề tài</p>
                                        )}
                                        <div className="flex items-center justify-between">
                                            <div className="flex -space-x-2 overflow-hidden">
                                                {group.members.slice(0, 3).map((member, i) => (
                                                    <div
                                                        key={member.studentId}
                                                        className="inline-block size-8 rounded-full ring-2 ring-white bg-slate-200 flex items-center justify-center text-xs font-bold text-slate-500"
                                                        title={member.fullName}
                                                    >
                                                        {member.fullName.charAt(0)}
                                                    </div>
                                                ))}
                                                {group.members.length > 3 && (
                                                    <div className="inline-block size-8 rounded-full ring-2 ring-white bg-slate-100 flex items-center justify-center text-xs font-bold text-slate-500">
                                                        +{group.members.length - 3}
                                                    </div>
                                                )}
                                            </div>
                                            <span className="text-xs font-medium text-slate-500">{group.members.length} thành viên</span>
                                        </div>
                                    </div>
                                    <div className="px-5 py-3 bg-slate-50 border-t border-slate-100 flex items-center justify-end">
                                        <button
                                            onClick={() => navigate(`/mentor/groups/${group.groupId}`)}
                                            className="text-primary hover:text-primary/80 text-sm font-semibold flex items-center gap-1"
                                        >
                                            Chi tiết <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
                                        </button>
                                    </div>
                                </motion.div>
                            ))}
                        </div>
                    )}

                    {/* Pagination */}
                    {!loading && groups.length > 0 && (
                        <motion.div variants={item} className="flex items-center justify-between border-t border-slate-200 pt-4">
                            <p className="text-sm text-slate-500">
                                Hiển thị <span className="font-medium text-slate-900">1-{groups.length}</span> trên <span className="font-medium text-slate-900">{groups.length}</span> nhóm
                            </p>
                        </motion.div>
                    )}
                </motion.div>
            </div>
        </>
    )
}
