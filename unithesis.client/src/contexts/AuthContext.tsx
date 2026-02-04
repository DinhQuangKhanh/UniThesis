import { createContext, useContext, useState, ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'

interface User {
    id: string
    name: string
    email: string
    role: 'admin' | 'mentor' | 'evaluator' | 'student'
    avatar?: string
}

interface AuthContextType {
    user: User | null
    isAuthenticated: boolean
    login: (username: string, password: string) => Promise<boolean>
    logout: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(() => {
        const stored = localStorage.getItem('user')
        return stored ? JSON.parse(stored) : null
    })
    const navigate = useNavigate()

    const login = async (username: string, _password: string): Promise<boolean> => {
        // Mock login - support admin, evaluator, mentor, and student roles
        const lowerUsername = username.toLowerCase()
        const isEvaluator = lowerUsername.includes('evaluator') || lowerUsername.includes('professor')
        const isMentor = lowerUsername.includes('mentor') || lowerUsername.includes('gvhd') || lowerUsername.includes('huongdan')
        const isStudent = lowerUsername.includes('student') || lowerUsername.includes('sinhvien') || lowerUsername.includes('sv')

        let mockUser: User

        if (isStudent) {
            mockUser = {
                id: 'SV001',
                name: 'Nguyễn Văn An',
                email: 'annv@student.uni.edu.vn',
                role: 'student',
                avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDcmhFbaP0vMcYOP70wqwwwzqaJSKf-3DBianrl7cMsyN3laUMyvlWs8wnYaX1nGPLIGVInAdzQXNsHKrfv82HbPyOEqqiste4qnOBNZlC9pOaZrSLZZg71hleEKDcTJeHR_GYWsO-keITdsHRIzw7R3rcP9y3adyO2PToD2nxURK0Afp67TENb5qrmoqmXYEQBi2m4pco1pHmYWtV4YOH6-TyoYeaerHqpC6lTitLFtQp4Ir5u8J_xlQdQDj7ofOfugeih7FL2vNVY'
            }
        } else if (isMentor) {
            mockUser = {
                id: 'MT001',
                name: 'TS. Trần Minh Tuấn',
                email: 'tuantm@uni.edu.vn',
                role: 'mentor',
                avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuASbtTbVEoWSAga7fHPa9DFfHlAdKGVh2rz214QRBxPEYfmc0KXwLNfL09_eyK92bi1AHg7gMG7ZNyVDjK4YF9ZjR10rrMEGZgS-OH7nz7dhcZ9fS1B0bPmEd3gwHFr63C74Rp_l0Z3UoRGSky1hq97-XENpD_okZvovDvwSvBEc0kgXlNRlRTGwI2hHSetDowc1MgT0mxpbGjZuqP_z5k4nFRpLr5OoJgKozmF9xXBM0NsACnCyMbCo5FUjY2_TxcGH0TbZAh04tEo'
            }
        } else if (isEvaluator) {
            mockUser = {
                id: 'EV001',
                name: 'Prof. Smith',
                email: 'professor@uni.edu.vn',
                role: 'evaluator',
                avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBDdM3fllGENubBoFyrdojNtPylaHBS5svRCSL7wIauztNwmDocPYjlScbI9a2pgJw4Cj1WgrOQyP1Tn178qoKADKgJSlN_UQCjut7rv7tzqMbLVmoRfS7d7JSvW33wAsUJhEz1eWplkN79Bv1X6HJ6a4apVRQdhvyvksDJ207wX0jRWmCROvzBNlNv6E0wJVamL6S1D3DLFqWNZkDzqKXg2TPTyayG4pvnJHkdjzmPmjz1YFvVuZdm1Gqqp9RNh8i8SUZriq_S8Qj1'
            }
        } else {
            // Default to admin
            mockUser = {
                id: 'AD001',
                name: 'Admin System',
                email: 'admin@uni.edu.vn',
                role: 'admin',
                avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDY_Kx-ArFQ7UqiOPV-bCxIOKtDGqxKO-kjKy9ahQQ1OJK5nEZlhIb72dCTDpcQUKhKf-DqgNqwFCEox19WQRGHBlufj-n4dKRmv74FjkJeyh4IzF2U0u25K1VvSA3Q88KOJP9B9MC-wSqiEMB-z3N1usHwo81aoG83zM7eStPMgdUYErl3xtyJNSqZDrks3CptPld5RiEZFch6ne5Xgj4_ztVzpX2zYDfLpokGYUp1wVtRaLmKNomcX0xKFOTR96SVBXKZP9l3uoh-'
            }
        }

        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500))

        if (username) {
            setUser(mockUser)
            localStorage.setItem('user', JSON.stringify(mockUser))
            return true
        }
        return false
    }

    const logout = () => {
        setUser(null)
        localStorage.removeItem('user')
        navigate('/login')
    }

    return (
        <AuthContext.Provider value={{ user, isAuthenticated: !!user, login, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    const context = useContext(AuthContext)
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider')
    }
    return context
}
