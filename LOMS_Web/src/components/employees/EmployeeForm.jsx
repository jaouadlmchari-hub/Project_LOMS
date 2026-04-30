import React, { useState, useEffect } from 'react';
import { employeeService } from '../../services/employeeService';

const EmployeeForm = ({ employee, onClose, onSave }) => {
    const isEdit = !!employee;
    
    // États pour les données du formulaire
    const [form, setForm] = useState(employee || { 
        IsActive: true, 
        Gender: 0,
        DepartmentID: '',
        JobTitleID: ''
    });

    // États pour les listes déroulantes (Lookup Data)
    const [departments, setDepartments] = useState([]);
    const [jobs, setJobs] = useState([]);
    const [countries, setCountries] = useState([]);
    
    // État pour l'onglet actif (comme dans WinForms)
    const [activeTab, setActiveTab] = useState(1);

    // Chargement initial des données de référence
    useEffect(() => {
        const loadReferenceData = async () => {
            try {
                // On charge tout en parallèle pour plus de rapidité
                const [deptsData, countriesData] = await Promise.all([
                    employeeService.getDepartments(),
                    employeeService.getCountries()
                ]);
                
                setDepartments(deptsData);
                setCountries(countriesData);

                // Si on est en mode édition, on doit aussi charger les Jobs du département actuel
                if (isEdit && form.DepartmentID) {
                    const jobsData = await employeeService.getJobsByDepartment(form.DepartmentID);
                    setJobs(jobsData);
                }
            } catch (error) {
                console.error("Erreur chargement données de référence", error);
            }
        };

        loadReferenceData();
    }, []); // Le tableau vide [] assure que cela ne s'exécute qu'une fois au montage

    // Gestionnaire de changement général pour les inputs
    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm(prev => ({ 
            ...prev, 
            [name]: type === 'checkbox' ? checked : value 
        }));
    };

    // Logique spécifique : Quand le Département change, on recharge les Jobs
    const handleDepartmentChange = async (e) => {
        const deptId = e.target.value;
        
        // 1. Mettre à jour le département dans le state
        setForm(prev => ({ 
            ...prev, 
            DepartmentID: deptId,
            JobTitleID: '' // Important : Reset le job quand on change de dept
        }));

        // 2. Charger les jobs correspondants
        if (deptId) {
            try {
                const jobsData = await employeeService.getJobsByDepartment(deptId);
                setJobs(jobsData);
            } catch (error) {
                console.error("Erreur chargement jobs", error);
                setJobs([]);
            }
        } else {
            setJobs([]);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isEdit) {
                await employeeService.update(form.EmployeeID, form);
            } else {
                await employeeService.create(form);
            }
            onSave(); // Notifie la page parente de recharger la liste
            onClose();
        } catch (error) {
            alert("Erreur: " + (error.response?.data || error.message));
        }
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl mx-4 overflow-hidden">
                {/* Header */}
                <div className="bg-blue-600 px-6 py-4 flex justify-between items-center">
                    <h3 className="text-lg font-semibold text-white">
                        {isEdit ? 'Modifier Employé' : 'Nouvel Employé'}
                    </h3>
                    <button onClick={onClose} className="text-white hover:text-gray-200">
                        <i className="fas fa-times"></i>
                    </button>
                </div>
                
                <div className="p-6">
                    {/* Onglets (Tabs) */}
                    <div className="flex border-b border-gray-200 mb-6">
                        <button 
                            className={`py-2 px-4 font-medium text-sm focus:outline-none ${activeTab === 1 ? 'border-b-2 border-blue-500 text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab(1)}
                        >
                            Personnel
                        </button>
                        <button 
                            className={`py-2 px-4 font-medium text-sm focus:outline-none ${activeTab === 2 ? 'border-b-2 border-blue-500 text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab(2)}
                        >
                            Emploi
                        </button>
                        <button 
                            className={`py-2 px-4 font-medium text-sm focus:outline-none ${activeTab === 3 ? 'border-b-2 border-blue-500 text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
                            onClick={() => setActiveTab(3)}
                        >
                            Contact
                        </button>
                    </div>

                    <form onSubmit={handleSubmit}>
                        {/* --- TAB 1: PERSONNEL --- */}
                        {activeTab === 1 && (
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">National No *</label>
                                    <input 
                                        name="NationalNo" 
                                        value={form.NationalNo || ''} 
                                        onChange={handleChange} 
                                        type="text" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        required 
                                        disabled={isEdit} 
                                    />
                                </div>
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Date Naissance</label>
                                    <input 
                                        name="DateOfBirth" 
                                        value={form.DateOfBirth ? form.DateOfBirth.split('T')[0] : ''} 
                                        onChange={handleChange} 
                                        type="date" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                    />
                                </div>
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Prénom *</label>
                                    <input 
                                        name="FirstName" 
                                        value={form.FirstName || ''} 
                                        onChange={handleChange} 
                                        type="text" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        required 
                                    />
                                </div>
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Nom *</label>
                                    <input 
                                        name="LastName" 
                                        value={form.LastName || ''} 
                                        onChange={handleChange} 
                                        type="text" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        required 
                                    />
                                </div>
                                
                                {/* SELECT PAYS (Countries) */}
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Pays</label>
                                    <select 
                                        name="NationalityCountryID" 
                                        value={form.NationalityCountryID || ''} 
                                        onChange={handleChange} 
                                        className="shadow border rounded w-full py-2 px-3 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        <option value="">Choisir un pays...</option>
                                        {countries.map(c => (
                                            <option key={c.CountryID} value={c.CountryID}>
                                                {c.CountryName}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Genre</label>
                                    <select 
                                        name="Gender" 
                                        value={form.Gender} 
                                        onChange={handleChange} 
                                        className="shadow border rounded w-full py-2 px-3 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        <option value={0}>Homme</option>
                                        <option value={1}>Femme</option>
                                    </select>
                                </div>
                            </div>
                        )}

                        {/* --- TAB 2: EMPLOI --- */}
                        {activeTab === 2 && (
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                
                                {/* SELECT DÉPARTEMENT */}
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Département *</label>
                                    <select 
                                        name="DepartmentID" 
                                        value={form.DepartmentID || ''} 
                                        onChange={handleDepartmentChange} 
                                        className="shadow border rounded w-full py-2 px-3 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        required
                                    >
                                        <option value="">Choisir un département...</option>
                                        {departments.map(d => (
                                            <option key={d.DepartmentID} value={d.DepartmentID}>
                                                {d.DepartmentName}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                {/* SELECT JOB (Dépend du Département) */}
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Poste *</label>
                                    <select 
                                        name="JobTitleID" 
                                        value={form.JobTitleID || ''} 
                                        onChange={handleChange} 
                                        className="shadow border rounded w-full py-2 px-3 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        required 
                                        disabled={!form.DepartmentID} // Désactivé si aucun dept sélectionné
                                    >
                                        <option value="">Choisir un poste...</option>
                                        {jobs.map(j => (
                                            <option key={j.JobTitleID} value={j.JobTitleID}>
                                                {j.TitleName}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Salaire</label>
                                    <input 
                                        name="Salary" 
                                        value={form.Salary || ''} 
                                        onChange={handleChange} 
                                        type="number" 
                                        step="0.01" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                    />
                                </div>
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Date Embauche</label>
                                    <input 
                                        name="HireDate" 
                                        value={form.HireDate ? form.HireDate.split('T')[0] : ''} 
                                        onChange={handleChange} 
                                        type="date" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                    />
                                </div>
                                <div className="col-span-2">
                                    <div className="flex items-center">
                                        <input 
                                            name="IsActive" 
                                            checked={form.IsActive} 
                                            onChange={handleChange} 
                                            type="checkbox" 
                                            className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded" 
                                            id="isActive" 
                                        />
                                        <label className="ml-2 block text-gray-900 text-sm font-bold" htmlFor="isActive">Actif</label>
                                    </div>
                                </div>
                            </div>
                        )}

                        {/* --- TAB 3: CONTACT --- */}
                        {activeTab === 3 && (
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Email</label>
                                    <input 
                                        name="Email" 
                                        value={form.Email || ''} 
                                        onChange={handleChange} 
                                        type="email" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                    />
                                </div>
                                <div>
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Téléphone</label>
                                    <input 
                                        name="Phone" 
                                        value={form.Phone || ''} 
                                        onChange={handleChange} 
                                        type="text" 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                    />
                                </div>
                                <div className="col-span-2">
                                    <label className="block text-gray-700 text-sm font-bold mb-2">Adresse</label>
                                    <textarea 
                                        name="Address" 
                                        value={form.Address || ''} 
                                        onChange={handleChange} 
                                        className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                        rows="3"
                                    ></textarea>
                                </div>
                            </div>
                        )}
                    </form>
                </div>

                {/* Footer Buttons */}
                <div className="bg-gray-50 px-6 py-4 flex justify-end gap-3">
                    <button 
                        type="button" 
                        onClick={onClose} 
                        className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                    >
                        Annuler
                    </button>
                    <button 
                        type="button" 
                        onClick={handleSubmit} 
                        className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                    >
                        Enregistrer
                    </button>
                </div>
            </div>
        </div>
    );
};

export default EmployeeForm;