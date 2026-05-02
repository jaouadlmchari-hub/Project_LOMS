import apiClient from '../api/apiClient'

export const leaveBalanceService = {
  getByEmployeeAndYear: async (employeeID, year) => {
    const response = await apiClient.get(`/api/leave-balances/employee/${employeeID}/year/${year}`)
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/leave-balances/${id}`)
    return response.data
  },
  initialize: async (employeeID, year) => {
    const response = await apiClient.post(`/api/leave-balances/initialize?employeeID=${employeeID}&year=${year}`)
    return response.data
  },
  updateUsedDays: async (employeeID, leaveTypeID, year, daysCount) => {
    const response = await apiClient.put(
      `/api/leave-balances/update-used-days?employeeID=${employeeID}&leaveTypeID=${leaveTypeID}&year=${year}&daysCount=${daysCount}`
    )
    return response.data
  },
  updateBalance: async (id, data) => {
    const response = await apiClient.put(`/api/leave-balances/${id}`, data)
    return response.data
  },
}
