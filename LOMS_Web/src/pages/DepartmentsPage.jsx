import { useEffect, useState, useCallback } from 'react'
import { departmentService } from '../services/departmentService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { departmentName: '', managerId: '', locationId: '' }

export default function DepartmentsPage() {
  const [departments, setDepartments] = useState([])
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

  const fetchDepts = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await departmentService.getAll()
      setDepartments(Array.isArray(data) ? data : data?.data ?? [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load departments')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchDepts() }, [fetchDepts])

  const filtered = departments.filter((d) =>
    (d.departmentName ?? d.name ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (dept) => {
    setEditTarget(dept)
    setForm({ departmentName: dept.departmentName ?? dept.name ?? '', managerId: dept.managerId ?? '', locationId: dept.locationId ?? '' })
    setFormError(null); setModalOpen(true)
  }
  const handleFormChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      if (editTarget) await departmentService.update(editTarget.id ?? editTarget.departmentId, form)
      else await departmentService.create(form)
      setModalOpen(false); fetchDepts()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await departmentService.delete(deleteTarget.id ?? deleteTarget.departmentId)
      setDeleteTarget(null); fetchDepts()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading departments..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchDepts} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Departments</h2>
          <p className="text-sm text-gray-500">{departments.length} total</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Department
        </button>
      </div>

      <div className="card p-4">
        <input type="text" placeholder="Search departments..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="table-head">ID</th>
                <th className="table-head">Department Name</th>
                <th className="table-head hidden md:table-cell">Manager ID</th>
                <th className="table-head hidden lg:table-cell">Location ID</th>
                <th className="table-head">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.length === 0 ? (
                <tr><td colSpan={5} className="table-cell text-center text-gray-400 py-12">No departments found</td></tr>
              ) : (
                filtered.map((dept, i) => (
                  <tr key={dept.id ?? dept.departmentId ?? i} className="hover:bg-gray-50">
                    <td className="table-cell text-gray-400">{dept.id ?? dept.departmentId}</td>
                    <td className="table-cell font-medium text-gray-800">{dept.departmentName ?? dept.name}</td>
                    <td className="table-cell hidden md:table-cell text-gray-500">{dept.managerId ?? '—'}</td>
                    <td className="table-cell hidden lg:table-cell text-gray-500">{dept.locationId ?? '—'}</td>
                    <td className="table-cell">
                      <div className="flex items-center gap-2">
                        <button onClick={() => openEdit(dept)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                        </button>
                        <button onClick={() => setDeleteTarget(dept)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Department' : 'Add Department'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div>
            <label className="label">Department Name</label>
            <input name="departmentName" value={form.departmentName} onChange={handleFormChange} required className="input-field" placeholder="e.g. Engineering" />
          </div>
          <div>
            <label className="label">Manager ID</label>
            <input name="managerId" value={form.managerId} onChange={handleFormChange} className="input-field" placeholder="e.g. 100" />
          </div>
          <div>
            <label className="label">Location ID</label>
            <input name="locationId" value={form.locationId} onChange={handleFormChange} className="input-field" placeholder="e.g. 1700" />
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
        title="Delete Department"
        message={`Are you sure you want to delete "${deleteTarget?.departmentName ?? deleteTarget?.name}"?`}
      />
    </div>
  )
}
