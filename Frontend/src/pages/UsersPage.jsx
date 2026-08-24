import { useEffect, useState } from 'react';
import PageLayout from '../components/PageLayout';
import api from '../api/httpClient';
import { useAuth } from '../contexts/AuthContext';

const UsersPage = () => {
  const { user } = useAuth();
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadUsers = async () => {
      try {
        const response = await api.get('/api/users');
        setUsers(response.data);
      } catch (ex) {
        setError('Unable to load users.');
      }
    };

    if (user?.roles?.includes('Admin')) {
      loadUsers();
    }
  }, [user]);

  return (
    <PageLayout title="User Management">
      {error && <div className="form-error">{error}</div>}
      <div className="table-container">
        <table>
          <thead>
            <tr>
              <th>Email</th>
              <th>Username</th>
              <th>Full name</th>
              <th>Active</th>
              <th>Roles</th>
            </tr>
          </thead>
          <tbody>
            {users.map((item) => (
              <tr key={item.id}>
                <td>{item.email}</td>
                <td>{item.username}</td>
                <td>{`${item.firstName} ${item.lastName}`}</td>
                <td>{item.isActive ? 'Yes' : 'No'}</td>
                <td>{item.roles?.join(', ')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </PageLayout>
  );
};

export default UsersPage;
