import apiClient from '../api/apiClient'

export const applicationService = {
  getAll: async () => {
    const response = await apiClient.get('/api/applications/all')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/applications/${id}`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/applications/add', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/applications/update/${id}`, data)
    return response.data
  },
  updateStatus: async (id, newStatus) => {
    const response = await apiClient.patch(`/api/applications/update-status/${id}?newStatus=${newStatus}`)
    return response.data
  },
  delete: async (id) => {
    const response = await apiClient.delete(`/api/applications/delete/${id}`)
    return response.data
  },
}
