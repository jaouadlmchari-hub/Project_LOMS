import apiClient from '../api/apiClient'

export const salaryService = {
  getByEmployeeId: async (employeeId) => {
    const response = await apiClient.get(`/api/salary/by-employee/${employeeId}`)
    return response.data
  },
  getByNationalNo: async (nationalNo) => {
    const response = await apiClient.get(`/api/salary/by-national-no/${nationalNo}`)
    return response.data
  },
  add: async (data) => {
    const response = await apiClient.post('/api/salary/add', data)
    return response.data
  },
}
