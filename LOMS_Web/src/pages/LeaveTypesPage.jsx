import { useEffect, useState, useCallback } from 'react'
import { leaveTypeService } from '../services/leaveTypeService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { LeaveName: '', MaxDaysPerYear: '', IsPaid: true, RequiresDocument: false, IsActive: true }

export default function LeaveTypesPage() {
  const [types, setTypes] = useState([])
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

  const fetchTypes = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await leaveTypeService.getAll()
      setTypes(Array.isArray(data) ? data : [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load leave types')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchTypes() }, [fetchTypes])

  const filtered = types.filter((t) =>
    (t.LeaveName ?? t.leaveName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (t) => {
    setEditTarget(t)
    setForm({
      LeaveName: t.LeaveName ?? t.leaveName ?? '',
      MaxDaysPerYear: t.MaxDaysPerYear ?? t.maxDaysPerYear ?? '',
      IsPaid: t.IsPaid ?? t.isPaid ?? true,
      RequiresDocument: t.RequiresDocument ?? t.requiresDocument ?? false,
      IsActive: t.IsActive ?? t.isActive ?? true,
    })
    setFormError(null); setModalOpen(true)
  }
  const handleFormChange = (e) => {
    const val = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm((f) => ({ ...f, [e.target.name]: val }))
  }

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      const payload = { ...form, MaxDaysPerYear: Number(form.MaxDaysPerYear) }
      if (editTarget) await leaveTypeService.update(editTarget.LeaveTypeID ?? editTarget.leaveTypeID, payload)
      else await leaveTypeService.create(payload)
      setModalOpen(false); fetchTypes()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await leaveTypeService.delete(deleteTarget.LeaveTypeID ?? deleteTarget.leaveTypeID)
      setDeleteTarget(null); fetchTypes()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading leave types..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchTypes} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Leave Types</h2>
          <p className="text-sm text-gray-500">{types.length} types configured</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Leave Type
        </button>
      </div>

      <div className="card p-4">
        <input type="text" placeholder="Search leave types..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {filtered.length === 0 ? (
          <div className="col-span-full card text-center text-gray-400 py-12">No leave types found</div>
        ) : (
          filtered.map((t, i) => {
            const id = t.LeaveTypeID ?? t.leaveTypeID ?? i
            const name = t.LeaveName ?? t.leaveName ?? '—'
            const maxDays = t.MaxDaysPerYear ?? t.maxDaysPerYear ?? '—'
            const isPaid = t.IsPaid ?? t.isPaid ?? false
            const requiresDoc = t.RequiresDocument ?? t.requiresDocument ?? false
            const isActive = t.IsActive ?? t.isActive ?? true
            return (
              <div key={id} className="card group">
                <div className="flex items-start justify-between mb-3">
                  <div>
                    <h3 className="font-semibold text-gray-800">{name}</h3>
                    <p className="text-xs text-gray-400 mt-0.5">ID: {id}</p>
                  </div>
                  <span className={`badge ${isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
                    {isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
                <div className="space-y-1.5 mb-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Max days/year</span>
                    <span className="font-medium text-gray-800">{maxDays}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Paid leave</span>
                    <span className={`font-medium ${isPaid ? 'text-green-600' : 'text-gray-500'}`}>{isPaid ? 'Yes' : 'No'}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Requires document</span>
                    <span className={`font-medium ${requiresDoc ? 'text-amber-600' : 'text-gray-500'}`}>{requiresDoc ? 'Yes' : 'No'}</span>
                  </div>
                </div>
                <div className="flex justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                  <button onClick={() => openEdit(t)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                  </button>
                  <button onClick={() => setDeleteTarget(t)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                  </button>
                </div>
              </div>
            )
          })
        )}
      </div>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Leave Type' : 'Add Leave Type'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div>
            <label className="label">Leave Name</label>
            <input name="LeaveName" value={form.LeaveName} onChange={handleFormChange} required className="input-field" placeholder="e.g. Annual Leave" />
          </div>
          <div>
            <label className="label">Max Days Per Year</label>
            <input name="MaxDaysPerYear" type="number" value={form.MaxDaysPerYear} onChange={handleFormChange} required className="input-field" placeholder="e.g. 21" />
          </div>
          <div className="space-y-3">
            {[['IsPaid', 'Paid leave'], ['RequiresDocument', 'Requires supporting document'], ['IsActive', 'Active']].map(([name, label]) => (
              <div key={name} className="flex items-center gap-3">
                <input type="checkbox" id={name} name={name} checked={form[name]} onChange={handleFormChange} className="w-4 h-4 rounded border-gray-300 text-blue-600" />
                <label htmlFor={name} className="text-sm text-gray-700">{label}</label>
              </div>
            ))}
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
        title="Delete Leave Type"
        message={`Are you sure you want to delete "${deleteTarget?.LeaveName ?? deleteTarget?.leaveName}"?`}
      />
    </div>
  )
}
