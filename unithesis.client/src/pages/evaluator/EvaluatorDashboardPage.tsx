import { motion } from 'framer-motion'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.08 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const pendingProjects = [
    {
        title: 'AI Traffic Control System',
        student: 'Nguyen Van A',
        studentAvatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAsvPevN4LhNX3dEX7Rg8a23xU5kVvq77McO0EaWFQQoDBSMPa_oqQNVgMqILLC4YiTNe6PAqrGZq0Qth9wagNNw0QXx6HvfgU1kc_i9Wandyj64CVKOtMPDlW9l0lqg9qFTEpgp2jSfg7WqBczqMIxqKimSWrVI40Db_M3sktF8mor5gQhP4uPduMLXi26c4zCvbwvivjrgVJsbkxHEHQZIWXVvUdra0TT2QSqzu_MEzvLmG7SZZ1T7WZmDUlJZTvyLOTThFNH2hUA',
        major: 'CS',
        submitted: 'Oct 24, 2023',
        status: 'Urgent',
        statusColor: 'bg-red-50 text-red-600 border-red-100',
        animate: true,
    },
    {
        title: 'E-commerce Optim. Framework',
        student: 'Tran Thi B',
        studentAvatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAibcdd9EMGrah-CKaXLvNnh0LvKwsxVKLfp8_UWO9ivAvapg3Qcup1ZyYC1L1ck2qN_937Bcr6JvHb1sqrvMB0m69cJzd0GFGOs8CJdoQ8AG_DonhFPP4NhEbQ93aAMAxcnJhcvuuBNIqYmrhqPJ--dkWkCp72kOUSaYP9qRUwmrKz31O7P0-t7bcQR7TZB1GbJyvuviw-0dBulmvF4CTvX6KRKTniq7wNstF4Pk7SiOMBDRaAprVQjCSbI_eHcCSwTY-j7_G4Phrf',
        major: 'Biz',
        submitted: 'Oct 23, 2023',
        status: 'New',
        statusColor: 'bg-blue-50 text-blue-600 border-blue-100',
        animate: false,
    },
    {
        title: 'Blockchain Agri-Supply Chain',
        student: 'Le Van C',
        studentAvatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCSj9vGOQ-o15MT4AOxGFHh6bj33-t0hZBsVGYOWONlfSEzW9OmgIjlo-zWHbmyWTHItaFMhBnyWWkPZCUAgYczFRnedQGNo7vM2C9MYD0MIMelJk54fF0tdvqDIrRrn9uNyvZwb-n3RpLpS9DW0pY1nkxCXnnvLrAaqoZSKoewuLmNlzdlnRkTqmjkR5Qf5IFUcXaeK4ytUblxGxXRNXCX8UEg4-ia8uSv9_UpbDnsL2qiO4O2sJcGSCogil2vrrbzLtX9Atz99g9e',
        major: 'CS',
        submitted: 'Oct 22, 2023',
        status: 'Pending',
        statusColor: 'bg-gray-100 text-gray-600 border-gray-200',
        animate: false,
    },
    {
        title: 'Urban Planning AI Model',
        student: 'Pham T',
        studentAvatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDEerg6rn9OS0-PIJhlnxTIkKgBsLwpYTrc_fp6TVs4hoqr1Bjt0uaJb_VluPBB5RpGvl9w2xzs2pzbNgnpD72rd0wG6vaHc9LLYNBZRWKSRas0tkFkuyTmn9rlQ1nwVKL7JEr09TX070bMca9S63Xt9l3viD2uoKpYXSGhMB3TkYqaNQ9wAZhwwMv9TTuQ9v2ZfO4BpxmKRqzgJabib1NYjdgfAEy9Bb0izgZpTBAocD7TS8pEghQ0EoHp5skX8G4zEQTLXQLTi3xL',
        major: 'Arch',
        submitted: 'Oct 20, 2023',
        status: 'Pending',
        statusColor: 'bg-gray-100 text-gray-600 border-gray-200',
        animate: false,
    },
]

