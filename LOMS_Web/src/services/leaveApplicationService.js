import apiClient from '../api/apiClient'

export const leaveApplicationService = {
  getByApplicationId: async (applicationId) => {
    const response = await apiClient.get(`/api/leave-applications/${applicationId}`)
    return response.data
  },
  calculateDays: async (startDate, endDate) => {
    const response = await apiClient.get('/api/leave-applications/calculate-days', {
      params: { startDate, endDate },
    })
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/leave-applications', data)
    return response.data
  },
  update: async (applicationId, data) => {
    const response = await apiClient.put(`/api/leave-applications/${applicationId}`, data)
    return response.data
  },
}
