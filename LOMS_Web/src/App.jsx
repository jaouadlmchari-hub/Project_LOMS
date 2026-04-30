import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import EmployeesPage from './pages/EmployeesPage';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-100">
        {/* Navbar simple pour tester */}
        <nav className="bg-blue-600 p-4 text-white shadow-md">
          <div className="container mx-auto">
            <h1 className="text-xl font-bold">LOMS Management System</h1>
          </div>
        </nav>

        <Routes>
          <Route path="/" element={<EmployeesPage />} />
          <Route path="/employees" element={<EmployeesPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;