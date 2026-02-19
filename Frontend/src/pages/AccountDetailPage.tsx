import React, { useState, useEffect, useCallback } from 'react';
import { useParams, Link } from 'react-router-dom';
import { accountsService, transfersService } from '../services/accountsService';
import { BankAccount, Transaction } from '../types';
import { getErrorMessage } from '../middleware/apiClient';

export default function AccountDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [account, setAccount] = useState<BankAccount | null>(null);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);

  // Deposit form
  const [depositAmount, setDepositAmount] = useState('');
  const [depositDesc, setDepositDesc] = useState('Deposit');
  const [depositError, setDepositError] = useState('');
  const [depositLoading, setDepositLoading] = useState(false);

  // Transfer form
  const [destAccount, setDestAccount] = useState('');
  const [transferAmount, setTransferAmount] = useState('');
  const [transferDesc, setTransferDesc] = useState('Transfer');
  const [transferError, setTransferError] = useState('');
  const [transferLoading, setTransferLoading] = useState(false);
  const [transferSuccess, setTransferSuccess] = useState('');

  const load = useCallback(async () => {
    if (!id) return;
    try {
      const [acc, txs] = await Promise.all([
        accountsService.getById(id),
        accountsService.getTransactions(id),
      ]);
      setAccount(acc);
      setTransactions(txs);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { load(); }, [load]);

  const handleDeposit = async (e: React.FormEvent) => {
    e.preventDefault();
    setDepositError('');
    setDepositLoading(true);
    try {
      await accountsService.deposit(id!, { amount: parseFloat(depositAmount), description: depositDesc });
      setDepositAmount('');
      await load();
    } catch (err) {
      setDepositError(getErrorMessage(err));
    } finally {
      setDepositLoading(false);
    }
  };

  const handleTransfer = async (e: React.FormEvent) => {
    e.preventDefault();
    setTransferError('');
    setTransferSuccess('');
    setTransferLoading(true);
    try {
      await transfersService.transfer({
        sourceAccountId: id!,
        destinationAccountNumber: destAccount,
        amount: parseFloat(transferAmount),
        description: transferDesc,
      });
      setTransferSuccess('Transfer completed successfully.');
      setDestAccount(''); setTransferAmount('');
      await load();
    } catch (err) {
      setTransferError(getErrorMessage(err));
    } finally {
      setTransferLoading(false);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (!account) return <p>Account not found.</p>;

  return (
    <div>
      <Link to="/dashboard">← Back</Link>
      <hr />

      <h2>Account Detail</h2>
      <p><strong>Account Number:</strong> {account.accountNumber}</p>
      <p><strong>Owner:</strong> {account.ownerName}</p>
      <p><strong>Email:</strong> {account.email}</p>
      <p><strong>Balance:</strong> ${account.balance.toFixed(2)}</p>
      <p><strong>Status:</strong> {account.isActive ? 'Active' : 'Inactive'}</p>
      <p><strong>Opened:</strong> {new Date(account.createdAt).toLocaleDateString()}</p>

      {account.isActive && (
        <>
          <hr />
          <h3>Deposit</h3>
          {depositError && <p className="error">{depositError}</p>}
          <form onSubmit={handleDeposit}>
            <label>Amount ($)</label>
            <input type="number" min="0.01" step="0.01" value={depositAmount}
              onChange={e => setDepositAmount(e.target.value)} required />
            <label>Description</label>
            <input value={depositDesc} onChange={e => setDepositDesc(e.target.value)} />
            <button type="submit" disabled={depositLoading}>
              {depositLoading ? 'Processing...' : 'Deposit'}
            </button>
          </form>

          <hr />
          <h3>Transfer</h3>
          {transferError && <p className="error">{transferError}</p>}
          {transferSuccess && <p className="success">{transferSuccess}</p>}
          <form onSubmit={handleTransfer}>
            <label>Destination Account Number</label>
            <input value={destAccount} onChange={e => setDestAccount(e.target.value)}
              placeholder="ACC123456789" required />
            <label>Amount ($)</label>
            <input type="number" min="0.01" step="0.01" value={transferAmount}
              onChange={e => setTransferAmount(e.target.value)} required />
            <label>Description</label>
            <input value={transferDesc} onChange={e => setTransferDesc(e.target.value)} />
            <button type="submit" disabled={transferLoading}>
              {transferLoading ? 'Transferring...' : 'Transfer'}
            </button>
          </form>
        </>
      )}

      <hr />
      <h3>Transaction History</h3>
      {transactions.length === 0 ? (
        <p>No transactions yet.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Type</th>
              <th>Amount</th>
              <th>Balance After</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            {transactions.map(t => (
              <tr key={t.id}>
                <td>{new Date(t.createdAt).toLocaleString()}</td>
                <td>{t.type}</td>
                <td>${t.amount.toFixed(2)}</td>
                <td>${t.balanceAfter.toFixed(2)}</td>
                <td>{t.description}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
