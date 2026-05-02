import apiClient from '../api/apiClient'

export const userService = {
  getAll: async () => {
    const response = await apiClient.get('/api/users')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/users/${id}`)
    return response.data
  },
  getUsername: async (userID) => {
    const response = await apiClient.get(`/api/users/${userID}/username`)
    return response.data
  },
  create: async (data) => {
    const response = await apiClient.post('/api/users', data)
    return response.data
  },
  update: async (id, data) => {
    const response = await apiClient.put(`/api/users/${id}`, data)
    return response.data
  },
  setStatus: async (id, isActive) => {
    const response = await apiClient.patch(`/api/users/${id}/status`, isActive)
    return response.data
  },
  delete: async (id) => {
    const response = await apiClient.delete(`/api/users/${id}`)
    return response.data
  },
  checkUsername: async (username) => {
    const response = await apiClient.get(`/api/users/check-username/${username}`)
    return response.data
  },
}
