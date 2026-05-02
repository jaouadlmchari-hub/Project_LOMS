import { useEffect, useState, useCallback } from 'react'
import { Link } from 'react-router-dom'
import { employeeService } from '../services/employeeService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { firstName: '', lastName: '', email: '', phone: '', salary: '', departmentId: '', jobId: '' }

export default function EmployeesPage() {
  const [employees, setEmployees] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [search, setSearch] = useState('')
  const [deleteTarget, setDeleteTarget] = useState(null)
  const [deleteLoading, setDeleteLoading] = useState(false)
  const [modalOpen, setModalOpen] = useState(false)
  const [editTarget, setEditTarget] = useState(null)
  const [form, setForm] = useState(EMPTY_FORM)
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState(null)

  const fetchEmployees = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await employeeService.getAll()
      const arr = Array.isArray(data) ? data : data?.data ?? []
      setEmployees(arr)
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load employees')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchEmployees() }, [fetchEmployees])

  const filtered = employees.filter((e) => {
    const q = search.toLowerCase()
    return (
      `${e.firstName} ${e.lastName}`.toLowerCase().includes(q) ||
      e.email?.toLowerCase().includes(q) ||
      e.departmentName?.toLowerCase().includes(q) ||
      e.jobTitle?.toLowerCase().includes(q)
    )
  })

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (emp) => {
    setEditTarget(emp)
    setForm({
      firstName: emp.firstName ?? '',
      lastName: emp.lastName ?? '',
      email: emp.email ?? '',
      phone: emp.phone ?? '',
      salary: emp.salary ?? '',
      departmentId: emp.departmentId ?? '',
      jobId: emp.jobId ?? '',
    })
    setFormError(null)
    setModalOpen(true)
  }

  const handleFormChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const handleSave = async (e) => {
    e.preventDefault()
    setSaving(true)
    setFormError(null)
    try {
      if (editTarget) {
        await employeeService.update(editTarget.id ?? editTarget.employeeId, form)
      } else {
        await employeeService.create(form)
      }
      setModalOpen(false)
      fetchEmployees()
    } catch (err) {
      setFormError(err.response?.data?.message || err.message || 'Save failed')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return
    setDeleteLoading(true)
    try {
      await employeeService.delete(deleteTarget.id ?? deleteTarget.employeeId)
      setDeleteTarget(null)
      fetchEmployees()
    } catch (err) {
      alert(err.response?.data?.message || err.message || 'Delete failed')
    } finally {
      setDeleteLoading(false)
    }
  }

  if (loading) return <LoadingSpinner message="Loading employees..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchEmployees} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Employees</h2>
          <p className="text-sm text-gray-500">{employees.length} total records</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Employee
        </button>
      </div>

      <div className="card p-4">
        <input
          type="text"
          placeholder="Search by name, email, department..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="input-field max-w-sm"
        />
      </div>

      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="table-head">Name</th>
                <th className="table-head">Email</th>
                <th className="table-head hidden md:table-cell">Phone</th>
                <th className="table-head hidden lg:table-cell">Department</th>
                <th className="table-head hidden lg:table-cell">Job</th>
                <th className="table-head hidden xl:table-cell">Salary</th>
                <th className="table-head">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.length === 0 ? (
                <tr>
                  <td colSpan={7} className="table-cell text-center text-gray-400 py-12">
                    No employees found
                  </td>
                </tr>
              ) : (
                filtered.map((emp, i) => (
                  <tr key={emp.id ?? emp.employeeId ?? i} className="hover:bg-gray-50">
                    <td className="table-cell font-medium text-gray-800">
                      <Link to={`/employees/${emp.id ?? emp.employeeId}`} className="hover:text-blue-600 hover:underline">
                        {emp.firstName} {emp.lastName}
                      </Link>
                    </td>
                    <td className="table-cell text-gray-500">{emp.email ?? '—'}</td>
                    <td className="table-cell hidden md:table-cell text-gray-500">{emp.phone ?? '—'}</td>
                    <td className="table-cell hidden lg:table-cell">{emp.departmentName ?? emp.department?.name ?? '—'}</td>
                    <td className="table-cell hidden lg:table-cell">{emp.jobTitle ?? emp.job?.title ?? '—'}</td>
                    <td className="table-cell hidden xl:table-cell">{emp.salary ? `$${Number(emp.salary).toLocaleString()}` : '—'}</td>
                    <td className="table-cell">
                      <div className="flex items-center gap-2">
                        <button onClick={() => openEdit(emp)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors" title="Edit">
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                          </svg>
                        </button>
                        <button onClick={() => setDeleteTarget(emp)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors" title="Delete">
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                          </svg>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Employee' : 'Add Employee'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">First Name</label>
              <input name="firstName" value={form.firstName} onChange={handleFormChange} required className="input-field" placeholder="John" />
            </div>
            <div>
              <label className="label">Last Name</label>
              <input name="lastName" value={form.lastName} onChange={handleFormChange} required className="input-field" placeholder="Doe" />
            </div>
          </div>
          <div>
            <label className="label">Email</label>
            <input name="email" type="email" value={form.email} onChange={handleFormChange} className="input-field" placeholder="john.doe@company.com" />
          </div>
          <div>
            <label className="label">Phone</label>
            <input name="phone" value={form.phone} onChange={handleFormChange} className="input-field" placeholder="+1 234 567 890" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Department ID</label>
              <input name="departmentId" value={form.departmentId} onChange={handleFormChange} className="input-field" placeholder="e.g. 10" />
            </div>
            <div>
              <label className="label">Job ID</label>
              <input name="jobId" value={form.jobId} onChange={handleFormChange} className="input-field" placeholder="e.g. 5" />
            </div>
          </div>
          <div>
            <label className="label">Salary</label>
            <input name="salary" type="number" value={form.salary} onChange={handleFormChange} className="input-field" placeholder="50000" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setModalOpen(false)} className="btn-secondary" disabled={saving}>Cancel</button>
            <button type="submit" className="btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        loading={deleteLoading}
        title="Delete Employee"
        message={`Are you sure you want to delete ${deleteTarget?.firstName} ${deleteTarget?.lastName}? This action cannot be undone.`}
      />
    </div>
  )
}
