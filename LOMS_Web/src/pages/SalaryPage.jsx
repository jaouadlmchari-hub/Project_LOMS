import { useState } from 'react'
import { salaryService } from '../services/salaryService'
import LoadingSpinner from '../components/LoadingSpinner'
import Modal from '../components/Modal'

export default function SalaryPage() {
  const [searchType, setSearchType] = useState('employee')
  const [searchValue, setSearchValue] = useState('')
  const [salary, setSalary] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [searched, setSearched] = useState(false)
  const [addModalOpen, setAddModalOpen] = useState(false)
  const [addForm, setAddForm] = useState({ EmployeeID: '', Salary: '', CreatedByUserID: '' })
  const [saving, setSaving] = useState(false)
  const [addError, setAddError] = useState(null)

  const handleSearch = async (e) => {
    e.preventDefault()
    if (!searchValue) return
    setLoading(true); setError(null); setSearched(true); setSalary(null)
    try {
      let data
      if (searchType === 'employee') data = await salaryService.getByEmployeeId(searchValue)
      else data = await salaryService.getByNationalNo(searchValue)
      setSalary(data)
    } catch (err) {
      if (err.response?.status === 404) setError('No salary record found.')
      else setError(err.response?.data?.message || err.message || 'Search failed')
    } finally { setLoading(false) }
  }

  const handleAddSalary = async (e) => {
    e.preventDefault(); setSaving(true); setAddError(null)
    try {
      await salaryService.add({ EmployeeID: Number(addForm.EmployeeID), Salary: Number(addForm.Salary), CreatedByUserID: Number(addForm.CreatedByUserID) })
      setAddModalOpen(false)
      setAddForm({ EmployeeID: '', Salary: '', CreatedByUserID: '' })
    } catch (err) { setAddError(err.response?.data?.message || err.message || 'Failed to add salary') }
    finally { setSaving(false) }
  }

  const fieldMap = salary ? Object.entries(salary) : []

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Salary Records</h2>
          <p className="text-sm text-gray-500">Look up salary information by employee ID or national number</p>
        </div>
        <button onClick={() => setAddModalOpen(true)} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add Salary
        </button>
      </div>

      <div className="card">
        <form onSubmit={handleSearch} className="flex flex-wrap gap-4 items-end">
          <div>
            <label className="label">Search By</label>
            <select value={searchType} onChange={(e) => { setSearchType(e.target.value); setSearchValue(''); setSalary(null); setSearched(false) }} className="input-field w-44">
              <option value="employee">Employee ID</option>
              <option value="national">National No.</option>
            </select>
          </div>
          <div>
            <label className="label">{searchType === 'employee' ? 'Employee ID' : 'National Number'}</label>
            <input
              type={searchType === 'employee' ? 'number' : 'text'}
              value={searchValue}
              onChange={(e) => setSearchValue(e.target.value)}
              required
              className="input-field w-48"
              placeholder={searchType === 'employee' ? 'e.g. 5' : 'e.g. AB123456'}
            />
          </div>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Searching...' : 'Search'}
          </button>
        </form>
      </div>

      {loading && <LoadingSpinner message="Fetching salary record..." />}

      {!loading && searched && error && (
        <div className="card border-red-200 bg-red-50">
          <p className="text-red-600 text-sm">{error}</p>
        </div>
      )}

      {salary && !loading && (
        <div className="card max-w-lg">
          <h3 className="font-semibold text-gray-800 mb-4">Salary Record</h3>
          <dl className="space-y-3">
            {fieldMap.map(([key, value]) => (
              <div key={key} className="flex justify-between text-sm border-b border-gray-100 pb-2 last:border-0">
                <dt className="text-gray-500 capitalize">{key.replace(/([A-Z])/g, ' $1').trim()}</dt>
                <dd className="font-medium text-gray-800">
                  {key.toLowerCase().includes('salary') && typeof value === 'number'
                    ? `$${Number(value).toLocaleString()}`
                    : value?.toString() ?? '—'}
                </dd>
              </div>
            ))}
          </dl>
        </div>
      )}

      {!loading && searched && !salary && !error && (
        <div className="card text-center py-12 text-gray-400">No salary record found</div>
      )}

      <Modal open={addModalOpen} onClose={() => setAddModalOpen(false)} title="Add Salary Record">
        <form onSubmit={handleAddSalary} className="space-y-4">
          {addError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{addError}</p>}
          <div>
            <label className="label">Employee ID</label>
            <input type="number" value={addForm.EmployeeID} onChange={(e) => setAddForm((f) => ({ ...f, EmployeeID: e.target.value }))} required className="input-field" placeholder="e.g. 5" />
          </div>
          <div>
            <label className="label">Salary</label>
            <input type="number" step="0.01" value={addForm.Salary} onChange={(e) => setAddForm((f) => ({ ...f, Salary: e.target.value }))} required className="input-field" placeholder="e.g. 50000" />
          </div>
          <div>
            <label className="label">Created By User ID</label>
            <input type="number" value={addForm.CreatedByUserID} onChange={(e) => setAddForm((f) => ({ ...f, CreatedByUserID: e.target.value }))} required className="input-field" placeholder="e.g. 1" />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <button type="button" onClick={() => setAddModalOpen(false)} className="btn-secondary" disabled={saving}>Cancel</button>
            <button type="submit" className="btn-primary" disabled={saving}>{saving ? 'Adding...' : 'Add Salary'}</button>
          </div>
        </form>
      </Modal>
    </div>
  )
}
