import { motion } from 'framer-motion'
import { Header } from '@/components/layout'

const container = {
    hidden: { opacity: 0 },
    show: { opacity: 1, transition: { staggerChildren: 0.05 } }
}

const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 }
}

const roles = ['Tất cả', 'Sinh viên', 'Mentor', 'Evaluator', 'Admin']

export function UsersPage() {
    return (
        <>
            <Header title="Quản Lý Người Dùng" showSearch={false} />

            <div className="flex-1 overflow-y-auto p-8 scrollbar-hide bg-slate-50">
                <motion.div
                    variants={container}
                    initial="hidden"
                    animate="show"
                    className="flex flex-col h-full"
                >
                    {/* Filters */}
                    <motion.div variants={item} className="flex flex-col lg:flex-row lg:items-center justify-between gap-4 mb-6">
                        <div className="flex bg-white p-1 rounded-lg border border-slate-200 shadow-sm overflow-x-auto scrollbar-hide">
                            {roles.map((role, i) => (
                                <button
                                    key={role}
                                    className={`px-4 py-2 text-sm font-medium rounded-md whitespace-nowrap transition-all ${i === 0 ? 'bg-primary text-white shadow-sm' : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'
                                        }`}
                                >
                                    {role}
                                </button>
                            ))}
                        </div>
                        <div className="flex items-center gap-3 w-full lg:w-auto">
                            <div className="relative flex-1 lg:w-72">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-slate-400 text-[18px]">search</span>
                                <input
                                    className="w-full pl-9 pr-4 py-2 text-sm border border-slate-200 rounded-md focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary bg-white placeholder-slate-400 text-slate-700"
                                    placeholder="Tìm theo tên, email hoặc mã số..."
                                    type="text"
                                />
                            </div>
                            <button className="flex items-center justify-center gap-2 px-4 py-2 bg-white border border-slate-200 text-slate-700 text-sm font-medium rounded-md hover:bg-slate-50 hover:text-primary transition-colors whitespace-nowrap">
                                <span className="material-symbols-outlined text-[20px]">upload_file</span>
                                <span className="hidden sm:inline">Nhập Excel</span>
                            </button>
                            <button className="flex items-center justify-center gap-2 px-4 py-2 bg-primary text-white text-sm font-medium rounded-md hover:bg-primary-light transition-colors shadow-sm whitespace-nowrap">
                                <span className="material-symbols-outlined text-[20px]">person_add</span>
                                <span className="hidden sm:inline">Thêm mới</span>
                            </button>
                        </div>
                    </motion.div>

                    {/* Table */}
                    <motion.div variants={item} className="bento-card rounded-md overflow-hidden bg-white flex flex-col flex-1 min-h-0">
                        <div className="overflow-auto flex-1">
                            <table className="w-full text-left text-sm text-slate-600">
                                <thead className="bg-slate-50 text-xs uppercase font-bold text-slate-500 border-b border-slate-200 sticky top-0 z-10">
                                    <tr>
                                        <th className="px-6 py-4">Người dùng</th>
                                        <th className="px-6 py-4">Vai trò</th>
                                        <th className="px-6 py-4">Khoa / Đơn vị</th>
                                        <th className="px-6 py-4">Ngày tạo</th>
                                        <th className="px-6 py-4">Trạng thái</th>
                                        <th className="px-6 py-4 text-right">Hành động</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100">
                                    <UserRow
                                        initials="NA"
                                        initialsColor="bg-blue-100 text-blue-600"
                                        name="Nguyễn Văn An"
                                        code="SV2024001"
                                        email="an.nguyen@uni.edu.vn"
                                        role="Sinh viên"
                                        roleColor="bg-slate-100 text-slate-700 border-slate-200"
                                        department="Công Nghệ Thông Tin"
                                        date="15/08/2023"
                                        status="active"
                                    />
                                    <UserRow
                                        avatar="https://lh3.googleusercontent.com/aida-public/AB6AXuAVLWRq7X6rhNLONqAo4Ou3t_Yq24D93xalGopZG_vTL5pmrXIEfQZ_qdoBNxlfRbSpt9jIceOrReAqOq5ehxchSXjj_AUE4PSjjEsv4xvURmpEUW4E4CO7P4eCPBL01LW-VcHJSy4GJ3zrV21J1ut0pWiO_qgid_2kE-iK9gDgSi0O6QPDvyaK_4qi_q2CkcbqwT-l4zEBf2rAu-8KyAmT4PBgijmeHGEMQjWJkUJMqBlwaKbOWo51SrAWwYTYWKupke-yK_nsrddB"
                                        name="TS. Lê Thị Bích"
                                        code="GV9921"
                                        email="bich.le@uni.edu.vn"
                                        role="Mentor"
                                        roleColor="bg-purple-50 text-purple-700 border-purple-100"
                                        department="Khoa học Máy tính"
                                        date="10/01/2022"
                                        status="active"
                                    />
                                    <UserRow
                                        initials="TH"
                                        initialsColor="bg-slate-200 text-slate-500"
                                        name="Trần Hoàng"
                                        code="SV2023882"
                                        email="hoang.tran@uni.edu.vn"
                                        role="Sinh viên"
                                        roleColor="bg-slate-100 text-slate-700 border-slate-200"
                                        department="Kinh Tế Số"
                                        date="22/09/2023"
                                        status="locked"
                                    />
                                    <UserRow
                                        avatar="https://lh3.googleusercontent.com/aida-public/AB6AXuAl7-z1a1swM4oDiNIbHY_2Tgl-LxcnkUi8Uz9JhgHO-GEuaaMt_Atf6byuMwAdHuRv_cIYGBIyrtymLGiLMfi97iJdD1zgpdQwHkAHhc5Clrhv83f_A2RnCR3_uXj9MFt5B9h4Swey1yxmrkF6c5Z4q4JOTIwQr6rfIkizM4V4J1fn481mOPKJLbjtUT-6kJK4bvwuN5A7-5Oky3753sHRaC2EveD9ih25npVNXzrg8Kesz0v3Uf-2YBLpcTZMRGIPCBNnTOb40WWl"
                                        name="ThS. Phạm Văn Minh"
                                        code="HD003"
                                        email="minh.pham@uni.edu.vn"
                                        role="Evaluator"
                                        roleColor="bg-orange-50 text-orange-700 border-orange-100"
                                        department="Quản Trị Kinh Doanh"
                                        date="05/03/2023"
                                        status="active"
                                    />
                                    <UserRow
                                        initials="QT"
                                        initialsColor="bg-primary text-white"
                                        name="Quản Trị Hệ Thống"
                                        code="AD001"
                                        email="admin@uni.edu.vn"
                                        role="Admin"
                                        roleColor="bg-slate-800 text-slate-100 border-slate-700"
                                        department="Phòng Đào Tạo"
                                        date="01/01/2020"
                                        status="active"
                                        noActions
                                    />
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination */}
                        <div className="p-4 border-t border-slate-200 flex items-center justify-between bg-white shrink-0">
                            <span className="text-sm text-slate-500 hidden sm:inline">
                                Hiển thị <span className="font-medium text-slate-900">1-5</span> trên <span className="font-medium text-slate-900">1,240</span> người dùng
                            </span>
                            <div className="flex gap-1 w-full sm:w-auto justify-center sm:justify-end">
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm disabled:opacity-50 transition-colors">Trước</button>
                                <button className="px-3 py-1 bg-primary text-white rounded text-sm hover:bg-primary-light transition-colors">1</button>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">2</button>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">3</button>
                                <span className="px-2 py-1 text-slate-400 hidden sm:inline">...</span>
                                <button className="px-3 py-1 border border-slate-200 rounded hover:bg-slate-50 text-slate-600 text-sm transition-colors">Sau</button>
                            </div>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </>
    )
}

