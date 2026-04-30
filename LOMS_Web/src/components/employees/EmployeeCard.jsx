import React from 'react';
import { FaUser, FaBriefcase, FaMapMarkerAlt, FaEnvelope, FaPhone } from 'react-icons/fa';

const EmployeeCard = ({ employee, onClose }) => {
    if (!employee) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl mx-4 overflow-hidden">
                <div className="bg-dark-blue bg-blue-900 text-white p-6 flex justify-between items-start">
                    <div className="flex items-center gap-6">
                        <img 
                            src={employee.ImagePath || (employee.Gender === 0 ? '/male-placeholder.png' : '/female-placeholder.png')} 
                            alt="Profile" 
                            className="w-24 h-24 rounded-full border-4 border-white object-cover"
                        />
                        <div>
                            <h2 className="text-2xl font-bold">{employee.FirstName} {employee.LastName}</h2>
                            <p className="text-blue-200">{employee.TitleName} - {employee.DepartmentName}</p>
                            <span className={`inline-block mt-2 px-3 py-1 rounded-full text-xs font-bold ${employee.IsActive ? 'bg-green-500 text-white' : 'bg-red-500 text-white'}`}>
                                {employee.IsActive ? 'ACTIF' : 'INACTIF'}
                            </span>
                        </div>
                    </div>
                    <button onClick={onClose} className="text-white hover:text-gray-300">
                        <i className="fas fa-times text-xl"></i>
                    </button>
                </div>

                <div className="p-6 grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Personal Info */}
                    <div className="bg-gray-50 p-4 rounded-lg">
                        <h3 className="text-lg font-bold text-gray-700 mb-4 border-b pb-2">Informations Personnelles</h3>
                        <div className="space-y-3 text-sm">
                            <div><span className="font-semibold text-gray-500">ID:</span> {employee.EmployeeID}</div>
                            <div><span className="font-semibold text-gray-500">National No:</span> {employee.NationalNo}</div>
                            <div><span className="font-semibold text-gray-500">Naissance:</span> {employee.DateOfBirth ? new Date(employee.DateOfBirth).toLocaleDateString() : '-'}</div>
                            <div><span className="font-semibold text-gray-500">Genre:</span> {employee.Gender === 0 ? 'Homme' : 'Femme'}</div>
                            <div><span className="font-semibold text-gray-500">Nationalité:</span> {employee.NationalityCountryName || '-'}</div>
                        </div>
                    </div>

                    {/* Job Info */}
                    <div className="bg-gray-50 p-4 rounded-lg">
                        <h3 className="text-lg font-bold text-gray-700 mb-4 border-b pb-2 flex items-center gap-2"><FaBriefcase/> Emploi</h3>
                        <div className="space-y-3 text-sm">
                            <div><span className="font-semibold text-gray-500">Département:</span> {employee.DepartmentName}</div>
                            <div><span className="font-semibold text-gray-500">Poste:</span> {employee.TitleName}</div>
                            <div><span className="font-semibold text-gray-500">Embauche:</span> {employee.HireDate ? new Date(employee.HireDate).toLocaleDateString() : '-'}</div>
                            <div><span className="font-semibold text-gray-500">Salaire:</span> {employee.Salary ? `${employee.Salary} MAD` : '-'}</div>
                        </div>
                    </div>

                    {/* Contact Info */}
                    <div className="bg-gray-50 p-4 rounded-lg">
                        <h3 className="text-lg font-bold text-gray-700 mb-4 border-b pb-2 flex items-center gap-2"><FaMapMarkerAlt/> Contact</h3>
                        <div className="space-y-3 text-sm">
                            <div className="flex items-center gap-2"><FaEnvelope className="text-blue-500"/> {employee.Email || 'N/A'}</div>
                            <div className="flex items-center gap-2"><FaPhone className="text-green-500"/> {employee.Phone || 'N/A'}</div>
                            <div className="mt-2 text-gray-600 italic">{employee.Address || 'Aucune adresse enregistrée'}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default EmployeeCard;