import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import ProtectedRoute from './routes/ProtectedRoute'
import Layout from './components/Layout'
import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import EmployeesPage from './pages/EmployeesPage'
import EmployeeDetailsPage from './pages/EmployeeDetailsPage'
import DepartmentsPage from './pages/DepartmentsPage'
import JobsPage from './pages/JobsPage'
import CountriesPage from './pages/CountriesPage'
import UsersPage from './pages/UsersPage'
import ApplicationsPage from './pages/ApplicationsPage'
import LeaveTypesPage from './pages/LeaveTypesPage'
import LeaveBalancesPage from './pages/LeaveBalancesPage'
import PublicHolidaysPage from './pages/PublicHolidaysPage'
import SalaryPage from './pages/SalaryPage'

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/employees" element={<EmployeesPage />} />
              <Route path="/employees/:id" element={<EmployeeDetailsPage />} />
              <Route path="/departments" element={<DepartmentsPage />} />
              <Route path="/jobs" element={<JobsPage />} />
              <Route path="/countries" element={<CountriesPage />} />
              <Route path="/salary" element={<SalaryPage />} />
              <Route path="/applications" element={<ApplicationsPage />} />
              <Route path="/leave-types" element={<LeaveTypesPage />} />
              <Route path="/leave-balances" element={<LeaveBalancesPage />} />
              <Route path="/holidays" element={<PublicHolidaysPage />} />
              <Route path="/users" element={<UsersPage />} />
            </Route>
          </Route>
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
