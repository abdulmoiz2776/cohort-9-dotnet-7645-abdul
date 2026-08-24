import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const RegisterPage = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    if (password !== confirmPassword) {
      setError('Passwords must match.');
      setLoading(false);
      return;
    }

    try {
      const result = await register({
        firstName,
        lastName,
        username,
        email,
        password,
        confirmPassword
      });

      setSuccess(result.message || 'Registration successful. Check your email to verify your account.');
      setTimeout(() => navigate('/login'), 2000);
    } catch (ex) {
      setError(ex.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <PageLayout title="Register">
      <form className="auth-form" onSubmit={handleSubmit}>
        <label>
          First name
          <input
            type="text"
            value={firstName}
            required
            onChange={(event) => setFirstName(event.target.value)}
          />
        </label>
        <label>
          Last name
          <input
            type="text"
            value={lastName}
            required
            onChange={(event) => setLastName(event.target.value)}
          />
        </label>
        <label>
          Username
          <input
            type="text"
            value={username}
            required
            onChange={(event) => setUsername(event.target.value)}
          />
        </label>
        <label>
          Email
          <input
            type="email"
            value={email}
            required
            onChange={(event) => setEmail(event.target.value)}
          />
        </label>
        <label>
          Password
          <input
            type="password"
            value={password}
            required
            onChange={(event) => setPassword(event.target.value)}
          />
        </label>
        <label>
          Confirm Password
          <input
            type="password"
            value={confirmPassword}
            required
            onChange={(event) => setConfirmPassword(event.target.value)}
          />
        </label>
        {error && <div className="form-error">{error}</div>}
        {success && <div className="form-success">{success}</div>}
        <button type="submit" disabled={loading}>
          {loading ? 'Registering...' : 'Create account'}
        </button>
      </form>
    </PageLayout>
  );
};

export default RegisterPage;
