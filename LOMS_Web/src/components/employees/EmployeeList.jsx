import React, { useState, useEffect, useRef } from 'react';
import { employeeService } from '../../services/employeeService';
import { FaEdit, FaTrash, FaEye, FaPlus, FaEnvelope, FaPhone, FaSync, FaSearch } from 'react-icons/fa';

const EmployeeList = ({ onEdit, onViewDetails, onAdd }) => {
    const [employees, setEmployees] = useState([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, emp: null });
    const menuRef = useRef(null);

    useEffect(() => {
        loadEmployees();
        const handleClickOutside = () => setContextMenu({ ...contextMenu, visible: false });
        document.addEventListener('click', handleClickOutside);
        return () => document.removeEventListener('click', handleClickOutside);
    }, []);

    const loadEmployees = async () => {
        try {
            const data = await employeeService.getAll();
            setEmployees(Array.isArray(data) ? data : []);
        } catch (error) {
            console.error("Erreur chargement employés", error);
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

    const handleRightClick = (e, emp) => {
        e.preventDefault();
        setContextMenu({
            visible: true,
            x: e.clientX,
            y: e.clientY,
            emp: emp
        });
    };

    const handleAction = (action) => {
        const emp = contextMenu.emp;
        setContextMenu({ ...contextMenu, visible: false });

        switch (action) {
            case 'details': onViewDetails(emp.EmployeeID); break;
            case 'edit': onEdit(emp); break;
            case 'add': onAdd(); break;
            case 'delete': 
                if(window.confirm(`Supprimer ${emp.FirstName} ?`)) {
                    employeeService.delete(emp.EmployeeID).then(loadEmployees);
                }
                break;
            case 'email':
                if(emp.Email) window.location.href = `mailto:${emp.Email}?subject=Regarding your employment`;
                else alert("Pas d'email enregistré.");
                break;
            case 'call':
                if(emp.Phone) window.location.href = `tel:${emp.Phone}`;
                else alert("Pas de téléphone enregistré.");
                break;
            default: break;
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
                        {employees.map(emp => (
                            <tr key={emp.EmployeeID} onContextMenu={(e) => handleRightClick(e, emp)} className="border-b border-gray-200 hover:bg-gray-50">
                                <td className="py-3 px-6">{emp.EmployeeID}</td>
                                <td className="py-3 px-6 font-medium">{emp.FullName} </td>
                                <td className="py-3 px-6">{emp.NationalNo}</td>
                                <td className="py-3 px-6">{emp.DepartmentName || 'N/A'}</td>
                                <td className="py-3 px-6 text-center">
                                    <span className={`px-3 py-1 rounded-full text-xs ${emp.IsActive ? 'bg-green-200 text-green-800' : 'bg-red-200 text-red-800'}`}>
                                        {emp.IsActive ? 'Actif' : 'Inactif'}
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
                        ))}
                    </tbody>
                </table>
            </div>

            {/* Context Menu Customisé */}
            {contextMenu.visible && (
                <div 
                    ref={menuRef}
                    className="position-fixed shadow-lg rounded bg-white py-2"
                    style={{ top: contextMenu.y, left: contextMenu.x, zIndex: 1050, minWidth: '200px' }}
                    onClick={(e) => e.stopPropagation()}
                >
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center" onClick={() => handleAction('details')}>
                        <FaEye className="text-blue-500 mr-2" /> Show Details
                    </div>
                    <hr className="my-1 border-gray-200" />
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center" onClick={() => handleAction('add')}>
                        <FaPlus className="text-green-500 mr-2" /> Add New Employee
                    </div>
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center" onClick={() => handleAction('edit')}>
                        <FaEdit className="text-yellow-500 mr-2" /> Edit
                    </div>
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center text-red-600" onClick={() => handleAction('delete')}>
                        <FaTrash className="mr-2" /> Delete
                    </div>
                    <hr className="my-1 border-gray-200" />
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center" onClick={() => handleAction('email')}>
                        <FaEnvelope className="text-blue-400 mr-2" /> Send Email
                    </div>
                    <div className="px-3 py-2 hover:bg-gray-100 cursor-pointer flex items-center" onClick={() => handleAction('call')}>
                        <FaPhone className="text-green-500 mr-2" /> Phone Call
                    </div>
                </div>
            )}
        </div>
    );
};

export default EmployeeList;