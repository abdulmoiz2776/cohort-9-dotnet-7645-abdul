import { useEffect, useState } from 'react';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const SessionsPage = () => {
  const { getSessions, revokeSession } = useAuth();
  const [sessions, setSessions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadSessions = async () => {
      try {
        const result = await getSessions();
        setSessions(result || []);
      } catch (ex) {
        setError(ex.message || 'Unable to load sessions.');
      } finally {
        setLoading(false);
      }
    };

    loadSessions();
  }, [getSessions]);

  const handleRevoke = async (sessionId) => {
    try {
      await revokeSession(sessionId);
      setSessions((prev) => prev.filter((s) => s.id !== sessionId));
    } catch (ex) {
      setError(ex.message || 'Unable to revoke session.');
    }
  };

  return (
    <PageLayout title="Active Sessions">
      <div className="auth-form">
        {loading && <p>Loading sessions...</p>}
        {error && <div className="form-error">{error}</div>}
        {!loading && !sessions.length && <p>No active sessions found.</p>}
        {!loading && sessions.length > 0 && (
          <table className="sessions-table">
            <thead>
              <tr>
                <th>Device</th>
                <th>IP</th>
                <th>Issued</th>
                <th>Expires</th>
                <th>Current</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {sessions.map((session) => (
                <tr key={session.sessionId}>
                  <td>{session.deviceName || session.browser || 'Unknown'}</td>
                  <td>{session.ipAddress || 'N/A'}</td>
                  <td>{new Date(session.createdAt).toLocaleString()}</td>
                  <td>{new Date(session.expiresAt).toLocaleString()}</td>
                  <td>{session.isCurrentDevice ? 'Yes' : 'No'}</td>
                  <td>
                    {!session.isCurrentDevice && (
                      <button onClick={() => handleRevoke(session.sessionId)}>
                        Revoke
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </PageLayout>
  );
};

export default SessionsPage;
