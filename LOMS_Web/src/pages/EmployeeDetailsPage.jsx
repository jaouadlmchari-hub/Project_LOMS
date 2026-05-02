import { useEffect, useState } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { employeeService } from '../services/employeeService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'

function Detail({ label, value }) {
  return (
    <div>
      <dt className="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-1">{label}</dt>
      <dd className="text-sm text-gray-800 font-medium">{value ?? '—'}</dd>
    </div>
  )
}

export default function EmployeeDetailsPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [employee, setEmployee] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetch = async () => {
      setLoading(true)
      setError(null)
      try {
        const data = await employeeService.getById(id)
        setEmployee(data)
      } catch (e) {
        setError(e.response?.data?.message || e.message || 'Failed to load employee')
      } finally {
        setLoading(false)
      }
    }
    fetch()
  }, [id])

  if (loading) return <LoadingSpinner message="Loading employee details..." />
  if (error) return <ErrorMessage message={error} onRetry={() => navigate(0)} />
  if (!employee) return null

  const initials = `${employee.firstName?.[0] ?? ''}${employee.lastName?.[0] ?? ''}`.toUpperCase()

  return (
    <div className="space-y-5 max-w-3xl">
      <div className="flex items-center gap-3">
        <button onClick={() => navigate(-1)} className="btn-secondary text-sm py-1.5">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          Back
        </button>
        <h2 className="text-xl font-bold text-gray-800">Employee Details</h2>
      </div>

      <div className="card">
        <div className="flex items-center gap-5 mb-6">
          <div className="w-16 h-16 rounded-full bg-blue-600 flex items-center justify-center text-white text-xl font-bold">
            {initials || '?'}
          </div>
          <div>
            <h3 className="text-xl font-bold text-gray-800">{employee.firstName} {employee.lastName}</h3>
            <p className="text-gray-500 text-sm">{employee.jobTitle ?? employee.job?.title ?? 'Employee'}</p>
            <p className="text-gray-400 text-xs mt-0.5">{employee.departmentName ?? employee.department?.name ?? ''}</p>
          </div>
        </div>

        <dl className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <Detail label="Employee ID" value={employee.id ?? employee.employeeId} />
          <Detail label="Email" value={employee.email} />
          <Detail label="Phone" value={employee.phone} />
          <Detail label="Department" value={employee.departmentName ?? employee.department?.name} />
          <Detail label="Job Title" value={employee.jobTitle ?? employee.job?.title} />
          <Detail label="Salary" value={employee.salary ? `$${Number(employee.salary).toLocaleString()}` : null} />
          <Detail label="Hire Date" value={employee.hireDate ? new Date(employee.hireDate).toLocaleDateString() : null} />
          <Detail label="Manager" value={employee.managerId ? `#${employee.managerId}` : null} />
          <Detail label="Commission" value={employee.commissionPct ? `${(employee.commissionPct * 100).toFixed(1)}%` : null} />
        </dl>
      </div>

      <div className="flex justify-end">
        <Link to="/employees" className="btn-secondary">
          View all employees
        </Link>
      </div>
    </div>
  )
}
