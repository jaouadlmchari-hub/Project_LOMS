import apiClient from '../api/apiClient'

export const countryService = {
  getAll: async () => {
    const response = await apiClient.get('/api/countries/all')
    return response.data
  },
  getById: async (id) => {
    const response = await apiClient.get(`/api/countries/${id}`)
    return response.data
  },
}
