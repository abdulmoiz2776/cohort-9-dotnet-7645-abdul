import { useEffect, useState } from 'react';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const RoleManagementPage = () => {
  const [roles, setRoles] = useState([]);
  const [users, setUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedRole, setSelectedRole] = useState('');
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  const loadData = async () => {
    try {
      const [rolesRes, usersRes] = await Promise.all([
        api.get('/api/users/roles'),
        api.get('/api/users')
      ]);

      setRoles(rolesRes.data || []);
      setUsers(usersRes.data || []);
    } catch (loadError) {
      setError(loadError.response?.data?.message || 'Unable to load role data.');
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleAssignRole = async () => {
    if (!selectedUserId || !selectedRole) {
      setError('Select a user and a role.');
      return;
    }

    try {
      setError('');
      setMessage('');
      await api.post(`/api/users/${selectedUserId}/roles`, { roleName: selectedRole });
      setMessage('Role assigned successfully.');
      await loadData();
    } catch (assignError) {
      setError(assignError.response?.data?.message || 'Unable to assign role.');
    }
  };

  return (
    <PageLayout title="Role Management">
      <div className="card form-card">
        {error && <div className="form-error">{error}</div>}
        {message && <div className="form-success">{message}</div>}

        <label>
          User
          <select value={selectedUserId} onChange={(event) => setSelectedUserId(event.target.value)}>
            <option value="">Select user</option>
            {users.map((user) => (
              <option key={user.id} value={user.id}>
                {user.username} ({user.email})
              </option>
            ))}
          </select>
        </label>

        <label>
          Role
          <select value={selectedRole} onChange={(event) => setSelectedRole(event.target.value)}>
            <option value="">Select role</option>
            {roles.map((role) => (
              <option key={role.name} value={role.name}>
                {role.name}
              </option>
            ))}
          </select>
        </label>

        <button type="button" className="primary-button" onClick={handleAssignRole}>Assign role</button>
      </div>
    </PageLayout>
  );
};

export default RoleManagementPage;
