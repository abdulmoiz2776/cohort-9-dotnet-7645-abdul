import PageLayout from '../components/PageLayout';
import { useAuth } from '../contexts/AuthContext';

const HomePage = () => {
  const { isAuthenticated, user } = useAuth();

  return (
    <PageLayout title="Welcome">
      <section className="home-hero">
        <p>
          {isAuthenticated
            ? `Welcome back, ${user?.firstName || user?.username}!`
            : 'Sign in to manage users, sessions, and secure your account.'}
        </p>
      </section>
    </PageLayout>
  );
};

export default HomePage;
