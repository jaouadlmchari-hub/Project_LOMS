import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { employeeService } from '../services/employeeService'
import { departmentService } from '../services/departmentService'
import { jobService } from '../services/jobService'
import { countryService } from '../services/countryService'
import { applicationService } from '../services/applicationService'
import { leaveTypeService } from '../services/leaveTypeService'
import StatCard from '../components/StatCard'
import LoadingSpinner from '../components/LoadingSpinner'

const STATUS_LABELS = { 0: 'Pending', 1: 'Approved', 2: 'Rejected', 3: 'Cancelled' }
const STATUS_COLORS = {
  0: 'bg-yellow-100 text-yellow-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-red-100 text-red-700',
  3: 'bg-gray-100 text-gray-500',
}

export default function DashboardPage() {
  const [stats, setStats] = useState({ employees: null, departments: null, jobs: null, countries: null, applications: null, leaveTypes: null })
  const [recentEmployees, setRecentEmployees] = useState([])
  const [recentApps, setRecentApps] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchAll = async () => {
      setLoading(true)
      const [emp, dept, job, country, apps, leaveTypes] = await Promise.allSettled([
        employeeService.getAll(),
        departmentService.getAll(),
        jobService.getAll(),
        countryService.getAll(),
        applicationService.getAll(),
        leaveTypeService.getAll(),
      ])

      const toArr = (r) => {
        if (r.status !== 'fulfilled') return []
        const d = r.value
        return Array.isArray(d) ? d : d?.data ?? []
      }

      const empArr = toArr(emp)
      const deptArr = toArr(dept)
      const jobArr = toArr(job)
      const countryArr = toArr(country)
      const appsArr = toArr(apps)
      const leaveArr = toArr(leaveTypes)

      setStats({
        employees: empArr.length,
        departments: deptArr.length,
        jobs: jobArr.length,
        countries: countryArr.length,
        applications: appsArr.length,
        leaveTypes: leaveArr.length,
      })
      setRecentEmployees(empArr.slice(0, 5))
      setRecentApps(appsArr.slice(0, 5))
      setLoading(false)
    }
    fetchAll()
  }, [])

  if (loading) return <LoadingSpinner message="Loading dashboard..." />

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-800">Welcome back</h2>
        <p className="text-gray-500 text-sm mt-0.5">Overview of your HR data from the LOMS microservices.</p>
      </div>

      <div className="grid grid-cols-2 xl:grid-cols-3 gap-4">
        <StatCard title="Employees" value={stats.employees} color="blue" subtitle="Active workforce"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" /></svg>}
        />
        <StatCard title="Departments" value={stats.departments} color="purple" subtitle="Org units"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" /></svg>}
        />
        <StatCard title="Job Positions" value={stats.jobs} color="green" subtitle="Available roles"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>}
        />
        <StatCard title="Applications" value={stats.applications} color="orange" subtitle="All requests"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" /></svg>}
        />
        <StatCard title="Leave Types" value={stats.leaveTypes} color="blue" subtitle="Configured types"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" /></svg>}
        />
        <StatCard title="Countries" value={stats.countries} color="green" subtitle="Global presence"
          icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
        />
      </div>

      <div className="grid grid-cols-1 xl:grid-cols-2 gap-5">
        <div className="card">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-semibold text-gray-800">Recent Employees</h3>
            <Link to="/employees" className="text-sm text-blue-600 hover:underline">View all →</Link>
          </div>
          {recentEmployees.length === 0 ? (
            <p className="text-sm text-gray-400 text-center py-8">No employee data available</p>
          ) : (
            <div className="space-y-2">
              {recentEmployees.map((emp, i) => {
                const firstName = emp.FirstName ?? emp.firstName ?? ''
                const lastName = emp.LastName ?? emp.lastName ?? ''
                const id = emp.EmployeeID ?? emp.employeeID ?? emp.id ?? i
                const dept = emp.DepartmentName ?? emp.departmentName ?? emp.Department ?? '—'
                const job = emp.JobTitle ?? emp.jobTitle ?? emp.Job ?? '—'
                return (
                  <div key={id} className="flex items-center gap-3 p-2.5 rounded-lg hover:bg-gray-50">
                    <div className="w-8 h-8 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-xs font-bold flex-shrink-0">
                      {firstName[0]}{lastName[0]}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-800 truncate">{firstName} {lastName}</p>
                      <p className="text-xs text-gray-400 truncate">{job} · {dept}</p>
                    </div>
                    <Link to={`/employees/${id}`} className="text-xs text-blue-500 hover:underline flex-shrink-0">View</Link>
                  </div>
                )
              })}
            </div>
          )}
        </div>

        <div className="card">
          <div className="flex items-center justify-between mb-4">
            <h3 className="font-semibold text-gray-800">Recent Applications</h3>
            <Link to="/applications" className="text-sm text-blue-600 hover:underline">View all →</Link>
          </div>
          {recentApps.length === 0 ? (
            <p className="text-sm text-gray-400 text-center py-8">No application data available</p>
          ) : (
            <div className="space-y-2">
              {recentApps.map((app, i) => {
                const appId = app.ApplicationID ?? app.applicationID ?? i
                const empId = app.EmployeeID ?? app.employeeID ?? '—'
                const status = app.Status ?? app.status ?? 0
                const typeId = app.ApplicationTypeID ?? app.applicationTypeID ?? '—'
                const date = app.CreatedDate ?? app.createdDate
                return (
                  <div key={appId} className="flex items-center gap-3 p-2.5 rounded-lg hover:bg-gray-50">
                    <div className="w-8 h-8 rounded-full bg-slate-100 text-slate-600 flex items-center justify-center text-xs font-mono font-bold flex-shrink-0">
                      #{appId}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-800">Employee #{empId} · Type {typeId}</p>
                      <p className="text-xs text-gray-400">{date ? new Date(date).toLocaleDateString() : '—'}</p>
                    </div>
                    <span className={`badge flex-shrink-0 ${STATUS_COLORS[status] ?? STATUS_COLORS[0]}`}>
                      {STATUS_LABELS[status] ?? 'Unknown'}
                    </span>
                  </div>
                )
              })}
            </div>
          )}
        </div>
      </div>

      <div className="card">
        <h3 className="font-semibold text-gray-800 mb-4">Quick Navigation</h3>
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
          {[
            { label: 'Employees', to: '/employees', color: 'blue' },
            { label: 'Departments', to: '/departments', color: 'purple' },
            { label: 'Applications', to: '/applications', color: 'orange' },
            { label: 'Leave Types', to: '/leave-types', color: 'green' },
            { label: 'Holidays', to: '/holidays', color: 'blue' },
            { label: 'Users', to: '/users', color: 'purple' },
          ].map((link) => (
            <Link
              key={link.to}
              to={link.to}
              className="flex flex-col items-center gap-2 p-3 rounded-xl border border-gray-200 hover:border-blue-300 hover:bg-blue-50 transition-colors text-center group"
            >
              <span className="text-sm font-medium text-gray-700 group-hover:text-blue-700">{link.label}</span>
            </Link>
          ))}
        </div>
      </div>
    </div>
  )
}
