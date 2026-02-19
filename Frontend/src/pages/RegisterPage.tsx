import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';
import { useAuth } from '../context/AuthContext';
import { getErrorMessage } from '../middleware/apiClient';

export default function RegisterPage() {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const user = await authService.register({ username, email, password });
      login(user);
      navigate('/dashboard');
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Register</h2>
      <hr />
      {error && <p className="error">{error}</p>}
      <form onSubmit={handleSubmit}>
        <label>Username</label>
        <input value={username} onChange={e => setUsername(e.target.value)} required />
        <label>Correo</label>
        <input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
        <label>Contraseña</label>
        <input type="password" value={password} onChange={e => setPassword(e.target.value)} required minLength={6} />
        <button type="submit" disabled={loading}>{loading ? 'Loading...' : 'Register'}</button>
      </form>
      <hr />
      <p>Crear Cuenta <Link to="/login">Login</Link></p>
    </div>
  );
}
