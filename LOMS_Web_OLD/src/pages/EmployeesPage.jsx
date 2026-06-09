import React, { useState } from 'react';
import EmployeeList from '../components/employees/EmployeeList';
import EmployeeForm from '../components/employees/EmployeeForm';
import EmployeeCard from '../components/employees/EmployeeCard';
import { employeeService } from '../services/employeeService';

const EmployeesPage = () => {
    const [showForm, setShowForm] = useState(false);
    const [selectedEmployee, setSelectedEmployee] = useState(null);
    const [viewingEmployeeId, setViewingEmployeeId] = useState(null);
    const [viewingEmployeeData, setViewingEmployeeData] = useState(null);
    const [refreshKey, setRefreshKey] = useState(0);

    const handleAdd = () => {
        setSelectedEmployee(null);
        setShowForm(true);
    };

    const handleEdit = (emp) => {
        setSelectedEmployee(emp);
        setShowForm(true);
    };

    const handleViewDetails = async (id) => {
        setViewingEmployeeId(id);
        try {
            const data = await employeeService.getById(id);
            setViewingEmployeeData(data);
        } catch (error) {
            console.error(error);
        }
    };

    const handleSaved = () => {
        setRefreshKey(prev => prev + 1);
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <EmployeeList 
                key={refreshKey}
                onEdit={handleEdit} 
                onAdd={handleAdd} 
                onViewDetails={handleViewDetails} 
            />

            {showForm && (
                <EmployeeForm 
                    employee={selectedEmployee} 
                    onClose={() => setShowForm(false)} 
                    onSave={handleSaved} 
                />
            )}

            {viewingEmployeeData && (
                <EmployeeCard 
                    employee={viewingEmployeeData} 
                    onClose={() => setViewingEmployeeData(null)} 
                />
            )}
        </div>
    );
};

export default EmployeesPage;