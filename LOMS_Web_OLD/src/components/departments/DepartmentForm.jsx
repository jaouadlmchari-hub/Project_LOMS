import React, { useState, useEffect } from 'react';
import { departmentService } from '../../services/departmentService';

const DepartmentForm = ({ department, onClose, onSave }) => {
    const isEdit = !!department;
    const [formData, setFormData] = useState({
        DepartmentName: '',
        Description: ''
    });
    const [error, setError] = useState('');

    useEffect(() => {
        if (isEdit) {
            setFormData({
                DepartmentName: department.DepartmentName,
                Description: department.Description || ''
            });
        }
    }, [department, isEdit]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (!formData.DepartmentName.trim()) {
            setError("Le nom du département est obligatoire.");
            return;
        }

        try {
            if (isEdit) {
                await departmentService.update(department.DepartmentID, formData);
            } else {
                await departmentService.create(formData);
            }
            onSave();
            onClose();
        } catch (err) {
            setError(err.response?.data || "Une erreur est survenue.");
        }
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-md mx-4 overflow-hidden">
                <div className="bg-blue-600 px-6 py-4">
                    <h3 className="text-lg font-semibold text-white">
                        {isEdit ? 'Modifier le Département' : 'Nouveau Département'}
                    </h3>
                </div>
                
                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    {error && (
                        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative">
                            {error}
                        </div>
                    )}

                    <div>
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Nom du Département *
                        </label>
                        <input 
                            type="text" 
                            name="DepartmentName"
                            value={formData.DepartmentName}
                            onChange={handleChange}
                            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Ex: Ressources Humaines"
                            autoFocus
                        />
                    </div>

                    <div>
                        <label className="block text-gray-700 text-sm font-bold mb-2">
                            Description
                        </label>
                        <textarea 
                            name="Description"
                            value={formData.Description}
                            onChange={handleChange}
                            rows="3"
                            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500"
                            placeholder="Description optionnelle..."
                        ></textarea>
                    </div>

                    <div className="flex items-center justify-end gap-3 mt-6">
                        <button 
                            type="button" 
                            onClick={onClose}
                            className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                        >
                            Annuler
                        </button>
                        <button 
                            type="submit"
                            className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                        >
                            Enregistrer
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default DepartmentForm;