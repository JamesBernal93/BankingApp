import { apiClient } from '../middleware/apiClient';
import { BankAccount, Transaction, BalanceResponse } from '../types';

export const accountsService = {
  async getAll(): Promise<BankAccount[]> {
    const res = await apiClient.get<BankAccount[]>('/accounts');
    return res.data;
  },

  async getById(id: string): Promise<BankAccount> {
    const res = await apiClient.get<BankAccount>(`/accounts/${id}`);
    return res.data;
  },

  async create(data: { ownerName: string; email: string; initialBalance: number }): Promise<BankAccount> {
    const res = await apiClient.post<BankAccount>('/accounts', data);
    return res.data;
  },

  async getBalance(id: string): Promise<BalanceResponse> {
    const res = await apiClient.get<BalanceResponse>(`/accounts/${id}/balance`);
    return res.data;
  },

  async deposit(id: string, data: { amount: number; description: string }): Promise<BankAccount> {
    const res = await apiClient.post<BankAccount>(`/accounts/${id}/deposit`, { accountId: id, ...data });
    return res.data;
  },

  async getTransactions(id: string): Promise<Transaction[]> {
    const res = await apiClient.get<Transaction[]>(`/accounts/${id}/transactions`);
    return res.data;
  },
};

export const transfersService = {
  async transfer(data: {
    sourceAccountId: string;
    destinationAccountNumber: string;
    amount: number;
    description: string;
  }): Promise<void> {
    await apiClient.post('/transfers', data);
  },
};
