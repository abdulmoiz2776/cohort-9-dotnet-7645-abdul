import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
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
    <div className="auth-page-shell">
      <header className="auth-topbar">
        <div className="auth-brand">TaskMaster</div>
      </header>

      <main className="auth-main">
        <div className="auth-card wide-card">
          <div className="auth-accent" />
          <div className="auth-header-copy">
            <h1>Create Account</h1>
            <p>Start organizing your team and tasks in minutes.</p>
          </div>

          <form className="auth-form" onSubmit={handleSubmit}>
            <div className="two-col-fields">
              <div className="field-wrap">
                <label htmlFor="firstName">First Name</label>
                <input
                  id="firstName"
                  type="text"
                  value={firstName}
                  required
                  onChange={(event) => setFirstName(event.target.value)}
                />
              </div>
              <div className="field-wrap">
                <label htmlFor="lastName">Last Name</label>
                <input
                  id="lastName"
                  type="text"
                  value={lastName}
                  required
                  onChange={(event) => setLastName(event.target.value)}
                />
              </div>
            </div>

            <div className="field-wrap">
              <label htmlFor="username">Username</label>
              <input
                id="username"
                type="text"
                value={username}
                required
                onChange={(event) => setUsername(event.target.value)}
              />
            </div>

            <div className="field-wrap">
              <label htmlFor="register-email">Email</label>
              <input
                id="register-email"
                type="email"
                value={email}
                required
                onChange={(event) => setEmail(event.target.value)}
              />
            </div>

            <div className="two-col-fields">
              <div className="field-wrap">
                <label htmlFor="register-password">Password</label>
                <input
                  id="register-password"
                  type="password"
                  value={password}
                  required
                  onChange={(event) => setPassword(event.target.value)}
                />
              </div>
              <div className="field-wrap">
                <label htmlFor="confirmPassword">Confirm Password</label>
                <input
                  id="confirmPassword"
                  type="password"
                  value={confirmPassword}
                  required
                  onChange={(event) => setConfirmPassword(event.target.value)}
                />
              </div>
            </div>

            {error && <div className="form-error">{error}</div>}
            {success && <div className="form-success">{success}</div>}

            <button type="submit" className="primary-button auth-submit" disabled={loading}>
              {loading ? 'Creating account...' : 'Create Account'}
            </button>
          </form>

          <div className="auth-footer">
            <p>
              Already have an account?
              <Link to="/login">Sign in</Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
};

export default RegisterPage;
