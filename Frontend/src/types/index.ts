export interface User {
  userId: string;
  username: string;
  token: string;
  expiresAt: string;
}

export interface BankAccount {
  id: string;
  accountNumber: string;
  ownerName: string;
  email: string;
  balance: number;
  isActive: boolean;
  createdAt: string;
}

export interface Transaction {
  id: string;
  accountId: string;
  type: 'Credit' | 'Debit' | 'Transfer';
  amount: number;
  balanceAfter: number;
  description: string;
  createdAt: string;
  relatedAccountId?: string;
}

export interface BalanceResponse {
  accountId: string;
  accountNumber: string;
  balance: number;
}

export interface ApiError {
  error: string;
  statusCode: number;
  timestamp: string;
}
