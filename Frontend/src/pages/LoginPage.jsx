import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
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
      navigate('/tasks');
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
        <div className="auth-card">
          <div className="auth-accent" />
          <div className="auth-header-copy">
            <h1>Welcome Back</h1>
            <p>Sign in to manage your tasks efficiently.</p>
          </div>

          <form className="auth-form" onSubmit={handleSubmit}>
            <div className="field-wrap">
              <label htmlFor="email">Email Address</label>
              <div className="input-icon-wrap">
                 
                <input
                  id="email"
                  type="email"
                  placeholder="alex.rivera@example.com"
                  value={email}
                  required
                  onChange={(event) => setEmail(event.target.value)}
                />
              </div>
            </div>

            <div className="field-wrap">
              <label htmlFor="password">Password</label>
              <div className="input-icon-wrap">
                
                <input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  value={password}
                  required
                  onChange={(event) => setPassword(event.target.value)}
                />
              </div>
            </div>

            <div className="helper-row">
              <label className="remember-me">
                <input
                  type="checkbox"
                  checked={rememberMe}
                  onChange={(event) => setRememberMe(event.target.checked)}
                />
                <span>Remember Me</span>
              </label>
              <Link to="/forgot-password">Forgot Password?</Link>
            </div>

            {error && <div className="form-error">{error}</div>}

            <button type="submit" className="primary-button auth-submit" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In'}
              <span className="material-symbols-outlined">login</span>
            </button>
          </form>

          <div className="auth-footer">
            <p>
              Don&apos;t have an account?
              <Link to="/register">Sign up for a new account</Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
};

export default LoginPage;
