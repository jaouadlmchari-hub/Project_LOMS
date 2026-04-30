import React, { useState } from 'react';
import DepartmentList from '../components/departments/DepartmentList';
import DepartmentForm from '../components/departments/DepartmentForm';

const DepartmentsPage = () => {
    const [showForm, setShowForm] = useState(false);
    const [selectedDepartment, setSelectedDepartment] = useState(null);
    const [refreshKey, setRefreshKey] = useState(0);

    const handleAdd = () => {
        setSelectedDepartment(null);
        setShowForm(true);
    };

    const handleEdit = (dept) => {
        setSelectedDepartment(dept);
        setShowForm(true);
    };

    const handleSaved = () => {
        setRefreshKey(prev => prev + 1);
    };

    return (
        <div className="container mx-auto px-4 py-8">
            <DepartmentList 
                key={refreshKey}
                onEdit={handleEdit} 
                onAdd={handleAdd} 
            />

            {showForm && (
                <DepartmentForm 
                    department={selectedDepartment} 
                    onClose={() => setShowForm(false)} 
                    onSave={handleSaved} 
                />
            )}
        </div>
    );
};

export default DepartmentsPage;