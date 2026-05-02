import apiClient from '../api/apiClient'

export const leaveTypeService = {
  getAll: async () => {
    const response = await apiClient.get('/api/leave-types')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/leave-types/${id}`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/leave-types', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/leave-types/${id}`, data)
    return response.data
  },
  delete: async (id) => {
    const response = await apiClient.delete(`/api/leave-types/${id}`)
    return response.data
  },
}
