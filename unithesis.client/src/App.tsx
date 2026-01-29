import { Routes, Route, Navigate } from 'react-router-dom'
import { AnimatePresence } from 'framer-motion'
import { AuthProvider } from '@/contexts/AuthContext'
import { ProtectedRoute } from '@/components/auth/ProtectedRoute'
import { AdminLayout, EvaluatorLayout, MentorLayout, StudentLayout } from '@/components/layout'
import {
    LoginPage,
    DashboardPage,
    ReportsPage,
    SettingsPage,
    SemestersPage,
    UsersPage,
    ProjectsPage,
    SupportPage,
    EvaluatorDashboardPage,
    EvaluatorProjectsPage,
    EvaluatorHistoryPage,
    EvaluatorReviewPage,
    EvaluatorSimilarityPage,
    EvaluatorSchedulePage,
    MentorDashboardPage,
    MentorGroupsPage,
    MentorTopicsPage,
    MentorSchedulePage,
    MentorFeedbackPage,
    MentorTopicDetailPage,
    StudentDashboardPage,
    StudentSchedulePage,
    StudentTopicsPage,
    StudentMyTopicPage,
} from '@/pages'

function App() {
    return (
        <AuthProvider>
            <AnimatePresence mode="wait">
                <Routes>
                    {/* Public Routes */}
                    <Route path="/login" element={<LoginPage />} />

                    {/* Protected Admin Routes */}
                    <Route
                        path="/admin"
                        element={
                            <ProtectedRoute>
                                <AdminLayout />
                            </ProtectedRoute>
                        }
                    >
                        <Route index element={<DashboardPage />} />
                        <Route path="reports" element={<ReportsPage />} />
                        <Route path="settings" element={<SettingsPage />} />
                        <Route path="semesters" element={<SemestersPage />} />
                        <Route path="users" element={<UsersPage />} />
                        <Route path="projects" element={<ProjectsPage />} />
                        <Route path="support" element={<SupportPage />} />
                    </Route>

                    {/* Protected Evaluator Routes */}
                    <Route
                        path="/evaluator"
                        element={
                            <ProtectedRoute>
                                <EvaluatorLayout />
                            </ProtectedRoute>
                        }
                    >
                        <Route index element={<EvaluatorDashboardPage />} />
                        <Route path="projects" element={<EvaluatorProjectsPage />} />
                        <Route path="schedule" element={<EvaluatorSchedulePage />} />
                        <Route path="history" element={<EvaluatorHistoryPage />} />
                        <Route path="review/:id" element={<EvaluatorReviewPage />} />
                        <Route path="review" element={<EvaluatorReviewPage />} />
                        <Route path="similarity" element={<EvaluatorSimilarityPage />} />
                    </Route>

                    {/* Protected Mentor Routes */}
                    <Route
                        path="/mentor"
                        element={
                            <ProtectedRoute>
                                <MentorLayout />
                            </ProtectedRoute>
                        }
                    >
                        <Route index element={<MentorDashboardPage />} />
                        <Route path="groups" element={<MentorGroupsPage />} />
                        <Route path="groups/:id" element={<MentorTopicDetailPage />} />
                        <Route path="topics" element={<MentorTopicsPage />} />
                        <Route path="topics/:id" element={<MentorFeedbackPage />} />
                        <Route path="schedule" element={<MentorSchedulePage />} />
                    </Route>

                    {/* Protected Student Routes */}
                    <Route
                        path="/student"
                        element={
                            <ProtectedRoute>
                                <StudentLayout />
                            </ProtectedRoute>
                        }
                    >
                        <Route index element={<StudentDashboardPage />} />
                        <Route path="my-topic" element={<StudentMyTopicPage />} />
                        <Route path="topics" element={<StudentTopicsPage />} />
                        <Route path="schedule" element={<StudentSchedulePage />} />
                    </Route>

                    {/* Redirect root to admin */}
                    <Route path="/" element={<Navigate to="/admin" replace />} />

                    {/* Catch all - redirect to admin */}
                    <Route path="*" element={<Navigate to="/admin" replace />} />
                </Routes>
            </AnimatePresence>
        </AuthProvider>
    )
}

export default App
