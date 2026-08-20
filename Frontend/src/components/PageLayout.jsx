import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const PageLayout = ({ title, subtitle, children }) => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const navItemClass = ({ isActive }) =>
    `side-nav-item ${isActive ? 'active' : ''}`;

  return (
    <div className="app-shell">
      <aside className="side-nav">
        <div className="nav-header">
          <div className="profile-avatar">
            <img
              src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=200&q=80"
              alt="User profile"
            />
          </div>
          <div className="nav-user-meta">
            <h1>{user ? `${user.firstName || 'Task'} ${user.lastName || 'Master'}` : 'TaskMaster Admin'}</h1>
            <p>{user?.email || 'admin@taskmaster.com'}</p>
          </div>
        </div>

        <Link className="primary-cta" to="/tasks/new">
          <span className="material-symbols-outlined">add</span>
          New Task
        </Link>

        <nav className="side-nav-links">
          <NavLink to="/" className={navItemClass}>
            <span className="material-symbols-outlined">dashboard</span>
            Dashboard
          </NavLink>
          {isAuthenticated && (
            <NavLink to="/tasks" className={navItemClass}>
              <span className="material-symbols-outlined">assignment</span>
              Tasks
            </NavLink>
          )}
          {isAuthenticated && (
            <NavLink to="/tasks/mine" className={navItemClass}>
              <span className="material-symbols-outlined">fact_check</span>
              My Tasks
            </NavLink>
          )}
          {isAuthenticated && (
            <NavLink to="/tasks/assigned" className={navItemClass}>
              <span className="material-symbols-outlined">groups</span>
              Assigned
            </NavLink>
          )}
          {isAuthenticated && (
            <NavLink to="/profile" className={navItemClass}>
              <span className="material-symbols-outlined">person</span>
              Profile
            </NavLink>
          )}
          {user?.roles?.includes('Admin') && (
            <NavLink to="/users" className={navItemClass}>
              <span className="material-symbols-outlined">group</span>
              Users
            </NavLink>
          )}
          {user?.roles?.includes('Admin') && (
            <NavLink to="/categories" className={navItemClass}>
              <span className="material-symbols-outlined">category</span>
              Categories
            </NavLink>
          )}
          {user?.roles?.includes('Admin') && (
            <NavLink to="/users/roles" className={navItemClass}>
              <span className="material-symbols-outlined">security</span>
              Roles
            </NavLink>
          )}
          {isAuthenticated && (
            <NavLink to="/sessions" className={navItemClass}>
              <span className="material-symbols-outlined">devices</span>
              Sessions
            </NavLink>
          )}
        </nav>

        <div className="side-nav-footer">
          <NavLink to="/help" className="side-nav-item secondary">
            <span className="material-symbols-outlined">help</span>
            Help Center
          </NavLink>
          {isAuthenticated ? (
            <button className="logout-link" onClick={handleLogout} type="button">
              <span className="material-symbols-outlined">logout</span>
              Logout
            </button>
          ) : (
            <NavLink to="/login" className="side-nav-item secondary">
              <span className="material-symbols-outlined">login</span>
              Login
            </NavLink>
          )}
        </div>
      </aside>

      <main className="page-main">
        <header className="page-header-row">
          <div>
            <h2>{title || 'TaskMaster'}</h2>
            {subtitle && <p>{subtitle}</p>}
          </div>
          <div className="top-actions">
            <button type="button" className="ghost-button">
              <span className="material-symbols-outlined">download</span>
              Export
            </button>
            {isAuthenticated && (
              <Link className="primary-button small" to="/tasks/new">
                <span className="material-symbols-outlined">add</span>
                New
              </Link>
            )}
          </div>
        </header>

        <div className="page-content-shell">{children}</div>
      </main>
    </div>
  );
};

export default PageLayout;
