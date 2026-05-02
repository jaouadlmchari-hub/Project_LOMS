import apiClient from '../api/apiClient'

export const jobService = {
  getAll: async () => {
    const response = await apiClient.get('/api/jobs/all')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/jobs/${id}`)
    return response.data
  },
  findByTitle: async (titleName) => {
    const response = await apiClient.get(`/api/jobs/find/${titleName}`)
    return response.data
  },
  getByDepartment: async (deptId) => {
    const response = await apiClient.get(`/api/jobs/department/${deptId}`)
    return response.data
  },
}
