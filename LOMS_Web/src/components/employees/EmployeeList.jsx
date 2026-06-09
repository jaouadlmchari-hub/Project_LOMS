import React, { useState, useEffect } from 'react';
import { employeeService } from '../../services/employeeService';
import { FaEdit, FaTrash, FaPlus, FaSync, FaSearch, FaEye, FaEnvelope, FaPhone } from 'react-icons/fa';

const EmployeeList = ({ onEdit, onViewDetails, onAdd }) => {
    const [employees, setEmployees] = useState([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [loading, setLoading] = useState(false);
    
    // État pour le menu contextuel
    const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, emp: null });

    useEffect(() => {
        loadEmployees();
        
        // Fermer le menu si on clique ailleurs
        const handleClick = () => setContextMenu({ ...contextMenu, visible: false });
        document.addEventListener('click', handleClick);
        return () => document.removeEventListener('click', handleClick);
    }, []);

    const loadEmployees = async () => {
        setLoading(true);
        try {
            const data = await employeeService.getAll();
            setEmployees(Array.isArray(data) ? data : []);
        } catch (error) {
            console.error("Erreur chargement employés", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = async () => {
        if (!searchQuery) return loadEmployees();
        try {
            const emp = await employeeService.findByNationalNo(searchQuery);
            setEmployees(emp ? [emp] : []);
        } catch (e) {
            setEmployees([]);
        }
    };

    // Gestion du clic droit
    const handleRightClick = (e, emp) => {
        e.preventDefault();
        setContextMenu({
            visible: true,
            x: e.clientX,
            y: e.clientY,
            emp: emp
        });
    };

    // Action du menu contextuel
    const handleMenuAction = (action) => {
        const emp = contextMenu.emp;
        setContextMenu({ ...contextMenu, visible: false });

        switch (action) {
            case 'details':
                if (onViewDetails) onViewDetails(emp.EmployeeID);
                break;
            case 'edit':
                onEdit(emp);
                break;
            case 'add':
                onAdd();
                break;
            case 'delete':
                if (window.confirm(`Supprimer ${emp.FullName} ?`)) {
                    employeeService.delete(emp.EmployeeID).then(loadEmployees);
                }
                break;
            case 'email':
                if (emp.Email) window.location.href = `mailto:${emp.Email}`;
                else alert("Pas d'email enregistré.");
                break;
            case 'call':
                if (emp.Phone) window.location.href = `tel:${emp.Phone}`;
                else alert("Pas de téléphone enregistré.");
                break;
            default:
                break;
        }
    };

    return (
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
            {/* Header & Filters */}
            <div className="p-4 border-b border-gray-200 bg-gray-50 flex flex-col sm:flex-row justify-between items-center gap-4">
                <h2 className="text-xl font-bold text-gray-800">Employés</h2>
                
                <div className="flex gap-2 w-full sm:w-auto">
                    <div className="relative flex-grow sm:flex-grow-0">
                        <input 
                            type="text" 
                            placeholder="Rechercher National No..." 
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                        <FaSearch className="absolute left-3 top-3 text-gray-400" />
                    </div>
                    <button onClick={handleSearch} className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition">
                        OK
                    </button>
                    <button onClick={loadEmployees} className="bg-gray-200 text-gray-700 px-3 py-2 rounded-lg hover:bg-gray-300 transition" title="Actualiser">
                        <FaSync />
                    </button>
                    <button onClick={onAdd} className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition flex items-center gap-2">
                        <FaPlus /> Ajouter
                    </button>
                </div>
            </div>

            {/* Table */}
            <div className="overflow-x-auto">
                <table className="w-full text-left border-collapse">
                    <thead>
                        <tr className="bg-gray-100 text-gray-600 uppercase text-sm leading-normal">
                            <th className="py-3 px-6 font-bold">ID</th>
                            <th className="py-3 px-6 font-bold">Nom Complet</th>
                            <th className="py-3 px-6 font-bold">National No</th>
                            <th className="py-3 px-6 font-bold">Département</th>
                            <th className="py-3 px-6 font-bold text-center">Statut</th>
                            <th className="py-3 px-6 font-bold text-center">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="text-gray-600 text-sm font-light">
                        {loading ? (
                            <tr><td colSpan="6" className="py-4 text-center">Chargement...</td></tr>
                        ) : employees.length === 0 ? (
                            <tr><td colSpan="6" className="py-4 text-center text-gray-500">Aucun employé trouvé.</td></tr>
                        ) : (
                            employees.map(emp => (
                                <tr 
                                    key={emp.EmployeeID} 
                                    onContextMenu={(e) => handleRightClick(e, emp)} 
                                    className="border-b border-gray-200 hover:bg-gray-50"
                                >
                                    <td className="py-3 px-6">{emp.EmployeeID}</td>
                                    
                                    {/* CORRECTION ICI : Utilisation de FullName au lieu de FirstName + LastName */}
                                    <td className="py-3 px-6 font-medium text-gray-900">
                                        {emp.FullName}
                                    </td>
                                    
                                    <td className="py-3 px-6">{emp.NationalNo}</td>
                                    <td className="py-3 px-6">{emp.DepartmentName || 'N/A'}</td>
                                    
                                    {/* CORRECTION ICI : Utilisation de Status (string) au lieu de IsActive (bool) */}
                                    <td className="py-3 px-6 text-center">
                                        <span className={`px-3 py-1 rounded-full text-xs font-bold ${emp.Status === 'Active' ? 'bg-green-200 text-green-800' : 'bg-red-200 text-red-800'}`}>
                                            {emp.Status === 'Active' ? 'Actif' : 'Inactif'}
                                        </span>
                                    </td>
                                    
                                    <td className="py-3 px-6 text-center">
                                        <div className="flex item-center justify-center gap-2">
                                            <button onClick={() => onEdit(emp)} className="w-8 h-8 rounded-full bg-blue-100 text-blue-600 hover:bg-blue-200 flex items-center justify-center">
                                                <FaEdit />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* Context Menu */}
            {contextMenu.visible && (
                <div 
                    className="fixed bg-white shadow-lg rounded-lg py-2 z-50 border border-gray-200 min-w-[200px]"
                    style={{ top: contextMenu.y, left: contextMenu.x }}
                    onClick={(e) => e.stopPropagation()}
                >
                    <div onClick={() => handleMenuAction('details')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2">
                        <FaEye className="text-blue-500" /> Show Details
                    </div>
                    <hr className="my-1 border-gray-200" />
                    <div onClick={() => handleMenuAction('add')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2">
                        <FaPlus className="text-green-500" /> Add New Employee
                    </div>
                    <div onClick={() => handleMenuAction('edit')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2">
                        <FaEdit className="text-yellow-500" /> Edit
                    </div>
                    <div onClick={() => handleMenuAction('delete')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2 text-red-600">
                        <FaTrash /> Delete
                    </div>
                    <hr className="my-1 border-gray-200" />
                    <div onClick={() => handleMenuAction('email')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2">
                        <FaEnvelope className="text-blue-400" /> Send Email
                    </div>
                    <div onClick={() => handleMenuAction('call')} className="px-4 py-2 hover:bg-gray-100 cursor-pointer flex items-center gap-2">
                        <FaPhone className="text-green-500" /> Phone Call
                    </div>
                </div>
            )}
        </div>
    );
};

export default EmployeeList;