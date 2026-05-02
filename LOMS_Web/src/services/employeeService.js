import apiClient from '../api/apiClient'

export const employeeService = {
  getAll: async () => {
    const response = await apiClient.get('/api/employees/all')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/employees/${id}`)
    return response.data
  },
  getFullName: async (id) => {
    const response = await apiClient.get(`/api/employees/${id}/fullname`)
    return response.data
  },
  findByNationalNo: async (nationalNo) => {
    const response = await apiClient.get(`/api/employees/find/${nationalNo}`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/employees', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/employees/${id}`, data)
    return response.data
  },
  delete: async (id) => {
    const response = await apiClient.delete(`/api/employees/${id}`)
    return response.data
  },
}
