import apiClient from '../api/apiClient'

export const roleService = {
  getAll: async () => {
    const response = await apiClient.get('/api/roles')
    return response.data
  },
  getRoleNames: async (roleIDs) => {
    const response = await apiClient.post('/api/roles/names', roleIDs)
    return response.data
  },
}
