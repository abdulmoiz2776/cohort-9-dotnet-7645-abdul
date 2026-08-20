import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const defaultFilters = {
  searchTerm: '',
  status: '',
  priority: '',
  categoryId: '',
  assignedToUserId: '',
  dueDate: ''
};

const formatDate = (value) => {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleDateString();
};

const statusClassName = (status) => {
  switch (status) {
    case 'Completed':
      return 'badge success';
    case 'InProgress':
      return 'badge warning';
    case 'Blocked':
      return 'badge danger';
    default:
      return 'badge muted';
  }
};

const priorityClassName = (priority) => {
  switch (priority) {
    case 'High':
      return 'badge danger';
    case 'Medium':
      return 'badge warning';
    case 'Low':
      return 'badge success';
    default:
      return 'badge muted';
  }
};

const TaskListPage = () => {
  const [tasks, setTasks] = useState([]);
  const [filters, setFilters] = useState(defaultFilters);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [meta, setMeta] = useState({ totalCount: 0, pageSize: 10 });

  useEffect(() => {
    const loadTasks = async () => {
      setLoading(true);
      setError('');

      try {
        const params = new URLSearchParams();
        params.set('page', String(page));
        params.set('pageSize', '10');

        Object.entries(filters).forEach(([key, value]) => {
          if (value) {
            params.set(key, value);
          }
        });

        const response = await api.get(`/api/tasks?${params.toString()}`);
        setTasks(response.data.items || []);
        setMeta({
          totalCount: response.data.totalCount || 0,
          pageSize: response.data.pageSize || 10
        });
      } catch (fetchError) {
        setError(fetchError.response?.data?.message || 'Unable to load tasks.');
      } finally {
        setLoading(false);
      }
    };

    loadTasks();
  }, [filters, page]);

  const handleFilterChange = (event) => {
    const { name, value } = event.target;
    setFilters((previous) => ({ ...previous, [name]: value }));
    setPage(1);
  };

  const clearFilters = () => {
    setFilters(defaultFilters);
    setPage(1);
  };

  return (
    <PageLayout title="Task List">
      <div className="toolbar">
        <div className="toolbar-group wide">
          <input
            type="search"
            placeholder="Search tasks"
            value={filters.searchTerm}
            name="searchTerm"
            onChange={handleFilterChange}
          />
        </div>
        <div className="toolbar-group">
          <select name="status" value={filters.status} onChange={handleFilterChange}>
            <option value="">All statuses</option>
            <option value="Pending">Pending</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Blocked">Blocked</option>
          </select>
        </div>
        <div className="toolbar-group">
          <select name="priority" value={filters.priority} onChange={handleFilterChange}>
            <option value="">All priorities</option>
            <option value="1">Low</option>
            <option value="2">Medium</option>
            <option value="3">High</option>
            <option value="4">Critical</option>
          </select>
        </div>
        <div className="toolbar-group">
          <input
            type="date"
            name="dueDate"
            value={filters.dueDate}
            onChange={handleFilterChange}
          />
        </div>
        <button type="button" className="secondary-button" onClick={clearFilters}>
          Clear
        </button>
        <Link className="primary-button" to="/tasks/new">
          + New Task
        </Link>
      </div>

      {error && <div className="form-error">{error}</div>}

      <div className="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Priority</th>
              <th>Status</th>
              <th>Category</th>
              <th>Assigned</th>
              <th>Due Date</th>
              <th>Updated</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan="7" className="empty-state">
                  Loading tasks...
                </td>
              </tr>
            ) : tasks.length === 0 ? (
              <tr>
                <td colSpan="7" className="empty-state">
                  No tasks found.
                </td>
              </tr>
            ) : (
              tasks.map((task) => (
                <tr key={task.id}>
                  <td>
                    <Link className="link-text" to={`/tasks/${task.id}`}>
                      {task.title}
                    </Link>
                  </td>
                  <td>
                    <span className={priorityClassName(task.priority)}>{task.priority}</span>
                  </td>
                  <td>
                    <span className={statusClassName(task.status)}>{task.status}</span>
                  </td>
                  <td>{task.categoryName || 'Uncategorized'}</td>
                  <td>{task.assignedToUsername || 'Unassigned'}</td>
                  <td>{formatDate(task.dueDate)}</td>
                  <td>{formatDate(task.updatedAt || task.createdAt)}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="pagination-row">
        <button
          type="button"
          className="secondary-button"
          disabled={page <= 1}
          onClick={() => setPage((current) => current - 1)}
        >
          Previous
        </button>
        <span>
          Page {page} • {meta.totalCount} tasks
        </span>
        <button
          type="button"
          className="secondary-button"
          disabled={(page * meta.pageSize) >= meta.totalCount}
          onClick={() => setPage((current) => current + 1)}
        >
          Next
        </button>
      </div>
    </PageLayout>
  );
};

export default TaskListPage;
