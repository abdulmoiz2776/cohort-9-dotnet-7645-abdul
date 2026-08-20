import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const formatDate = (value) => {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleDateString();
};

const AssignedTasksPage = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadTasks = async () => {
      try {
        const response = await api.get('/api/tasks?page=1&pageSize=20');
        const filtered = (response.data.items || []).filter((item) => item.assignedToUserId);
        setTasks(filtered);
      } catch (loadError) {
        setError(loadError.response?.data?.message || 'Unable to load assigned tasks.');
      } finally {
        setLoading(false);
      }
    };

    loadTasks();
  }, []);

  return (
    <PageLayout title="Assigned Tasks">
      {error && <div className="form-error">{error}</div>}
      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Task</th>
              <th>Owner</th>
              <th>Status</th>
              <th>Priority</th>
              <th>Due Date</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan="5" className="empty-state">Loading assigned tasks...</td></tr>
            ) : tasks.length === 0 ? (
              <tr><td colSpan="5" className="empty-state">No assigned tasks.</td></tr>
            ) : (
              tasks.map((task) => (
                <tr key={task.id}>
                  <td><Link className="link-text" to={`/tasks/${task.id}`}>{task.title}</Link></td>
                  <td>{task.createdByUserId}</td>
                  <td><span className={`badge ${task.status === 'Completed' ? 'success' : task.status === 'InProgress' ? 'warning' : task.status === 'Blocked' ? 'danger' : 'muted'}`}>{task.status}</span></td>
                  <td>{task.priority}</td>
                  <td>{formatDate(task.dueDate)}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </PageLayout>
  );
};

export default AssignedTasksPage;
