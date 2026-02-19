import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { accountsService } from '../services/accountsService';
import { useAuth } from '../context/AuthContext';
import { BankAccount } from '../types';
import { getErrorMessage } from '../middleware/apiClient';

export default function DashboardPage() {
  const { user, logout } = useAuth();
  const [accounts, setAccounts] = useState<BankAccount[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Create account form
  const [ownerName, setOwnerName] = useState('');
  const [email, setEmail] = useState('');
  const [initialBalance, setInitialBalance] = useState('0');
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');
  const [showForm, setShowForm] = useState(false);

  useEffect(() => {
    accountsService.getAll()
      .then(setAccounts)
      .catch(err => setError(getErrorMessage(err)))
      .finally(() => setLoading(false));
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setCreateError('');
    setCreating(true);
    try {
      const acc = await accountsService.create({
        ownerName,
        email,
        initialBalance: parseFloat(initialBalance),
      });
      setAccounts(prev => [...prev, acc]);
      setOwnerName(''); setEmail(''); setInitialBalance('0');
      setShowForm(false);
    } catch (err) {
      setCreateError(getErrorMessage(err));
    } finally {
      setCreating(false);
    }
  };

  return (
    <div>
      <p>Logged in as <strong>{user?.username}</strong> — <button onClick={logout}>Logout</button></p>
      <h2>Dashboard</h2>
      <hr />

      {error && <p className="error">{error}</p>}
      {loading && <p>Loading...</p>}

      <h3>My Accounts</h3>
      <button onClick={() => setShowForm(!showForm)}>
        {showForm ? 'Cancel' : 'New Account'}
      </button>

      {showForm && (
        <div>
          <br />
          <h4>Create Account</h4>
          {createError && <p className="error">{createError}</p>}
          <form onSubmit={handleCreate}>
            <label>Owner Name</label>
            <input value={ownerName} onChange={e => setOwnerName(e.target.value)} required />
            <label>Email</label>
            <input type="email" value={email} onChange={e => setEmail(e.target.value)} required />
            <label>Initial Balance ($)</label>
            <input type="number" min="0" step="0.01" value={initialBalance}
              onChange={e => setInitialBalance(e.target.value)} required />
            <button type="submit" disabled={creating}>{creating ? 'Creating...' : 'Create'}</button>
          </form>
        </div>
      )}

      <br />
      {!loading && accounts.length === 0 && <p>No accounts yet.</p>}
      {accounts.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Account Number</th>
              <th>Owner</th>
              <th>Balance</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {accounts.map(a => (
              <tr key={a.id}>
                <td>{a.accountNumber}</td>
                <td>{a.ownerName}</td>
                <td>${a.balance.toFixed(2)}</td>
                <td>{a.isActive ? 'Active' : 'Inactive'}</td>
                <td><Link to={`/accounts/${a.id}`}>View</Link></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
