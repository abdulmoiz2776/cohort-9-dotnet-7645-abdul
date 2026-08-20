import { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/httpClient';

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setMessage('');
    setError('');
    setLoading(true);

    try {
      const response = await api.post('/api/auth/forgot-password', { email });
      setMessage(response.data?.message || 'If an account exists, a reset link has been sent.');
      setEmail('');
    } catch (submitError) {
      setError(submitError.response?.data?.message || 'Unable to send reset link.');
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
          <div className="auth-header-copy compact-head">
            <div className="icon-circle">
              <span className="material-symbols-outlined">key</span>
            </div>
            <h1>Forgot Password</h1>
            <p>Enter your email and we&apos;ll send you a link to reset your password.</p>
          </div>

          <form className="auth-form" onSubmit={handleSubmit}>
            <div className="field-wrap">
              <label htmlFor="forgot-email">Email Address</label>
              <div className="input-icon-wrap">
                <span className="material-symbols-outlined">mail</span>
                <input
                  id="forgot-email"
                  type="email"
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  placeholder="name@example.com"
                  required
                />
              </div>
            </div>

            {error && <div className="form-error">{error}</div>}
            {message && <div className="form-success">{message}</div>}

            <button type="submit" className="primary-button auth-submit" disabled={loading}>
              {loading ? 'Sending...' : 'Send Reset Link'}
            </button>
          </form>

          <div className="auth-footer">
            <Link to="/login">
              <span className="material-symbols-outlined">arrow_back</span>
              Back to Login
            </Link>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ForgotPasswordPage;
