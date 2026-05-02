import apiClient from '../api/apiClient';

export const authService = {
  login: async (username, password) => {
    try {
      // CORRECTION ICI : 'Users' avec une majuscule pour correspondre au UsersController C#
      const response = await apiClient.post('/api/Users/login', { 
        UserName: username, 
        Password: password 
      });
      
      // Si ton apiClient retourne déjà .data via un intercepteur, utilise 'response'
      // Sinon, utilise 'response.data'. 
      // La plupart des configs standards Axios nécessitent .data
      return response.data; 
    } catch (error) {
      console.error("Erreur lors du login:", error);
      throw error;
    }
  },
};