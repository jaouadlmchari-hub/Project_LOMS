import { createContext, useContext, useState, useCallback } from 'react';
import { authService } from '../services/authService';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('user');
    return saved ? JSON.parse(saved) : null;
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const isAuthenticated = Boolean(user);

  const login = useCallback(async (username, password) => {
    setLoading(true);
    setError(null);
    
    try {
      // Appel au service
      const data = await authService.login(username, password);
      
      // Stockage dans le localStorage
      localStorage.setItem('user', JSON.stringify(data));
      setUser(data);
      
      return true;
    } catch (err) {
      // Gestion des erreurs affichées à l'utilisateur
      let message = 'Une erreur est survenue.';
      
      if (err.response?.status === 401) {
        message = 'Identifiants incorrects.';
      } else if (err.response?.data?.message) {
        message = err.response.data.message;
      } else if (err.message) {
        message = err.message;
      }
      
      setError(message);
      console.error("Détails de l'erreur:", err);
      return false;
    } finally {
      setLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('user');
    setUser(null);
    setError(null);
  }, []);

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, loading, error, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}