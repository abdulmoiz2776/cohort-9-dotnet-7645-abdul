import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const HomePage = () => {
  const { isAuthenticated, user } = useAuth();
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!isAuthenticated) return;

    const loadDashboardData = async () => {
      setLoading(true);
      try {
        const response = await api.get('/api/tasks?page=1&pageSize=100');
        setTasks(response.data.items || []);
      } catch {
        setTasks([]);
      } finally {
        setLoading(false);
      }
    };

    loadDashboardData();
  }, [isAuthenticated]);

  const summary = useMemo(() => {
    const totals = {
      all: tasks.length,
      completed: tasks.filter((task) => task.status === 'Completed').length,
      inProgress: tasks.filter((task) => task.status === 'InProgress').length,
      pending: tasks.filter((task) => task.status === 'Pending').length,
      blocked: tasks.filter((task) => task.status === 'Blocked').length,
      highPriority: tasks.filter((task) => task.priority === 'High').length
    };

    const statusCounts = ['Pending', 'InProgress', 'Completed', 'Blocked'].map((status) => ({
      label: status,
      value: totals[status === 'InProgress' ? 'inProgress' : status === 'Completed' ? 'completed' : status === 'Pending' ? 'pending' : 'blocked']
    }));

    const maxValue = Math.max(1, ...statusCounts.map((entry) => entry.value));

    return { totals, statusCounts, maxValue };
  }, [tasks]);

  if (!isAuthenticated) {
    return (
      <PageLayout title="Welcome">
        <section className="home-hero card">
          <div>
            <p className="eyebrow">TaskMaster</p>
            <h3>Stay on top of work across your team.</h3>
            <p>
              Sign in to manage tasks, track progress, update priorities, and keep everyone aligned.
            </p>
          </div>
          <div className="hero-actions">
            <Link className="primary-button" to="/login">Sign in</Link>
            <Link className="secondary-button" to="/register">Create account</Link>
          </div>
        </section>
      </PageLayout>
    );
  }

  return (
    <PageLayout title="Dashboard" subtitle={`Welcome back, ${user?.firstName || user?.username || 'User'}!`}>
      <section className="dashboard-grid">
        <div className="stat-card">
          <span>Total Tasks</span>
          <strong>{summary.totals.all}</strong>
        </div>
        <div className="stat-card accent">
          <span>Completed</span>
          <strong>{summary.totals.completed}</strong>
        </div>
        <div className="stat-card warning">
          <span>In Progress</span>
          <strong>{summary.totals.inProgress}</strong>
        </div>
        <div className="stat-card danger">
          <span>High Priority</span>
          <strong>{summary.totals.highPriority}</strong>
        </div>
      </section>

      <section className="dashboard-panels">
        <div className="chart-card card">
          <div className="panel-header">
            <h3>Task Status</h3>
            <span>{loading ? 'Loading...' : `${summary.totals.all} total`}</span>
          </div>

          <div className="bar-chart" aria-label="Task status chart">
            {summary.statusCounts.map((entry) => (
              <div key={entry.label} className="bar-group">
                <div className="bar-value">{entry.value}</div>
                <div className="bar-track">
                  <div
                    className={`bar-fill ${entry.label === 'Completed' ? 'success' : entry.label === 'InProgress' ? 'warning' : entry.label === 'Blocked' ? 'danger' : 'neutral'}`}
                    style={{ height: `${(entry.value / summary.maxValue) * 100}%` }}
                  />
                </div>
                <div className="bar-label">{entry.label}</div>
              </div>
            ))}
          </div>
        </div>

        <div className="card insights-card">
          <div className="panel-header">
            <h3>Quick Insights</h3>
          </div>

          <ul className="insights-list">
            <li>
              <span className="dot success" />
              {summary.totals.completed} tasks completed
            </li>
            <li>
              <span className="dot warning" />
              {summary.totals.pending} awaiting action
            </li>
            <li>
              <span className="dot danger" />
              {summary.totals.blocked} blocked items need attention
            </li>
            <li>
              <span className="dot neutral" />
              {summary.totals.highPriority} high-priority tasks
            </li>
          </ul>
        </div>
      </section>
    </PageLayout>
  );
};

export default HomePage;
