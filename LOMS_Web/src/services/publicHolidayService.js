import apiClient from '../api/apiClient'

export const publicHolidayService = {
  getAll: async () => {
    const response = await apiClient.get('/api/holidays')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/holidays/${id}`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/holidays', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/holidays/${id}`, data)
    return response.data
  },
  delete: async (id) => {
    const response = await apiClient.delete(`/api/holidays/${id}`)
    return response.data
  },
  checkIfHoliday: async (date) => {
    const response = await apiClient.get(`/api/holidays/is-holiday/${date}`)
    return response.data
  },
}
