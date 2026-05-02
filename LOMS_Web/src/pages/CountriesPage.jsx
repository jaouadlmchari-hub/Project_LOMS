import { useEffect, useState, useCallback } from 'react'
import { countryService } from '../services/countryService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { countryId: '', countryName: '', regionId: '' }

export default function CountriesPage() {
  const [countries, setCountries] = useState([])
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

  const fetchCountries = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await countryService.getAll()
      setCountries(Array.isArray(data) ? data : data?.data ?? [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load countries')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchCountries() }, [fetchCountries])

  const filtered = countries.filter((c) =>
    (c.countryName ?? c.name ?? '').toLowerCase().includes(search.toLowerCase()) ||
    (c.countryId ?? '').toString().toLowerCase().includes(search.toLowerCase())
  )

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (c) => {
    setEditTarget(c)
    setForm({ countryId: c.countryId ?? '', countryName: c.countryName ?? c.name ?? '', regionId: c.regionId ?? '' })
    setFormError(null); setModalOpen(true)
  }
  const handleFormChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }))

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      if (editTarget) await countryService.update(editTarget.countryId ?? editTarget.id, form)
      else await countryService.create(form)
      setModalOpen(false); fetchCountries()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await countryService.delete(deleteTarget.countryId ?? deleteTarget.id)
      setDeleteTarget(null); fetchCountries()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading countries..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchCountries} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Countries</h2>
          <p className="text-sm text-gray-500">{countries.length} total</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Country
        </button>
      </div>

      <div className="card p-4">
        <input type="text" placeholder="Search countries..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {filtered.length === 0 ? (
          <div className="col-span-full card text-center text-gray-400 py-12">No countries found</div>
        ) : (
          filtered.map((country, i) => (
            <div key={country.countryId ?? country.id ?? i} className="card flex items-center justify-between group">
              <div>
                <p className="font-semibold text-gray-800">{country.countryName ?? country.name}</p>
                <p className="text-xs text-gray-400 mt-0.5 font-mono">{country.countryId ?? country.id} · Region {country.regionId ?? '—'}</p>
              </div>
              <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                <button onClick={() => openEdit(country)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                </button>
                <button onClick={() => setDeleteTarget(country)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit Country' : 'Add Country'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div>
            <label className="label">Country ID (2-letter code)</label>
            <input name="countryId" value={form.countryId} onChange={handleFormChange} required maxLength={2} className="input-field uppercase" placeholder="e.g. US" />
          </div>
          <div>
            <label className="label">Country Name</label>
            <input name="countryName" value={form.countryName} onChange={handleFormChange} required className="input-field" placeholder="e.g. United States" />
          </div>
          <div>
            <label className="label">Region ID</label>
            <input name="regionId" type="number" value={form.regionId} onChange={handleFormChange} className="input-field" placeholder="e.g. 2" />
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
        title="Delete Country"
        message={`Are you sure you want to delete "${deleteTarget?.countryName ?? deleteTarget?.name}"?`}
      />
    </div>
  )
}
