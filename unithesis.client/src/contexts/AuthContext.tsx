import { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    signInWithEmailAndPassword,
    signInWithPopup,
    GoogleAuthProvider,
    signOut,
    onAuthStateChanged,
    type User as FirebaseUser,
} from 'firebase/auth'
import { auth } from '@/config/firebase'

interface User {
    id: string
    name: string
    email: string
    role: 'admin' | 'mentor' | 'evaluator' | 'student'
    avatar?: string
    firebaseToken?: string
}

interface AuthContextType {
    user: User | null
    isAuthenticated: boolean
    login: (username: string, password: string) => Promise<boolean>
    loginWithGoogle: () => Promise<boolean>
    loginWithEmailPassword: (email: string, password: string) => Promise<boolean>
    logout: () => void
    isLoading: boolean
}

const useFirebase = import.meta.env.VITE_USE_FIREBASE_EMULATOR === 'true' ||
    (import.meta.env.VITE_FIREBASE_API_KEY && import.meta.env.VITE_FIREBASE_API_KEY !== 'fake-api-key')

const AuthContext = createContext<AuthContextType | undefined>(undefined)

function inferRoleFromEmail(email: string): User['role'] {
    const lower = email.toLowerCase()
    if (lower.includes('admin')) return 'admin'
    if (lower.includes('eval')) return 'evaluator'
    if (lower.includes('mentor')) return 'mentor'
    return 'student'
}

function firebaseUserToUser(fbUser: FirebaseUser, token: string): User {
    const email = fbUser.email || ''
    return {
        id: fbUser.uid,
        name: fbUser.displayName || email.split('@')[0],
        email,
        role: inferRoleFromEmail(email),
        avatar: fbUser.photoURL || undefined,
        firebaseToken: token,
    }
}

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(() => {
        const stored = localStorage.getItem('user')
        return stored ? JSON.parse(stored) : null
    })
    const [isLoading, setIsLoading] = useState(!!useFirebase)
    const navigate = useNavigate()

    // Listen for Firebase auth state changes
    useEffect(() => {
        if (!useFirebase) return

        const unsubscribe = onAuthStateChanged(auth, async (fbUser) => {
            if (fbUser) {
                const token = await fbUser.getIdToken()
                const appUser = firebaseUserToUser(fbUser, token)
                setUser(appUser)
                localStorage.setItem('user', JSON.stringify(appUser))
            } else {
                setUser(null)
                localStorage.removeItem('user')
            }
            setIsLoading(false)
        })

        return () => unsubscribe()
    }, [])

    const loginWithEmailPassword = async (email: string, password: string): Promise<boolean> => {
        try {
            const result = await signInWithEmailAndPassword(auth, email, password)
            const token = await result.user.getIdToken()
            const appUser = firebaseUserToUser(result.user, token)
            setUser(appUser)
            localStorage.setItem('user', JSON.stringify(appUser))
            return true
        } catch (err) {
            console.error('Email/password login failed:', err)
            return false
        }
    }

    const loginWithGoogle = async (): Promise<boolean> => {
        try {
            const provider = new GoogleAuthProvider()
            provider.setCustomParameters({ hd: 'fpt.edu.vn' })
            const result = await signInWithPopup(auth, provider)
            const token = await result.user.getIdToken()
            const appUser = firebaseUserToUser(result.user, token)
            setUser(appUser)
            localStorage.setItem('user', JSON.stringify(appUser))
            return true
        } catch (err) {
            console.error('Google login failed:', err)
            return false
        }
    }

    // Legacy mock login (kept for backward compatibility when Firebase is not configured)
    const login = async (username: string, _password: string): Promise<boolean> => {
        if (useFirebase) {
            return loginWithGoogle()
        }

        const lowerUsername = username.toLowerCase()
        const isEvaluator = lowerUsername.includes('evaluator') || lowerUsername.includes('professor')
        const isMentor = lowerUsername.includes('mentor') || lowerUsername.includes('gvhd') || lowerUsername.includes('huongdan')
        const isStudent = lowerUsername.includes('student') || lowerUsername.includes('sinhvien') || lowerUsername.includes('sv')

        let mockUser: User

        if (isStudent) {
            mockUser = { id: 'SV001', name: 'Nguyen Van An', email: 'annv@student.uni.edu.vn', role: 'student' }
        } else if (isMentor) {
            mockUser = { id: 'MT001', name: 'TS. Tran Minh Tuan', email: 'tuantm@uni.edu.vn', role: 'mentor' }
        } else if (isEvaluator) {
            mockUser = { id: 'EV001', name: 'Prof. Smith', email: 'professor@uni.edu.vn', role: 'evaluator' }
        } else {
            mockUser = { id: 'AD001', name: 'Admin System', email: 'admin@uni.edu.vn', role: 'admin' }
        }

        await new Promise(resolve => setTimeout(resolve, 500))

        if (username) {
            setUser(mockUser)
            localStorage.setItem('user', JSON.stringify(mockUser))
            return true
        }
        return false
    }

    const logout = async () => {
        if (useFirebase) {
            await signOut(auth)
        }
        setUser(null)
        localStorage.removeItem('user')
        navigate('/login')
    }

    return (
        <AuthContext.Provider value={{
            user,
            isAuthenticated: !!user,
            login,
            loginWithGoogle,
            loginWithEmailPassword,
            logout,
            isLoading,
        }}>
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
