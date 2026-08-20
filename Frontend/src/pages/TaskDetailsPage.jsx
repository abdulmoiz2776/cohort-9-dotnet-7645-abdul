import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const formatDate = (value) => {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleString();
};

const TaskDetailsPage = () => {
  const { taskId } = useParams();
  const navigate = useNavigate();
  const [task, setTask] = useState(null);
  const [status, setStatus] = useState('Pending');
  const [assignmentUserId, setAssignmentUserId] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadTask = async () => {
      try {
        const response = await api.get(`/api/tasks/${taskId}`);
        setTask(response.data);
        setStatus(response.data.status || 'Pending');
        setAssignmentUserId(response.data.assignedToUserId || '');
      } catch (loadError) {
        setError(loadError.response?.data?.message || 'Unable to load task details.');
      } finally {
        setLoading(false);
      }
    };

    if (taskId) {
      loadTask();
    }
  }, [taskId]);

  const handleStatusChange = async () => {
    try {
      await api.post(`/api/tasks/${taskId}/status`, { status });
      const response = await api.get(`/api/tasks/${taskId}`);
      setTask(response.data);
    } catch (updateError) {
      setError(updateError.response?.data?.message || 'Unable to update task status.');
    }
  };

  const handleAssignment = async () => {
    try {
      await api.post(`/api/tasks/${taskId}/assign`, { assignedToUserId: assignmentUserId || '' });
      const response = await api.get(`/api/tasks/${taskId}`);
      setTask(response.data);
    } catch (updateError) {
      setError(updateError.response?.data?.message || 'Unable to assign task.');
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Delete this task?')) return;

    try {
      await api.delete(`/api/tasks/${taskId}`);
      navigate('/tasks');
    } catch (deleteError) {
      setError(deleteError.response?.data?.message || 'Unable to delete task.');
    }
  };

  if (loading) {
    return <PageLayout title="Task Details"><p>Loading task...</p></PageLayout>;
  }

  if (!task) {
    return <PageLayout title="Task Details"><div className="form-error">{error || 'Task not found.'}</div></PageLayout>;
  }

  return (
    <PageLayout title="Task Details">
      <div className="page-actions">
        <Link className="secondary-button" to="/tasks">Back to tasks</Link>
        <Link className="primary-button" to={`/tasks/${taskId}/edit`}>Edit task</Link>
      </div>

      {error && <div className="form-error">{error}</div>}

      <div className="detail-grid">
        <div className="card">
          <p className="eyebrow">Title</p>
          <h2>{task.title}</h2>
          <p className="task-description">{task.description || 'No description provided.'}</p>

          <div className="metadata-grid">
            <div><strong>Priority:</strong> {task.priority}</div>
            <div><strong>Status:</strong> {task.status}</div>
            <div><strong>Category:</strong> {task.categoryName || 'Uncategorized'}</div>
            <div><strong>Assigned user:</strong> {task.assignedToUsername || 'Unassigned'}</div>
            <div><strong>Created by:</strong> {task.createdByUsername || task.createdByUserId}</div>
            <div><strong>Due date:</strong> {formatDate(task.dueDate)}</div>
            <div><strong>Created:</strong> {formatDate(task.createdAt)}</div>
            <div><strong>Updated:</strong> {formatDate(task.updatedAt)}</div>
          </div>
        </div>

        <div className="card stacked-form">
          <h3>Update status</h3>
          <label>
            Status
            <select value={status} onChange={(event) => setStatus(event.target.value)}>
              <option value="Pending">Pending</option>
              <option value="InProgress">In Progress</option>
              <option value="Completed">Completed</option>
              <option value="Blocked">Blocked</option>
            </select>
          </label>
          <button type="button" className="primary-button" onClick={handleStatusChange}>Save status</button>

          <h3>Assignment</h3>
          <label>
            User ID
            <input
              type="text"
              value={assignmentUserId}
              onChange={(event) => setAssignmentUserId(event.target.value)}
              placeholder="Enter user id"
            />
          </label>
          <button type="button" className="secondary-button" onClick={handleAssignment}>Assign task</button>

          <button type="button" className="danger-button" onClick={handleDelete}>Delete task</button>
        </div>
      </div>
    </PageLayout>
  );
};

export default TaskDetailsPage;
