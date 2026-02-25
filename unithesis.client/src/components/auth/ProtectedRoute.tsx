import { Navigate, useLocation } from 'react-router-dom'
import { useAuth } from '@/contexts/AuthContext'
import { useMaintenance } from '@/contexts/MaintenanceContext'
import { ReactNode } from 'react'

interface ProtectedRouteProps {
    children: ReactNode
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
    const { isAuthenticated, user } = useAuth()
    const { isMaintenanceMode } = useMaintenance()
    const location = useLocation()

    if (!isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />
    }

    // If maintenance mode is on and user is not admin, redirect to maintenance page
    if (isMaintenanceMode && user?.role !== 'admin') {
        return <Navigate to="/maintenance" replace />
    }

    return <>{children}</>
}
