import { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { motion } from 'framer-motion'
import { useAuth } from '@/contexts/AuthContext'

export function LoginPage() {
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [showPassword, setShowPassword] = useState(false)
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState('')

    const { login } = useAuth()
    const navigate = useNavigate()
    const location = useLocation()

    const from = location.state?.from?.pathname || '/admin'

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError('')
        setIsLoading(true)

        try {
            const success = await login(username, password)
            if (success) {
                navigate(from, { replace: true })
            } else {
                setError('Thông tin đăng nhập không chính xác')
            }
        } catch {
            setError('Đã có lỗi xảy ra')
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <div className="font-display academic-pattern text-slate-800 min-h-screen flex flex-col items-center justify-center p-4 relative overflow-hidden selection:bg-primary/20 selection:text-primary-dark">
            {/* Background decorations */}
            <div className="absolute top-[-10%] left-[-5%] w-[400px] h-[400px] bg-primary/5 rounded-full blur-[80px] pointer-events-none" />
            <div className="absolute bottom-[-10%] right-[-5%] w-[300px] h-[300px] bg-primary/10 rounded-full blur-[60px] pointer-events-none" />

            <motion.main
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, ease: 'easeOut' }}
                className="relative w-full max-w-[440px] bg-white rounded-2xl shadow-soft flex flex-col overflow-hidden border border-white/50 backdrop-blur-sm"
            >
                {/* Header */}
                <motion.div
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.2, duration: 0.4 }}
                    className="pt-8 pb-4 px-8 text-center flex flex-col items-center"
                >
                    <div className="w-14 h-14 bg-gradient-to-br from-primary/10 to-primary/20 text-primary rounded-xl flex items-center justify-center mb-4 shadow-sm border border-primary/10">
                        <span className="material-symbols-outlined text-3xl">school</span>
                    </div>
                    <h1 className="text-2xl font-bold tracking-tight text-slate-800">Đăng nhập hệ thống</h1>
                    <p className="text-slate-500 text-sm mt-1 font-medium">
                        Hệ thống quản lý thẩm định đề tài
                    </p>
                </motion.div>

                {/* Form */}
                <motion.form
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: 0.3, duration: 0.4 }}
                    onSubmit={handleSubmit}
                    className="p-8 pt-4 flex flex-col gap-5"
                >
                    {error && (
                        <motion.div
                            initial={{ opacity: 0, height: 0 }}
                            animate={{ opacity: 1, height: 'auto' }}
                            className="p-3 bg-error/10 border border-error/20 rounded-lg text-error text-sm"
                        >
                            {error}
                        </motion.div>
                    )}

                    {/* Username */}
                    <div className="flex flex-col gap-2">
                        <label className="text-slate-800 text-sm font-semibold leading-normal flex items-center gap-2">
                            <span className="material-symbols-outlined text-lg text-primary">badge</span>
                            Mã số (MSSV/MSGV)
                        </label>
                        <div className="relative group">
                            <input
                                className="w-full resize-none overflow-hidden rounded-lg text-slate-800 focus:outline-none focus:ring-2 focus:ring-primary/20 border border-slate-200 bg-slate-50 focus:border-primary h-12 placeholder:text-slate-400 p-4 text-base font-normal leading-normal transition-all"
                                placeholder="Nhập mã số định danh..."
                                type="text"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </div>
                    </div>

                    {/* Password */}
                    <div className="flex flex-col gap-2">
                        <div className="flex justify-between items-center">
                            <label className="text-slate-800 text-sm font-semibold leading-normal flex items-center gap-2">
                                <span className="material-symbols-outlined text-lg text-primary">lock</span>
                                Mật khẩu
                            </label>
                        </div>
                        <div className="relative group">
                            <input
                                className="w-full resize-none overflow-hidden rounded-lg text-slate-800 focus:outline-none focus:ring-2 focus:ring-primary/20 border border-slate-200 bg-slate-50 focus:border-primary h-12 placeholder:text-slate-400 p-4 text-base font-normal leading-normal transition-all"
                                placeholder="••••••••"
                                type={showPassword ? 'text' : 'password'}
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                            <button
                                className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-primary transition-colors"
                                type="button"
                                onClick={() => setShowPassword(!showPassword)}
                            >
                                <span className="material-symbols-outlined text-[20px]">
                                    {showPassword ? 'visibility' : 'visibility_off'}
                                </span>
                            </button>
                        </div>
                        <div className="flex justify-end mt-1">
                            <a
                                className="text-primary text-xs font-semibold hover:underline decoration-primary/50 underline-offset-2 transition-all"
                                href="#"
                            >
                                Quên mật khẩu?
                            </a>
                        </div>
                    </div>

                    {/* Submit Button */}
                    <motion.button
                        whileHover={{ y: -2 }}
                        whileTap={{ scale: 0.98 }}
                        disabled={isLoading}
                        className="flex w-full items-center justify-center rounded-lg bg-primary hover:bg-primary-light text-white font-bold h-12 transition-all shadow-[0_4px_14px_0_rgba(40,177,189,0.39)] hover:shadow-[0_6px_20px_rgba(40,177,189,0.23)] mt-2 disabled:opacity-70 disabled:cursor-not-allowed"
                    >
                        {isLoading ? (
                            <span className="material-symbols-outlined animate-spin">progress_activity</span>
                        ) : (
                            'Đăng nhập'
                        )}
                    </motion.button>

                    {/* Divider */}
                    <div className="relative flex py-2 items-center">
                        <div className="flex-grow border-t border-slate-200" />
                        <span className="flex-shrink-0 mx-4 text-xs font-medium text-slate-400 uppercase tracking-wider">
                            Hoặc tiếp tục với
                        </span>
                        <div className="flex-grow border-t border-slate-200" />
                    </div>

                    {/* OAuth Buttons */}
                    <div className="grid grid-cols-2 gap-3">
                        <motion.button
                            whileHover={{ scale: 1.02 }}
                            whileTap={{ scale: 0.98 }}
                            type="button"
                            className="flex items-center justify-center gap-2 h-11 rounded-lg border border-slate-200 bg-white hover:bg-slate-50 transition-colors group"
                        >
                            <svg className="w-5 h-5" viewBox="0 0 24 24">
                                <path
                                    d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                                    fill="#4285F4"
                                />
                                <path
                                    d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                                    fill="#34A853"
                                />
                                <path
                                    d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                                    fill="#FBBC05"
                                />
                                <path
                                    d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                                    fill="#EA4335"
                                />
                            </svg>
                            <span className="text-sm font-semibold text-slate-700">Google</span>
                        </motion.button>

                        <motion.button
                            whileHover={{ scale: 1.02 }}
                            whileTap={{ scale: 0.98 }}
                            type="button"
                            className="flex items-center justify-center gap-2 h-11 rounded-lg border border-slate-200 bg-white hover:bg-slate-50 transition-colors group"
                        >
                            <svg className="w-5 h-5" viewBox="0 0 23 23">
                                <path d="M0 0h23v23H0z" fill="#f3f3f3" />
                                <path d="M1 1h10v10H1z" fill="#f35325" />
                                <path d="M12 1h10v10H12z" fill="#81bc06" />
                                <path d="M1 12h10v10H1z" fill="#05a6f0" />
                                <path d="M12 12h10v10H12z" fill="#ffba08" />
                            </svg>
                            <span className="text-sm font-semibold text-slate-700">Office 365</span>
                        </motion.button>
                    </div>
                </motion.form>

                {/* Bottom accent */}
                <div className="h-1.5 w-full bg-gradient-to-r from-primary via-[#5eead4] to-primary" />
            </motion.main>

            {/* Footer */}
            <motion.footer
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ delay: 0.5, duration: 0.4 }}
                className="mt-8 text-center relative z-10"
            >
                <p className="text-slate-500 text-xs font-medium">
                    Hỗ trợ kỹ thuật:{' '}
                    <a className="underline hover:text-primary transition-colors" href="#">
                        support@university.edu.vn
                    </a>
                </p>
                <p className="text-slate-400 text-[10px] mt-2">
                    © 2024 University Graduation Project Management System
                </p>
            </motion.footer>
        </div>
    )
}
