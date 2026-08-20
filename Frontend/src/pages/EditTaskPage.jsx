import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const EditTaskPage = () => {
  const { taskId } = useParams();
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [form, setForm] = useState({
    title: '',
    description: '',
    priority: 'Medium',
    categoryId: '',
    assignedToUserId: '',
    dueDate: ''
  });
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadTask = async () => {
      try {
        const [taskResponse, categoriesResponse] = await Promise.all([
          api.get(`/api/tasks/${taskId}`),
          api.get('/api/categories')
        ]);

        const task = taskResponse.data;
        setCategories(categoriesResponse.data || []);
        setForm({
          title: task.title || '',
          description: task.description || '',
          priority: task.priority || 'Medium',
          categoryId: task.categoryId || '',
          assignedToUserId: task.assignedToUserId || '',
          dueDate: task.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : ''
        });
      } catch (loadError) {
        setError(loadError.response?.data?.message || 'Unable to load task.');
      } finally {
        setLoading(false);
      }
    };

    if (taskId) {
      loadTask();
    }
  }, [taskId]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((previous) => ({ ...previous, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setSubmitting(true);
    setError('');

    try {
      await api.put(`/api/tasks/${taskId}`, {
        title: form.title,
        description: form.description,
        priority: form.priority,
        categoryId: form.categoryId || null,
        assignedToUserId: form.assignedToUserId || null,
        dueDate: form.dueDate || null
      });

      navigate(`/tasks/${taskId}`);
    } catch (submitError) {
      setError(submitError.response?.data?.message || 'Unable to update task.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <PageLayout title="Edit Task"><p>Loading task...</p></PageLayout>;
  }

  return (
    <PageLayout title="Edit Task">
      <form className="card form-card" onSubmit={handleSubmit}>
        {error && <div className="form-error">{error}</div>}

        <label>
          Title
          <input type="text" name="title" value={form.title} onChange={handleChange} required />
        </label>

        <label>
          Description
          <textarea name="description" rows="4" value={form.description} onChange={handleChange} />
        </label>

        <div className="two-column-grid">
          <label>
            Priority
            <select name="priority" value={form.priority} onChange={handleChange}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </label>

          <label>
            Category
            <select name="categoryId" value={form.categoryId} onChange={handleChange}>
              <option value="">Uncategorized</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="two-column-grid">
          <label>
            Assigned to user ID
            <input type="text" name="assignedToUserId" value={form.assignedToUserId} onChange={handleChange} />
          </label>

          <label>
            Due date
            <input type="date" name="dueDate" value={form.dueDate} onChange={handleChange} />
          </label>
        </div>

        <button type="submit" className="primary-button" disabled={submitting}>
          {submitting ? 'Saving...' : 'Save changes'}
        </button>
      </form>
    </PageLayout>
  );
};

export default EditTaskPage;
