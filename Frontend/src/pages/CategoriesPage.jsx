import { useEffect, useState } from 'react';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const emptyForm = {
  name: '',
  description: '',
  isActive: true
};

const CategoriesPage = () => {
  const [categories, setCategories] = useState([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  const loadCategories = async () => {
    setLoading(true);
    try {
      const response = await api.get('/api/categories');
      setCategories(response.data || []);
    } catch (loadError) {
      setError(loadError.response?.data?.message || 'Unable to load categories.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;
    setForm((previous) => ({
      ...previous,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');

    try {
      if (editingId) {
        await api.put(`/api/categories/${editingId}`, form);
      } else {
        await api.post('/api/categories', form);
      }

      setForm(emptyForm);
      setEditingId(null);
      await loadCategories();
    } catch (submitError) {
      setError(submitError.response?.data?.message || 'Unable to save category.');
    }
  };

  const handleEdit = (category) => {
    setEditingId(category.id);
    setForm({
      name: category.name,
      description: category.description || '',
      isActive: category.isActive
    });
  };

  const handleDelete = async (categoryId) => {
    if (!window.confirm('Deactivate this category?')) return;

    try {
      await api.delete(`/api/categories/${categoryId}`);
      await loadCategories();
    } catch (deleteError) {
      setError(deleteError.response?.data?.message || 'Unable to remove category.');
    }
  };

  return (
    <PageLayout title="Task Categories">
      <div className="two-column-layout">
        <form className="card form-card" onSubmit={handleSubmit}>
          <h3>{editingId ? 'Update category' : 'Create category'}</h3>
          {error && <div className="form-error">{error}</div>}

          <label>
            Category name
            <input type="text" name="name" value={form.name} onChange={handleChange} required />
          </label>

          <label>
            Description
            <textarea name="description" rows="4" value={form.description} onChange={handleChange} />
          </label>

          <label className="checkbox-row">
            <input type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} />
            Active
          </label>

          <button type="submit" className="primary-button">
            {editingId ? 'Update category' : 'Create category'}
          </button>
        </form>

        <div className="card">
          <h3>Categories</h3>
          {loading ? (
            <p>Loading categories...</p>
          ) : categories.length === 0 ? (
            <p className="empty-state">No categories found.</p>
          ) : (
            <div className="list-stack">
              {categories.map((category) => (
                <div key={category.id} className="list-item">
                  <div>
                    <strong>{category.name}</strong>
                    <p>{category.description || 'No description'}</p>
                    <small>{category.isActive ? 'Active' : 'Inactive'}</small>
                  </div>
                  <div className="inline-actions">
                    <button type="button" className="secondary-button" onClick={() => handleEdit(category)}>Edit</button>
                    <button type="button" className="danger-button" onClick={() => handleDelete(category.id)}>Delete</button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </PageLayout>
  );
};

export default CategoriesPage;
