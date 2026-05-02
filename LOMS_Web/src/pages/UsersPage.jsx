import { useEffect, useState, useCallback } from 'react'
import { userService } from '../services/userService'
import LoadingSpinner from '../components/LoadingSpinner'
import ErrorMessage from '../components/ErrorMessage'
import ConfirmDialog from '../components/ConfirmDialog'
import Modal from '../components/Modal'

const EMPTY_FORM = { UserName: '', Password: '', IsActive: true, RoleIDs: [] }

function StatusBadge({ active }) {
  return (
    <span className={`badge ${active ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-500'}`}>
      {active ? 'Active' : 'Inactive'}
    </span>
  )
}

export default function UsersPage() {
  const [users, setUsers] = useState([])
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

  const fetchUsers = useCallback(async () => {
    setLoading(true); setError(null)
    try {
      const data = await userService.getAll()
      setUsers(Array.isArray(data) ? data : data?.data ?? [])
    } catch (e) {
      setError(e.response?.data?.message || e.message || 'Failed to load users')
    } finally { setLoading(false) }
  }, [])

  useEffect(() => { fetchUsers() }, [fetchUsers])

  const filtered = users.filter((u) => {
    const q = search.toLowerCase()
    return (
      (u.UserName ?? u.userName ?? '').toLowerCase().includes(q) ||
      (u.EmployeeFullName ?? u.employeeFullName ?? '').toLowerCase().includes(q)
    )
  })

  const openCreate = () => { setEditTarget(null); setForm(EMPTY_FORM); setFormError(null); setModalOpen(true) }
  const openEdit = (u) => {
    setEditTarget(u)
    setForm({ UserName: u.UserName ?? u.userName ?? '', Password: '', IsActive: u.IsActive ?? u.isActive ?? true, RoleIDs: u.RoleIDs ?? [] })
    setFormError(null); setModalOpen(true)
  }
  const handleFormChange = (e) => {
    const val = e.target.type === 'checkbox' ? e.target.checked : e.target.value
    setForm((f) => ({ ...f, [e.target.name]: val }))
  }

  const handleSave = async (e) => {
    e.preventDefault(); setSaving(true); setFormError(null)
    try {
      if (editTarget) await userService.update(editTarget.UserID ?? editTarget.userID ?? editTarget.id, form)
      else await userService.create(form)
      setModalOpen(false); fetchUsers()
    } catch (err) { setFormError(err.response?.data?.message || err.message || 'Save failed') }
    finally { setSaving(false) }
  }

  const handleToggleStatus = async (u) => {
    const id = u.UserID ?? u.userID ?? u.id
    const current = u.IsActive ?? u.isActive ?? true
    try {
      await userService.setStatus(id, !current)
      fetchUsers()
    } catch (err) { alert(err.response?.data?.message || 'Failed to update status') }
  }

  const handleDelete = async () => {
    if (!deleteTarget) return; setDeleteLoading(true)
    try {
      await userService.delete(deleteTarget.UserID ?? deleteTarget.userID ?? deleteTarget.id)
      setDeleteTarget(null); fetchUsers()
    } catch (err) { alert(err.response?.data?.message || err.message || 'Delete failed') }
    finally { setDeleteLoading(false) }
  }

  if (loading) return <LoadingSpinner message="Loading users..." />
  if (error) return <ErrorMessage message={error} onRetry={fetchUsers} />

  return (
    <div className="space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-center gap-3 justify-between">
        <div>
          <h2 className="text-xl font-bold text-gray-800">Users</h2>
          <p className="text-sm text-gray-500">{users.length} total</p>
        </div>
        <button onClick={openCreate} className="btn-primary self-start sm:self-auto">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Add User
        </button>
      </div>

      <div className="card p-4">
        <input type="text" placeholder="Search by username or employee name..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field max-w-sm" />
      </div>

      <div className="card p-0 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="table-head">ID</th>
                <th className="table-head">Username</th>
                <th className="table-head hidden md:table-cell">Employee</th>
                <th className="table-head">Status</th>
                <th className="table-head">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {filtered.length === 0 ? (
                <tr><td colSpan={5} className="table-cell text-center text-gray-400 py-12">No users found</td></tr>
              ) : (
                filtered.map((u, i) => {
                  const id = u.UserID ?? u.userID ?? u.id ?? i
                  const username = u.UserName ?? u.userName ?? '—'
                  const empName = u.EmployeeFullName ?? u.employeeFullName ?? '—'
                  const isActive = u.IsActive ?? u.isActive ?? true
                  return (
                    <tr key={id} className="hover:bg-gray-50">
                      <td className="table-cell text-gray-400">{id}</td>
                      <td className="table-cell font-medium text-gray-800">{username}</td>
                      <td className="table-cell hidden md:table-cell text-gray-500">{empName}</td>
                      <td className="table-cell"><StatusBadge active={isActive} /></td>
                      <td className="table-cell">
                        <div className="flex items-center gap-2">
                          <button onClick={() => handleToggleStatus(u)} title={isActive ? 'Deactivate' : 'Activate'} className="p-1.5 text-gray-500 hover:text-amber-600 hover:bg-amber-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={isActive ? 'M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728A9 9 0 015.636 5.636m12.728 12.728L5.636 5.636' : 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z'} /></svg>
                          </button>
                          <button onClick={() => openEdit(u)} className="p-1.5 text-gray-500 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                          </button>
                          <button onClick={() => setDeleteTarget(u)} className="p-1.5 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editTarget ? 'Edit User' : 'Add User'}>
        <form onSubmit={handleSave} className="space-y-4">
          {formError && <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{formError}</p>}
          <div>
            <label className="label">Username</label>
            <input name="UserName" value={form.UserName} onChange={handleFormChange} required className="input-field" placeholder="john.doe" />
          </div>
          <div>
            <label className="label">Password {editTarget && <span className="text-gray-400 font-normal">(leave blank to keep)</span>}</label>
            <input name="Password" type="password" value={form.Password} onChange={handleFormChange} className="input-field" placeholder="••••••••" />
          </div>
          <div className="flex items-center gap-3">
            <input type="checkbox" id="isActive" name="IsActive" checked={form.IsActive} onChange={handleFormChange} className="w-4 h-4 rounded border-gray-300 text-blue-600" />
            <label htmlFor="isActive" className="text-sm text-gray-700">Active account</label>
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
        title="Delete User"
        message={`Are you sure you want to delete user "${deleteTarget?.UserName ?? deleteTarget?.userName}"?`}
      />
    </div>
  )
}
