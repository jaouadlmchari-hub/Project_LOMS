import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/';

// Configuration globale d'Axios pour ce service
const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Intercepteur optionnel pour gérer les erreurs globalement 
apiClient.interceptors.response.use(
    (response) => response.data, // Retourne directement les données si succès
    (error) => {
        const message = error.response?.data?.message || error.message || 'Une erreur est survenue';
        console.error('API Error (Departments):', message);
        return Promise.reject(error);
    }
);

export const departmentService = {
    // GET: /api/Departments/all
    getAll: async () => {
        return await apiClient.get('/api/Departments/all');
    },

    // GET: /api/Departments/{id}
    getById: async (id) => {
        return await apiClient.get(`/api/Departments/${id}`);
    },

    // GET: /api/Departments/find/{name}
    findByName: async (name) => {
        return await apiClient.get(`/api/Departments/find/${name}`);
    },

    // POST: /api/Departments
    create: async (dto) => {
        return await apiClient.post('/api/Departments', dto);
    },

    // PUT: /api/Departments/{id}
    update: async (id, dto) => {
        return await apiClient.put(`/api/Departments/${id}`, dto);
    },

    // DELETE: /api/Departments/{id}
    delete: async (id) => {
        return await apiClient.delete(`/api/Departments/${id}`);
    },

    // PATCH: /api/Departments/{id}/status
    toggleStatus: async (id, newStatus) => {
        // newStatus est un booléen (true/false)
        return await apiClient.patch(`/api/Departments/${id}/status`, newStatus);
    }
};