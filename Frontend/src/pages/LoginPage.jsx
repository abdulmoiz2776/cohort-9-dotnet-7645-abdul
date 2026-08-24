import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const LoginPage = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login({ email, password, rememberMe });
      navigate('/');
    } catch (ex) {
      setError(ex.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <PageLayout title="Login">
      <form className="auth-form" onSubmit={handleSubmit}>
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
        <label className="checkbox-label">
          <input
            type="checkbox"
            checked={rememberMe}
            onChange={(event) => setRememberMe(event.target.checked)}
          />
          Remember me
        </label>
        {error && <div className="form-error">{error}</div>}
        <button type="submit" disabled={loading}>
          {loading ? 'Signing in...' : 'Sign in'}
        </button>
      </form>
    </PageLayout>
  );
};

export default LoginPage;
