import { useEffect, useState, useCallback } from 'react'
import { applicationService } from '../services/applicationService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const STATUS_LABELS = { 0: 'Pending', 1: 'Approved', 2: 'Rejected', 3: 'Cancelled' }
const STATUS_COLORS = {
  0: 'bg-yellow-100 text-yellow-700',
  1: 'bg-green-100 text-green-700',
  2: 'bg-red-100 text-red-700',
  3: 'bg-gray-100 text-gray-500',
}

const EMPTY_FORM = { EmployeeID: '', ApplicationTypeID: '', CreatedByUserID: '', Notes: '' }

export default function ApplicationsPage() {
  const [apps, setApps] = useState([])
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
  const [statusModalOpen, setStatusModalOpen] = useState(false)
  const [statusTarget, setStatusTarget] = useState(null)
  const [newStatus, setNewStatus] = useState(0)

  const fetchApps = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await applicationService.getAll()
      setApps(Array.isArray(data) ? data : data?.data ?? [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load applications')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchApps() }, [fetchApps])

  const filtered = apps.filter((a) => {
    const q = search.toLowerCase()
    const id = (a.ApplicationID ?? a.applicationID ?? '').toString()
    const emp = (a.EmployeeID ?? a.employeeID ?? '').toString()
    const notes = (a.Notes ?? a.notes ?? '').toLowerCase()
    return id.includes(q) || emp.includes(q) || notes.includes(q)
  })

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (a) => {
    setEditTarget(a)
    setForm({ EmployeeID: a.EmployeeID ?? '', ApplicationTypeID: a.ApplicationTypeID ?? '', CreatedByUserID: a.CreatedByUserID ?? '', Notes: a.Notes ?? '', Status: a.Status ?? 0, ActionByUserID: a.ActionByUserID ?? '' })
    setFormError(null); setModalOpen(true)
  }
  const openStatusModal = (a) => { setStatusTarget(a); setNewStatus(a.Status ?? 0); setStatusModalOpen(true) }
  const handleFormChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      if (editTarget) await applicationService.update(editTarget.ApplicationID ?? editTarget.applicationID, form)
      else await applicationService.create(form)
      setModalOpen(false); fetchApps()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleUpdateStatus = async () => {
    if (!statusTarget) return; setSaving(true)
    try {
      await applicationService.updateStatus(statusTarget.ApplicationID ?? statusTarget.applicationID, newStatus)
      setStatusModalOpen(false); fetchApps()
    } catch (err) { alert(err.response?.data?.message || 'Failed to update status') }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await applicationService.delete(deleteTarget.ApplicationID ?? deleteTarget.applicationID)
      setDeleteTarget(null); fetchApps()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading applications..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchApps} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Applications</h2>
          <p className="text-sm text-gray-500">{apps.length} total</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          New Application
        </button>
      </div>

      <div className="card p-4">
        <input type="text" placeholder="Search by ID, employee or notes..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="table-head">App ID</th>
                <th className="table-head">Employee ID</th>
                <th className="table-head hidden md:table-cell">Type ID</th>
                <th className="table-head hidden lg:table-cell">Notes</th>
                <th className="table-head">Status</th>
                <th className="table-head hidden lg:table-cell">Date</th>
                <th className="table-head">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.length === 0 ? (
                <tr><td colSpan={7} className="table-cell text-center text-gray-400 py-12">No applications found</td></tr>
              ) : (
                filtered.map((a, i) => {
                  const id = a.ApplicationID ?? a.applicationID ?? i
                  const status = a.Status ?? a.status ?? 0
                  return (
                    <tr key={id} className="hover:bg-gray-50">
                      <td className="table-cell font-mono text-xs text-gray-500">#{id}</td>
                      <td className="table-cell text-gray-800">{a.EmployeeID ?? a.employeeID ?? '—'}</td>
                      <td className="table-cell hidden md:table-cell text-gray-500">{a.ApplicationTypeID ?? a.applicationTypeID ?? '—'}</td>
                      <td className="table-cell hidden lg:table-cell text-gray-500 max-w-[200px] truncate">{a.Notes ?? a.notes ?? '—'}</td>
                      <td className="table-cell">
                        <span className={`badge ${STATUS_COLORS[status] ?? STATUS_COLORS[0]}`}>
                          {STATUS_LABELS[status] ?? 'Unknown'}
                        </span>
                      </td>
                      <td className="table-cell hidden lg:table-cell text-gray-400 text-xs">
                        {a.CreatedDate ?? a.createdDate ? new Date(a.CreatedDate ?? a.createdDate).toLocaleDateString() : '—'}
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center gap-2">
                          <button onClick={() => openStatusModal(a)} title="Update Status" className="p-1.5 text-gray-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                          </button>
                          <button onClick={() => openEdit(a)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                          </button>
                          <button onClick={() => setDeleteTarget(a)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                          </button>
                        </div>
                      </td>
                    </tr>
                  )
                })
              )}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Application' : 'New Application'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Employee ID</label>
              <input name="EmployeeID" type="number" value={form.EmployeeID} onChange={handleFormChange} required className="input-field" placeholder="e.g. 5" />
            </div>
            <div>
              <label className="label">Application Type ID</label>
              <input name="ApplicationTypeID" type="number" value={form.ApplicationTypeID} onChange={handleFormChange} required className="input-field" placeholder="e.g. 1" />
            </div>
          </div>
          <div>
            <label className="label">Created By User ID</label>
            <input name="CreatedByUserID" type="number" value={form.CreatedByUserID} onChange={handleFormChange} required className="input-field" placeholder="e.g. 1" />
          </div>
          {editTarget && (
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="label">Status</label>
                <select name="Status" value={form.Status} onChange={handleFormChange} className="input-field">
                  {Object.entries(STATUS_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
                </select>
              </div>
              <div>
                <label className="label">Action By User ID</label>
                <input name="ActionByUserID" type="number" value={form.ActionByUserID} onChange={handleFormChange} className="input-field" placeholder="e.g. 1" />
              </div>
            </div>
          )}
          <div>
            <label className="label">Notes</label>
            <textarea name="Notes" value={form.Notes} onChange={handleFormChange} rows={3} className="input-field resize-none" placeholder="Additional notes..." />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setModalOpen(false)} className="btn-secondary" disabled={saving}>Cancel</button>
            <button type="submit" className="btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
          </div>
        </form>
      </Modal>

      <Modal open={statusModalOpen} onClose={() => setStatusModalOpen(false)} title="Update Application Status">
        <div className="space-y-4">
          <p className="text-sm text-gray-600">Application <span className="font-semibold">#{statusTarget?.ApplicationID ?? statusTarget?.applicationID}</span></p>
          <div>
            <label className="label">New Status</label>
            <select value={newStatus} onChange={(e) => setNewStatus(Number(e.target.value))} className="input-field">
              {Object.entries(STATUS_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
            </select>
          </div>
          <div className="flex justify-end gap-3">
            <button onClick={() => setStatusModalOpen(false)} className="btn-secondary" disabled={saving}>Cancel</button>
            <button onClick={handleUpdateStatus} className="btn-primary" disabled={saving}>{saving ? 'Updating...' : 'Update Status'}</button>
          </div>
        </div>
      </Modal>

      <ConfirmDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        loading={deleteLoading}
        title="Delete Application"
        message={`Are you sure you want to delete application #${deleteTarget?.ApplicationID ?? deleteTarget?.applicationID}?`}
      />
    </div>
  )
}