export function EvaluatorDashboardPage() {
    return (
        <div className="p-8 flex flex-col gap-8">
            {/* Header */}
            <motion.header
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex flex-col md:flex-row md:items-end justify-between gap-6"
            >
                <div className="flex flex-col gap-1">
                    <h2 className="text-slate-900 text-2xl md:text-3xl font-bold tracking-tight">
                        Welcome back, Professor
                    </h2>
                    <p className="text-slate-500 text-sm md:text-base">
                        You have <span className="text-primary font-bold">5 projects</span> waiting for evaluation.
                    </p>
                </div>
                <div className="flex gap-3">
                    <button className="hidden md:flex items-center justify-center gap-2 h-10 px-4 rounded-lg border border-gray-200 bg-white text-slate-900 text-sm font-semibold hover:bg-gray-50 transition-colors">
                        <span className="material-symbols-outlined text-[20px]">filter_list</span>
                        <span>Filter</span>
                    </button>
                    <button className="flex items-center justify-center gap-2 h-10 px-5 rounded-lg bg-primary text-white text-sm font-semibold hover:bg-primary-dark transition-colors shadow-lg shadow-primary/20">
                        <span className="material-symbols-outlined text-[20px]">play_circle</span>
                        <span>Start Review Session</span>
                    </button>
                </div>
            </motion.header>

            <motion.div variants={container} initial="hidden" animate="show" className="grid grid-cols-1 xl:grid-cols-12 gap-6">
                {/* Main Content */}
                <div className="xl:col-span-8 flex flex-col gap-6">
                    {/* Pending Reviews Table */}
                    <motion.div variants={item} className="bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden flex flex-col">
                        <div className="px-6 py-5 border-b border-gray-100 flex justify-between items-center">
                            <div className="flex items-center gap-2">
                                <div className="p-1.5 rounded-md bg-primary/10 text-primary">
                                    <span className="material-symbols-outlined text-xl">pending_actions</span>
                                </div>
                                <h3 className="text-slate-900 text-lg font-bold">Pending Reviews</h3>
                            </div>
                            <a className="text-primary text-sm font-bold hover:text-primary-dark transition-colors" href="#">
                                View All
                            </a>
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="bg-gray-50/80 border-b border-gray-100">
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider w-1/3">Project Name</th>
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden sm:table-cell">Student</th>
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden md:table-cell">Major</th>
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider hidden lg:table-cell">Submitted</th>
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider">Status</th>
                                        <th className="px-6 py-3 text-[11px] font-bold text-slate-500 uppercase tracking-wider text-right">Action</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-gray-100">
                                    {pendingProjects.map((project, idx) => (
                                        <motion.tr
                                            key={idx}
                                            whileHover={{ backgroundColor: 'rgb(249 250 251)' }}
                                            className="group transition-colors"
                                        >
                                            <td className="px-6 py-4">
                                                <div className="flex flex-col">
                                                    <span className="text-slate-900 font-semibold text-sm">{project.title}</span>
                                                    <span className="text-xs text-slate-500 mt-1 sm:hidden">{project.student}</span>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 hidden sm:table-cell">
                                                <div className="flex items-center gap-3">
                                                    <div
                                                        className="size-8 rounded-full bg-gray-200 bg-cover ring-1 ring-gray-100"
                                                        style={{ backgroundImage: `url('${project.studentAvatar}')` }}
                                                    />
                                                    <span className="text-slate-900 font-medium text-sm">{project.student}</span>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 hidden md:table-cell">
                                                <span className="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-gray-100 text-gray-700 border border-gray-200">
                                                    {project.major}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 hidden lg:table-cell">
                                                <span className="text-slate-500 text-sm font-medium">{project.submitted}</span>
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${project.statusColor}`}>
                                                    {project.animate && <span className="size-1.5 rounded-full bg-red-500 animate-pulse" />}
                                                    {project.status}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-right">
                                                <button
                                                    className={`inline-flex items-center justify-center h-8 px-4 text-xs font-bold rounded-lg transition-all ${project.status === 'Urgent'
                                                        ? 'bg-primary text-white hover:bg-primary-dark shadow-sm shadow-primary/20'
                                                        : 'bg-white border border-gray-200 text-slate-900 hover:bg-gray-50 hover:border-primary/50 hover:text-primary'
                                                        }`}
                                                >
                                                    Review
                                                </button>
                                            </td>
                                        </motion.tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </motion.div>
                </div>

                {/* Right Column */}
                <div className="xl:col-span-4 flex flex-col gap-6">
                    {/* Performance Card */}
                    <motion.div variants={item} className="bg-white rounded-2xl border border-gray-200 p-6 shadow-sm">
                        <div className="flex justify-between items-start mb-6">
                            <div className="flex items-center gap-2">
                                <div className="p-1.5 rounded-md bg-green-50 text-green-700">
                                    <span className="material-symbols-outlined text-xl">monitoring</span>
                                </div>
                                <h3 className="text-slate-900 text-lg font-bold">Performance</h3>
                            </div>
                            <div className="px-2.5 py-1 bg-gray-100 rounded-md text-xs font-bold text-slate-500">This Term</div>
                        </div>
                        <div className="flex items-center gap-6">
                            <div className="relative size-32 shrink-0">
                                <svg className="size-full -rotate-90" viewBox="0 0 36 36">
                                    <path className="text-gray-100" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="currentColor" strokeWidth="3" />
                                    <path className="text-primary" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="currentColor" strokeDasharray="75, 100" strokeLinecap="round" strokeWidth="3" />
                                </svg>
                                <div className="absolute inset-0 flex flex-col items-center justify-center text-center">
                                    <span className="text-3xl font-bold text-slate-900 tracking-tight">75%</span>
                                </div>
                            </div>
                            <div className="flex flex-col gap-3 flex-1">
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center gap-2">
                                        <div className="size-2 rounded-full bg-primary" />
                                        <span className="text-sm text-slate-500 font-medium">Reviewed</span>
                                    </div>
                                    <span className="text-sm font-bold text-slate-900">42</span>
                                </div>
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center gap-2">
                                        <div className="size-2 rounded-full bg-gray-200" />
                                        <span className="text-sm text-slate-500 font-medium">Pending</span>
                                    </div>
                                    <span className="text-sm font-bold text-slate-900">14</span>
                                </div>
                                <div className="mt-1 pt-3 border-t border-gray-100 flex justify-between items-center">
                                    <span className="text-xs text-slate-500 font-medium">Avg. Time</span>
                                    <span className="text-xs font-bold text-green-600 bg-green-50 px-2 py-0.5 rounded-full">1.5 Days ▼</span>
                                </div>
                            </div>
                        </div>
                    </motion.div>

                    {/* Schedule Card */}
                    <motion.div variants={item} className="bg-white rounded-2xl border border-gray-200 p-6 shadow-sm">
                        <div className="flex justify-between items-center mb-6">
                            <div className="flex items-center gap-2">
                                <div className="p-1.5 rounded-md bg-blue-50 text-blue-700">
                                    <span className="material-symbols-outlined text-xl">event_note</span>
                                </div>
                                <h3 className="text-slate-900 text-lg font-bold">Schedule</h3>
                            </div>
                            <button className="size-8 rounded-full hover:bg-gray-100 flex items-center justify-center transition-colors">
                                <span className="material-symbols-outlined text-slate-500 text-lg">more_horiz</span>
                            </button>
                        </div>
                        <div className="relative pl-2 space-y-8 before:absolute before:left-[11px] before:top-2 before:bottom-2 before:w-[2px] before:bg-gray-100">
                            <div className="relative pl-8">
                                <div className="absolute left-0 top-1.5 size-3 rounded-full bg-orange-500 ring-4 ring-white" />
                                <div className="flex flex-col gap-1">
                                    <p className="text-xs font-bold text-orange-500 uppercase tracking-wide">Today, 14:00</p>
                                    <p className="text-sm font-bold text-slate-900">Thesis Defense: Group 4</p>
                                    <div className="flex items-center gap-1.5 text-xs text-slate-500 mt-0.5">
                                        <span className="material-symbols-outlined text-[14px]">location_on</span>
                                        <span>Room B102, Innovation Hall</span>
                                    </div>
                                </div>
                            </div>
                            <div className="relative pl-8">
                                <div className="absolute left-0 top-1.5 size-3 rounded-full bg-primary ring-4 ring-white" />
                                <div className="flex flex-col gap-1">
                                    <p className="text-xs font-bold text-primary uppercase tracking-wide">Tomorrow, 09:30</p>
                                    <p className="text-sm font-bold text-slate-900">Department Meeting</p>
                                    <div className="flex items-center gap-1.5 text-xs text-slate-500 mt-0.5">
                                        <span className="material-symbols-outlined text-[14px]">videocam</span>
                                        <span>Online (Zoom)</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </motion.div>

                    {/* Recent Feedback Card */}
                    <motion.div variants={item} className="bg-white rounded-2xl border border-gray-200 p-6 shadow-sm flex flex-col gap-4">
                        <div className="flex justify-between items-center mb-1">
                            <div className="flex items-center gap-2">
                                <div className="p-1.5 rounded-md bg-amber-50 text-amber-700">
                                    <span className="material-symbols-outlined text-xl">comment</span>
                                </div>
                                <h3 className="text-slate-900 text-lg font-bold">Recent Feedback</h3>
                            </div>
                        </div>
                        <div className="flex flex-col gap-3">
                            <div className="group cursor-pointer p-4 rounded-xl bg-gray-50 hover:bg-white hover:shadow-md transition-all border border-transparent hover:border-gray-100">
                                <div className="flex justify-between items-start mb-2">
                                    <span className="text-xs font-bold text-slate-900">Re: Solar Energy Grid</span>
                                    <span className="text-[10px] font-medium text-slate-500 bg-white px-1.5 py-0.5 rounded border border-gray-100">2h ago</span>
                                </div>
                                <p className="text-xs text-slate-500 leading-relaxed line-clamp-2 group-hover:text-slate-900 transition-colors">
                                    "Excellent literature review, however the methodology section lacks specific data..."
                                </p>
                            </div>
                            <div className="group cursor-pointer p-4 rounded-xl bg-gray-50 hover:bg-white hover:shadow-md transition-all border border-transparent hover:border-gray-100">
                                <div className="flex justify-between items-start mb-2">
                                    <span className="text-xs font-bold text-slate-900">Re: AI Chatbot</span>
                                    <span className="text-[10px] font-medium text-slate-500 bg-white px-1.5 py-0.5 rounded border border-gray-100">Yesterday</span>
                                </div>
                                <p className="text-xs text-slate-500 leading-relaxed line-clamp-2 group-hover:text-slate-900 transition-colors">
                                    "Approved for next stage. Please incorporate the security feedback from Dr. Emily."
                                </p>
                            </div>
                        </div>
                        <button className="w-full py-2.5 text-xs font-bold text-primary hover:text-primary-dark hover:bg-primary/5 rounded-lg transition-colors mt-2">
                            View All Activity
                        </button>
                    </motion.div>
                </div>
            </motion.div>
        </div>
    )
}
