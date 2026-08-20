import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const formatDate = (value) => {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleString();
};

const UserDetailsPage = () => {
  const { userId } = useParams();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadUser = async () => {
      try {
        const response = await api.get(`/api/users/${userId}`);
        setUser(response.data);
      } catch (loadError) {
        setError(loadError.response?.data?.message || 'Unable to load user details.');
      } finally {
        setLoading(false);
      }
    };

    if (userId) {
      loadUser();
    }
  }, [userId]);

  if (loading) {
    return <PageLayout title="User Details"><p>Loading user...</p></PageLayout>;
  }

  if (!user) {
    return <PageLayout title="User Details"><div className="form-error">{error || 'User not found.'}</div></PageLayout>;
  }

  return (
    <PageLayout title="User Details">
      <div className="page-actions">
        <Link className="secondary-button" to="/users">Back to users</Link>
      </div>

      <div className="card detail-grid">
        <div>
          <p className="eyebrow">Profile</p>
          <h2>{user.firstName} {user.lastName}</h2>
          <p><strong>Username:</strong> {user.username}</p>
          <p><strong>Email:</strong> {user.email}</p>
          <p><strong>Roles:</strong> {user.roles?.join(', ') || 'None'}</p>
          <p><strong>Status:</strong> {user.isActive ? 'Active' : 'Inactive'}</p>
          <p><strong>Email confirmed:</strong> {user.emailConfirmed ? 'Yes' : 'No'}</p>
          <p><strong>Created:</strong> {formatDate(user.createdAt)}</p>
        </div>
      </div>
    </PageLayout>
  );
};

export default UserDetailsPage;
