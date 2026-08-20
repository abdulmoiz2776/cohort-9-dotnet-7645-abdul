import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const statusMap = ['Pending', 'InProgress', 'Completed', 'Blocked'];

const formatDate = (value) => {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleDateString();
};

const MyTasksPage = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadTasks = async () => {
      try {
        const response = await api.get('/api/tasks?assignedToUserId=&page=1&pageSize=20');
        setTasks(response.data.items || []);
      } catch (loadError) {
        setError(loadError.response?.data?.message || 'Unable to load tasks.');
      } finally {
        setLoading(false);
      }
    };

    loadTasks();
  }, []);

  return (
    <PageLayout title="My Tasks">
      {error && <div className="form-error">{error}</div>}
      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Status</th>
              <th>Priority</th>
              <th>Category</th>
              <th>Due Date</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr><td colSpan="5" className="empty-state">Loading tasks...</td></tr>
            ) : tasks.length === 0 ? (
              <tr><td colSpan="5" className="empty-state">No tasks yet.</td></tr>
            ) : (
              tasks.map((task) => (
                <tr key={task.id}>
                  <td><Link className="link-text" to={`/tasks/${task.id}`}>{task.title}</Link></td>
                  <td><span className={statusMap.includes(task.status) ? `badge ${task.status === 'Completed' ? 'success' : task.status === 'InProgress' ? 'warning' : task.status === 'Blocked' ? 'danger' : 'muted'}` : 'badge muted'}>{task.status}</span></td>
                  <td>{task.priority}</td>
                  <td>{task.categoryName || 'Uncategorized'}</td>
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

export default MyTasksPage;
