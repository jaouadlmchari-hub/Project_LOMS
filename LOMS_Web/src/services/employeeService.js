import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080/';

// Configuration globale d'Axios pour ce service
const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Intercepteur pour gérer les erreurs globalement 
apiClient.interceptors.response.use(
    (response) => response.data, // Retourne directement les données si succès
    (error) => {
        const message = error.response?.data?.message || error.message || 'Une erreur est survenue';
        console.error('API Error:', message);
        return Promise.reject(error);
    }
);

export const employeeService = {
    // --- EMPLOYEES ---
    
    getAll: async () => {
        // Note: Le slash devant 'api' est important si baseURL ne l'a pas
        return await apiClient.get('/api/Employees/all');
    },

    getById: async (id) => {
        return await apiClient.get(`/api/Employees/${id}`);
    },

    findByNationalNo: async (nationalNo) => {
        return await apiClient.get(`/api/Employees/find/${nationalNo}`);
    },

    create: async (dto) => {
        return await apiClient.post('/api/Employees', dto);
    },

    update: async (id, dto) => {
        return await apiClient.put(`/api/Employees/${id}`, dto);
    },

    delete: async (id) => {
        return await apiClient.delete(`/api/Employees/${id}`);
    },

    // --- LOOKUP DATA (Pour les formulaires) ---

    getDepartments: async () => {
        return await apiClient.get('/api/Departments/all');
    },

    getJobsByDepartment: async (deptId) => {
        if (!deptId) return [];
        return await apiClient.get(`/api/Jobs/department/${deptId}`);
    },

    getCountries: async () => {
        return await apiClient.get('/api/Countries/all');
    }
};