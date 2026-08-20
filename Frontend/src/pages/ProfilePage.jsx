import { useEffect, useState } from 'react';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const ProfilePage = () => {
  const { user, updateProfile, changePassword } = useAuth();
  const [firstName, setFirstName] = useState(user?.firstName || '');
  const [lastName, setLastName] = useState(user?.lastName || '');
  const [username, setUsername] = useState(user?.username || '');
  const [email, setEmail] = useState(user?.email || '');
  const [profileMessage, setProfileMessage] = useState('');
  const [profileError, setProfileError] = useState('');
  const [passwordForm, setPasswordForm] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' });
  const [passwordMessage, setPasswordMessage] = useState('');
  const [passwordError, setPasswordError] = useState('');

  useEffect(() => {
    setFirstName(user?.firstName || '');
    setLastName(user?.lastName || '');
    setUsername(user?.username || '');
    setEmail(user?.email || '');
  }, [user]);

  const handleProfileSubmit = async (event) => {
    event.preventDefault();
    setProfileError('');
    setProfileMessage('');

    try {
      const updated = await updateProfile({ firstName, lastName, username, email });
      setProfileMessage('Profile updated successfully.');
      setUsername(updated.username);
      setFirstName(updated.firstName);
      setLastName(updated.lastName);
      setEmail(updated.email);
    } catch (ex) {
      setProfileError(ex.message);
    }
  };

  const handlePasswordSubmit = async (event) => {
    event.preventDefault();
    setPasswordError('');
    setPasswordMessage('');

    if (passwordForm.newPassword !== passwordForm.confirmPassword) {
      setPasswordError('New password confirmation does not match.');
      return;
    }

    try {
      const result = await changePassword({
        currentPassword: passwordForm.currentPassword,
        newPassword: passwordForm.newPassword,
        confirmPassword: passwordForm.confirmPassword
      });
      setPasswordMessage(result.message || 'Password updated successfully.');
      setPasswordForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
    } catch (ex) {
      setPasswordError(ex.message);
    }
  };

  return (
    <PageLayout title="Profile">
      <div className="two-column-layout">
        <div className="card">
          <h3>My details</h3>
          <div className="profile-summary">
            <p><strong>Email:</strong> {user?.email}</p>
            <p><strong>Roles:</strong> {user?.roles?.join(', ') || 'No roles'}</p>
            <p><strong>Status:</strong> {user?.isActive ? 'Active' : 'Inactive'}</p>
          </div>

          <form className="stacked-form" onSubmit={handleProfileSubmit}>
            <label>
              First name
              <input type="text" value={firstName} onChange={(event) => setFirstName(event.target.value)} required />
            </label>
            <label>
              Last name
              <input type="text" value={lastName} onChange={(event) => setLastName(event.target.value)} required />
            </label>
            <label>
              Username
              <input type="text" value={username} onChange={(event) => setUsername(event.target.value)} required />
            </label>
            <label>
              Email
              <input type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
            </label>
            {profileError && <div className="form-error">{profileError}</div>}
            {profileMessage && <div className="form-success">{profileMessage}</div>}
            <button type="submit" className="primary-button">Save profile</button>
          </form>
        </div>

        <div className="card">
          <h3>Change password</h3>
          <form className="stacked-form" onSubmit={handlePasswordSubmit}>
            <label>
              Current password
              <input type="password" value={passwordForm.currentPassword} onChange={(event) => setPasswordForm((prev) => ({ ...prev, currentPassword: event.target.value }))} required />
            </label>
            <label>
              New password
              <input type="password" value={passwordForm.newPassword} onChange={(event) => setPasswordForm((prev) => ({ ...prev, newPassword: event.target.value }))} required />
            </label>
            <label>
              Confirm password
              <input type="password" value={passwordForm.confirmPassword} onChange={(event) => setPasswordForm((prev) => ({ ...prev, confirmPassword: event.target.value }))} required />
            </label>
            {passwordError && <div className="form-error">{passwordError}</div>}
            {passwordMessage && <div className="form-success">{passwordMessage}</div>}
            <button type="submit" className="secondary-button">Change password</button>
          </form>
        </div>
      </div>
    </PageLayout>
  );
};

export default ProfilePage;
