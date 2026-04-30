import React, { useState, useEffect } from 'react';
import { departmentService } from '../../services/departmentService';
import { FaEdit, FaTrash, FaPlus, FaSync, FaSearch, FaToggleOn, FaToggleOff } from 'react-icons/fa';

const DepartmentList = ({ onEdit, onAdd }) => {
    const [departments, setDepartments] = useState([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        loadDepartments();
    }, []);

    const loadDepartments = async () => {
        setLoading(true);
        try {
            const data = await departmentService.getAll();
            setDepartments(Array.isArray(data) ? data : []);
        } catch (error) {
            console.error("Erreur chargement départements", error);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = async () => {
        if (!searchQuery) return loadDepartments();
        try {
            const dept = await departmentService.findByName(searchQuery);
            setDepartments(dept ? [dept] : []);
        } catch (e) {
            setDepartments([]);
        }
    };

    const handleDelete = async (id) => {
        if (window.confirm('Êtes-vous sûr de vouloir supprimer ce département ?\nAttention : Impossible si des employés y sont liés.')) {
            try {
                await departmentService.delete(id);
                loadDepartments();
            } catch (error) {
                alert("Suppression impossible. Vérifiez si des employés sont associés à ce département.");
            }
        }
    };

    const handleToggleStatus = async (id, currentStatus) => {
        try {
            await departmentService.toggleStatus(id, !currentStatus);
            loadDepartments(); // Recharger pour voir le nouveau statut
        } catch (error) {
            alert("Erreur lors du changement de statut");
        }
    };

    return (
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
            {/* Header & Filters */}
            <div className="p-4 border-b border-gray-200 bg-gray-50 flex flex-col sm:flex-row justify-between items-center gap-4">
                <h2 className="text-xl font-bold text-gray-800">Départements</h2>
                
                <div className="flex gap-2 w-full sm:w-auto">
                    <div className="relative flex-grow sm:flex-grow-0">
                        <input 
                            type="text" 
                            placeholder="Rechercher par nom..." 
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="w-full pl-10 pr-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                        <FaSearch className="absolute left-3 top-3 text-gray-400" />
                    </div>
                    <button onClick={handleSearch} className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition">
                        OK
                    </button>
                    <button onClick={loadDepartments} className="bg-gray-200 text-gray-700 px-3 py-2 rounded-lg hover:bg-gray-300 transition" title="Actualiser">
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
                            <th className="py-3 px-6 font-bold">Nom</th>
                            <th className="py-3 px-6 font-bold">Description</th>
                            <th className="py-3 px-6 font-bold text-center">Statut</th>
                            <th className="py-3 px-6 font-bold text-center">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="text-gray-600 text-sm font-light">
                        {loading ? (
                            <tr><td colSpan="5" className="py-4 text-center">Chargement...</td></tr>
                        ) : departments.length === 0 ? (
                            <tr><td colSpan="5" className="py-4 text-center">Aucun département trouvé.</td></tr>
                        ) : (
                            departments.map((dept) => (
                                <tr key={dept.DepartmentID} className="border-b border-gray-200 hover:bg-gray-50">
                                    <td className="py-3 px-6">{dept.DepartmentID}</td>
                                    <td className="py-3 px-6 font-medium">{dept.DepartmentName}</td>
                                    <td className="py-3 px-6 truncate max-w-xs">{dept.Description || '-'}</td>
                                    <td className="py-3 px-6 text-center">
                                        <button 
                                            onClick={() => handleToggleStatus(dept.DepartmentID, dept.IsActive)}
                                            className={`focus:outline-none transition-colors duration-200 ${dept.IsActive ? 'text-green-500 hover:text-green-700' : 'text-red-500 hover:text-red-700'}`}
                                            title={dept.IsActive ? "Désactiver" : "Activer"}
                                        >
                                            {dept.IsActive ? <FaToggleOn size={24} /> : <FaToggleOff size={24} />}
                                        </button>
                                    </td>
                                    <td className="py-3 px-6 text-center">
                                        <div className="flex item-center justify-center gap-2">
                                            <button onClick={() => onEdit(dept)} className="w-8 h-8 rounded-full bg-blue-100 text-blue-600 hover:bg-blue-200 flex items-center justify-center">
                                                <FaEdit />
                                            </button>
                                            <button onClick={() => handleDelete(dept.DepartmentID)} className="w-8 h-8 rounded-full bg-red-100 text-red-600 hover:bg-red-200 flex items-center justify-center">
                                                <FaTrash />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default DepartmentList;