function UserRow({
    initials,
    initialsColor,
    avatar,
    name,
    code,
    email,
    role,
    roleColor,
    department,
    date,
    status,
    noActions,
}: {
    initials?: string
    initialsColor?: string
    avatar?: string
    name: string
    code: string
    email: string
    role: string
    roleColor: string
    department: string
    date: string
    status: 'active' | 'locked'
    noActions?: boolean
}) {
    const isLocked = status === 'locked'

    return (
        <motion.tr
            whileHover={{ backgroundColor: 'rgb(248 250 252)' }}
            className={`transition-colors group ${isLocked ? 'bg-slate-50/50' : ''}`}
        >
            <td className={`px-6 py-4 ${isLocked ? 'opacity-60' : ''}`}>
                <div className="flex items-center gap-3">
                    {avatar ? (
                        <div className="w-10 h-10 rounded-full bg-slate-200 bg-cover bg-center" style={{ backgroundImage: `url("${avatar}")` }} />
                    ) : (
                        <div className={`w-10 h-10 rounded-full ${initialsColor} flex items-center justify-center font-bold text-sm`}>
                            {initials}
                        </div>
                    )}
                    <div>
                        <p className="font-semibold text-slate-800">{name}</p>
                        <p className="text-xs text-slate-500">{code} • {email}</p>
                    </div>
                </div>
            </td>
            <td className={`px-6 py-4 ${isLocked ? 'opacity-60' : ''}`}>
                <span className={`inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold border ${roleColor}`}>
                    {role}
                </span>
            </td>
            <td className={`px-6 py-4 ${isLocked ? 'opacity-60' : ''}`}>{department}</td>
            <td className={`px-6 py-4 ${isLocked ? 'opacity-60' : ''}`}>{date}</td>
            <td className="px-6 py-4">
                {isLocked ? (
                    <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded text-xs font-medium bg-slate-100 text-slate-500 border border-slate-200">
                        <span className="material-symbols-outlined text-[14px]">lock</span>
                        Đã khóa
                    </span>
                ) : (
                    <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded text-xs font-medium bg-success/10 text-success border border-success/20">
                        <span className="w-1.5 h-1.5 rounded-full bg-success"></span>
                        Hoạt động
                    </span>
                )}
            </td>
            <td className="px-6 py-4 text-right">
                {noActions ? (
                    <span className="text-xs text-slate-400 italic">No actions</span>
                ) : (
                    <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <button className="p-1.5 text-slate-400 hover:text-primary hover:bg-primary/5 rounded transition-colors" title="Chỉnh sửa">
                            <span className="material-symbols-outlined text-[20px]">edit</span>
                        </button>
                        {isLocked ? (
                            <button className="p-1.5 text-success hover:bg-success/5 rounded transition-colors" title="Mở khóa">
                                <span className="material-symbols-outlined text-[20px]">lock_open_right</span>
                            </button>
                        ) : (
                            <button className="p-1.5 text-slate-400 hover:text-error hover:bg-error/5 rounded transition-colors" title="Khóa tài khoản">
                                <span className="material-symbols-outlined text-[20px]">lock_open</span>
                            </button>
                        )}
                    </div>
                )}
            </td>
        </motion.tr>
    )
}
