import { useState } from 'react'
import { leaveBalanceService } from '../services/leaveBalanceService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import Modal from '../components/Modal'

export default function LeaveBalancesPage() {
  const [employeeID, setEmployeeID] = useState('')
  const [year, setYear] = useState(new Date().getFullYear())
  const [balances, setBalances] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [searched, setSearched] = useState(false)
  const [initLoading, setInitLoading] = useState(false)
  const [editTarget, setEditTarget] = useState(null)
  const [editForm, setEditForm] = useState({ EntitledDays: '', UsedDays: '' })
  const [saving, setSaving] = useState(false)

  const handleSearch = async (e) => {
    e.preventDefault()
    if (!employeeID) return
    setLoading(true); setError(null); setSearched(true)
    try {
      const data = await leaveBalanceService.getByEmployeeAndYear(employeeID, year)
      setBalances(Array.isArray(data) ? data : [])
    } catch (err) {
      if (err.response?.status === 404) setBalances([])
      else setError(err.response?.data?.message || err.message || 'Failed to fetch balances')
    } finally { setLoading(false) }
  }

  const handleInitialize = async () => {
    if (!employeeID || !year) return
    setInitLoading(true)
    try {
      await leaveBalanceService.initialize(employeeID, year)
      await handleSearch({ preventDefault: () => {} })
    } catch (err) { alert(err.response?.data?.message || 'Initialization failed — balances may already exist.') }
    finally { setInitLoading(false) }
  }

  const openEdit = (b) => {
    setEditTarget(b)
    setEditForm({ EntitledDays: b.EntitledDays ?? b.entitledDays ?? '', UsedDays: b.UsedDays ?? b.usedDays ?? '', LeaveBalanceID: b.LeaveBalanceID ?? b.leaveBalanceID })
  }

  const handleEditSave = async (e) => {
    e.preventDefault(); setSaving(true)
    try {
      const id = editTarget.LeaveBalanceID ?? editTarget.leaveBalanceID
      await leaveBalanceService.updateBalance(id, { ...editTarget, ...editForm, EntitledDays: Number(editForm.EntitledDays), UsedDays: Number(editForm.UsedDays), LeaveBalanceID: id })
      setEditTarget(null)
      await handleSearch({ preventDefault: () => {} })
    } catch (err) { alert(err.response?.data?.message || 'Update failed') }
    finally { setSaving(false) }
  }

  const progressPct = (used, entitled) => {
    if (!entitled || entitled === 0) return 0
    return Math.min(100, Math.round((used / entitled) * 100))
  }

  return (
    <div className="space-y-5">
      <div>
        <h2 className="text-xl font-bold text-gray-800">Leave Balances</h2>
        <p className="text-sm text-gray-500">Query leave balances by employee and year</p>
      </div>

      <div className="card">
        <form onSubmit={handleSearch} className="flex flex-wrap gap-4 items-end">
          <div>
            <label className="label">Employee ID</label>
            <input type="number" value={employeeID} onChange={(e) => setEmployeeID(e.target.value)} required className="input-field w-40" placeholder="e.g. 5" />
          </div>
          <div>
            <label className="label">Year</label>
            <input type="number" value={year} onChange={(e) => setYear(Number(e.target.value))} required className="input-field w-28" min="2000" max="2100" />
          </div>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Searching...' : 'Search'}
          </button>
          {searched && (
            <button type="button" onClick={handleInitialize} disabled={initLoading} className="btn-secondary">
              {initLoading ? 'Initializing...' : 'Initialize Balances'}
            </button>
          )}
        </form>
      </div>

      {loading && <LoadingSpinner message="Fetching balances..." />}
      {error && <ErrorMessage message={error} />}

      {!loading && searched && balances.length === 0 && !error && (
        <div className="card text-center py-12">
          <p className="text-gray-400 mb-2">No balances found for Employee #{employeeID} in {year}</p>
          <p className="text-sm text-gray-400">Use "Initialize Balances" to create them for this year.</p>
        </div>
      )}

      {balances.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4">
          {balances.map((b, i) => {
            const id = b.LeaveBalanceID ?? b.leaveBalanceID ?? i
            const entitled = Number(b.EntitledDays ?? b.entitledDays ?? 0)
            const used = Number(b.UsedDays ?? b.usedDays ?? 0)
            const remaining = Number(b.RemainingDays ?? b.remainingDays ?? (entitled - used))
            const pct = progressPct(used, entitled)
            return (
              <div key={id} className="card group">
                <div className="flex items-start justify-between mb-3">
                  <div>
                    <p className="text-xs text-gray-400 font-mono">Type #{b.LeaveTypeID ?? b.leaveTypeID}</p>
                    <p className="text-sm font-semibold text-gray-800 mt-0.5">Balance ID: {id}</p>
                  </div>
                  <button onClick={() => openEdit(b)} className="opacity-0 group-hover:opacity-100 p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-all">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                  </button>
                </div>
                <div className="space-y-2 mb-3">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Entitled</span><span className="font-medium">{entitled} days</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Used</span><span className="font-medium text-amber-600">{used} days</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Remaining</span><span className={`font-semibold ${remaining <= 0 ? 'text-red-600' : 'text-green-600'}`}>{remaining} days</span>
                  </div>
                </div>
                <div className="w-full bg-gray-100 rounded-full h-2">
                  <div className={`h-2 rounded-full transition-all ${pct >= 90 ? 'bg-red-500' : pct >= 70 ? 'bg-amber-500' : 'bg-green-500'}`} style={{ width: `${pct}%` }} />
                </div>
                <p className="text-xs text-gray-400 mt-1 text-right">{pct}% used</p>
              </div>
            )
          })}
        </div>
      )}

      <Modal open={Boolean(editTarget)} onClose={() => setEditTarget(null)} title="Edit Leave Balance">
        <form onSubmit={handleEditSave} className="space-y-4">
          <div>
            <label className="label">Entitled Days</label>
            <input type="number" step="0.5" value={editForm.EntitledDays} onChange={(e) => setEditForm((f) => ({ ...f, EntitledDays: e.target.value }))} required className="input-field" />
          </div>
          <div>
            <label className="label">Used Days</label>
            <input type="number" step="0.5" value={editForm.UsedDays} onChange={(e) => setEditForm((f) => ({ ...f, UsedDays: e.target.value }))} required className="input-field" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setEditTarget(null)} className="btn-secondary" disabled={saving}>Cancel</button>
            <button type="submit" className="btn-primary" disabled={saving}>{saving ? 'Saving...' : 'Save'}</button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
