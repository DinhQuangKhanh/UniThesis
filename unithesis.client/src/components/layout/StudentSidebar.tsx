import { NavLink, useLocation } from 'react-router-dom'
import { motion } from 'framer-motion'
import { useAuth } from '@/contexts/AuthContext'

const navItems = [
    { label: 'Trang chủ', icon: 'dashboard', path: '/student' },
    { label: 'Đề tài của tôi', icon: 'book_2', path: '/student/my-topic' },
    { label: 'Kho đề tài đề xuất', icon: 'inventory_2', path: '/student/topics' },
    { label: 'Lịch trình chung', icon: 'calendar_month', path: '/student/schedule' },
    { label: 'Tin nhắn', icon: 'forum', path: '/student/messages', badge: '2' },
]

const systemItems = [
    { label: 'Cài đặt', icon: 'settings', path: '/student/settings' },
]

export function StudentSidebar() {
    const location = useLocation()
    const { user, logout } = useAuth()

    return (
        <motion.aside
            initial={{ x: -64 }}
            animate={{ x: 0 }}
            transition={{ type: 'spring', stiffness: 300, damping: 30 }}
            className="w-64 h-full bg-white border-r border-[#e9ecf1] flex flex-col shrink-0 z-20"
        >
            {/* Logo */}
            <div className="p-6 flex items-center gap-3">
                <div className="bg-primary/10 p-2 rounded-lg text-primary">
                    <span className="material-symbols-outlined fill-1">school</span>
                </div>
                <h1 className="text-primary text-xl font-bold tracking-tight">UniThesis</h1>
            </div>

            {/* Navigation */}
            <nav className="flex-1 px-4 flex flex-col gap-1.5 overflow-y-auto">
                {navItems.map((item) => (
                    <NavItem key={item.path} {...item} active={location.pathname === item.path} />
                ))}
            </nav>

            {/* Footer */}
            <div className="p-4 border-t border-[#e9ecf1]">
                {systemItems.map((item) => (
                    <NavItem key={item.path} {...item} active={location.pathname === item.path} />
                ))}
                <button
                    onClick={logout}
                    className="mt-4 flex items-center gap-3 w-full px-4 py-2 hover:bg-gray-50 rounded-lg transition-colors"
                >
                    <div
                        className="h-10 w-10 rounded-full bg-gray-200 bg-cover bg-center"
                        style={{
                            backgroundImage: `url('https://lh3.googleusercontent.com/aida-public/AB6AXuDcmhFbaP0vMcYOP70wqwwwzqaJSKf-3DBianrl7cMsyN3laUMyvlWs8wnYaX1nGPLIGVInAdzQXNsHKrfv82HbPyOEqqiste4qnOBNZlC9pOaZrSLZZg71hleEKDcTJeHR_GYWsO-keITdsHRIzw7R3rcP9y3adyO2PToD2nxURK0Afp67TENb5qrmoqmXYEQBi2m4pco1pHmYWtV4YOH6-TyoYeaerHqpC6lTitLFtQp4Ir5u8J_xlQdQDj7ofOfugeih7FL2vNVY')`,
                        }}
                    />
                    <div className="flex flex-col overflow-hidden text-left">
                        <p className="text-sm font-bold text-[#101319] truncate">{user?.name || 'Nguyễn Văn An'}</p>
                        <p className="text-xs text-[#58698d] truncate">K62 - CNTT</p>
                    </div>
                </button>
            </div>
        </motion.aside>
    )
}

function NavItem({
    label,
    icon,
    path,
    badge,
    active
}: {
    label: string
    icon: string
    path: string
    badge?: string
    active: boolean
}) {
    return (
        <NavLink
            to={path}
            className={`flex items-center gap-3 px-4 py-3 rounded-lg font-medium transition-all group ${active
                ? 'bg-primary/10 text-primary font-semibold'
                : 'text-[#58698d] hover:bg-gray-50 hover:text-primary'
                }`}
        >
            <span className={`material-symbols-outlined ${active ? 'fill-1' : 'group-hover:fill-1'}`}>{icon}</span>
            <span className="text-sm">{label}</span>
            {badge && (
                <span className="ml-auto bg-red-100 text-red-600 text-xs font-bold px-2 py-0.5 rounded-full">
                    {badge}
                </span>
            )}
        </NavLink>
    )
}
