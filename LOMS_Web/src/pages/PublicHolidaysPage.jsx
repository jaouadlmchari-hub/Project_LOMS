import { useEffect, useState, useCallback } from 'react'
import { publicHolidayService } from '../services/publicHolidayService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { HolidayName: '', HolidayDate: '', IsRepeatedAnnually: false }

export default function PublicHolidaysPage() {
  const [holidays, setHolidays] = useState([])
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

  const fetchHolidays = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await publicHolidayService.getAll()
      setHolidays(Array.isArray(data) ? data : [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load public holidays')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchHolidays() }, [fetchHolidays])

  const filtered = holidays.filter((h) =>
    (h.HolidayName ?? h.holidayName ?? '').toLowerCase().includes(search.toLowerCase())
  )

  const sorted = [...filtered].sort((a, b) => {
    const da = new Date(a.HolidayDate ?? a.holidayDate)
    const db = new Date(b.HolidayDate ?? b.holidayDate)
    return da - db
  })

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (h) => {
    setEditTarget(h)
    const dateStr = h.HolidayDate ?? h.holidayDate ?? ''
    const formatted = dateStr ? new Date(dateStr).toISOString().split('T')[0] : ''
    setForm({ HolidayName: h.HolidayName ?? h.holidayName ?? '', HolidayDate: formatted, IsRepeatedAnnually: h.IsRepeatedAnnually ?? h.isRepeatedAnnually ?? false })
    setFormError(null); setModalOpen(true)
  }
  const handleFormChange = (e) => {
    const val = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm((f) => ({ ...f, [e.target.name]: val }))
  }

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      if (editTarget) await publicHolidayService.update(editTarget.HolidayID ?? editTarget.holidayID, form)
      else await publicHolidayService.create(form)
      setModalOpen(false); fetchHolidays()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await publicHolidayService.delete(deleteTarget.HolidayID ?? deleteTarget.holidayID)
      setDeleteTarget(null); fetchHolidays()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading public holidays..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchHolidays} />

  const today = new Date()
  const upcoming = sorted.filter((h) => new Date(h.HolidayDate ?? h.holidayDate) >= today).slice(0, 3)

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Public Holidays</h2>
          <p className="text-sm text-gray-500">{holidays.length} holidays configured</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Holiday
        </button>
      </div>

      {upcoming.length > 0 && (
        <div className="card bg-blue-50 border-blue-200">
          <h3 className="text-sm font-semibold text-blue-800 mb-3">Upcoming Holidays</h3>
          <div className="flex flex-wrap gap-3">
            {upcoming.map((h, i) => (
              <div key={i} className="bg-white rounded-lg px-4 py-2 border border-blue-200 text-sm">
                <p className="font-medium text-gray-800">{h.HolidayName ?? h.holidayName}</p>
                <p className="text-blue-600 text-xs">{new Date(h.HolidayDate ?? h.holidayDate).toLocaleDateString()}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      <div className="card p-4">
        <input type="text" placeholder="Search holidays..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="table-head">ID</th>
                <th className="table-head">Holiday Name</th>
                <th className="table-head">Date</th>
                <th className="table-head hidden md:table-cell">Recurring</th>
                <th className="table-head">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {sorted.length === 0 ? (
                <tr><td colSpan={5} className="table-cell text-center text-gray-400 py-12">No holidays found</td></tr>
              ) : (
                sorted.map((h, i) => {
                  const id = h.HolidayID ?? h.holidayID ?? i
                  const name = h.HolidayName ?? h.holidayName ?? '—'
                  const date = h.HolidayDate ?? h.holidayDate
                  const recurring = h.IsRepeatedAnnually ?? h.isRepeatedAnnually ?? false
                  const isPast = date && new Date(date) < today
                  return (
                    <tr key={id} className={`hover:bg-gray-50 ${isPast ? 'opacity-60' : ''}`}>
                      <td className="table-cell text-gray-400">{id}</td>
                      <td className="table-cell font-medium text-gray-800">{name}</td>
                      <td className="table-cell text-gray-700">
                        {date ? new Date(date).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' }) : '—'}
                      </td>
                      <td className="table-cell hidden md:table-cell">
                        {recurring ? (
                          <span className="badge bg-purple-100 text-purple-700">Annual</span>
                        ) : (
                          <span className="badge bg-gray-100 text-gray-500">One-time</span>
                        )}
                      </td>
                      <td className="table-cell">
                        <div className="flex items-center gap-2">
                          <button onClick={() => openEdit(h)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                          </button>
                          <button onClick={() => setDeleteTarget(h)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Holiday' : 'Add Holiday'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div>
            <label className="label">Holiday Name</label>
            <input name="HolidayName" value={form.HolidayName} onChange={handleFormChange} required className="input-field" placeholder="e.g. New Year's Day" />
          </div>
          <div>
            <label className="label">Date</label>
            <input name="HolidayDate" type="date" value={form.HolidayDate} onChange={handleFormChange} required className="input-field" />
          </div>
          <div className="flex items-center gap-3">
            <input type="checkbox" id="IsRepeatedAnnually" name="IsRepeatedAnnually" checked={form.IsRepeatedAnnually} onChange={handleFormChange} className="w-4 h-4 rounded border-gray-300 text-blue-600" />
            <label htmlFor="IsRepeatedAnnually" className="text-sm text-gray-700">Repeated annually</label>
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
        title="Delete Holiday"
        message={`Are you sure you want to delete "${deleteTarget?.HolidayName ?? deleteTarget?.holidayName}"?`}
      />
    </div>
  )
}
