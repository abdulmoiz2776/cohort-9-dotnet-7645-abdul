import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const VerifyEmailPage = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus] = useState('Verifying your email...');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { verifyEmail } = useAuth();

  useEffect(() => {
    const verify = async () => {
      const userId = searchParams.get('userId');
      const token = searchParams.get('token');

      if (!userId || !token) {
        setError('Missing verification parameters.');
        setStatus('Unable to verify.');
        return;
      }

      try {
        const result = await verifyEmail({ userId, token });
        if (result.succeeded) {
          setStatus('Your email has been verified successfully.');
          setTimeout(() => navigate('/login'), 2500);
        } else {
          setError(result.message || 'Verification failed.');
          setStatus('Verification failed');
        }
      } catch (ex) {
        setError(ex.message || 'Unable to verify email.');
        setStatus('Verification failed');
      }
    };

    verify();
  }, [searchParams, verifyEmail, navigate]);

  return (
    <PageLayout title="Verify Email">
      <div className="auth-form">
        <p>{status}</p>
        {error && <div className="form-error">{error}</div>}
        {!error && <p>You will be redirected to login shortly.</p>}
      </div>
    </PageLayout>
  );
};

export default VerifyEmailPage;
