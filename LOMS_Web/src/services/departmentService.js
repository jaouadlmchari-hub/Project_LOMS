import apiClient from '../api/apiClient'

export const departmentService = {
  getAll: async () => {
    const response = await apiClient.get('/api/departments/all')
    return response.data
  },
  getSummary: async () => {
    const response = await apiClient.get('/api/departments/summary')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/departments/${id}`)
    return response.data
  },
  findByName: async (name) => {
    const response = await apiClient.get(`/api/departments/find/${name}`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/departments', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/departments/${id}`, data)
    return response.data
  },
  setStatus: async (id, isActive) => {
    const response = await apiClient.patch(`/api/departments/${id}/status`, isActive)
    return response.data
  },
}
