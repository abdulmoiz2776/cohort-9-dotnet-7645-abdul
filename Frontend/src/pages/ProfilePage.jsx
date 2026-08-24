import { useEffect, useState } from 'react';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const ProfilePage = () => {
  const { user, updateProfile } = useAuth();
  const [firstName, setFirstName] = useState(user?.firstName || '');
  const [lastName, setLastName] = useState(user?.lastName || '');
  const [username, setUsername] = useState(user?.username || '');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    setFirstName(user?.firstName || '');
    setLastName(user?.lastName || '');
    setUsername(user?.username || '');
  }, [user]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setMessage('');

    try {
      const updated = await updateProfile({ firstName, lastName, username });
      setMessage('Profile updated successfully.');
      setUsername(updated.username);
      setFirstName(updated.firstName);
      setLastName(updated.lastName);
    } catch (ex) {
      setError(ex.message);
    }
  };

  return (
    <PageLayout title="Profile">
      <div className="profile-card">
        <h2>My details</h2>
        <p>Email: {user?.email}</p>
        <p>Roles: {user?.roles?.join(', ')}</p>
        <form className="auth-form" onSubmit={handleSubmit}>
          <label>
            First name
            <input
              type="text"
              value={firstName}
              onChange={(event) => setFirstName(event.target.value)}
              required
            />
          </label>
          <label>
            Last name
            <input
              type="text"
              value={lastName}
              onChange={(event) => setLastName(event.target.value)}
              required
            />
          </label>
          <label>
            Username
            <input
              type="text"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              required
            />
          </label>
          {error && <div className="form-error">{error}</div>}
          {message && <div className="form-success">{message}</div>}
          <button type="submit">Save changes</button>
        </form>
      </div>
    </PageLayout>
  );
};

export default ProfilePage;
