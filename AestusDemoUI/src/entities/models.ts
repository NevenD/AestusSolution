export interface TransactionDto {
  id: number | undefined;
  userId: string;
  amount: number;
  amountWithCurrency: string;
  timestamp: string;
  location: string;
  isSuspicious: boolean;
  comment: string;
}

export interface DashboardDto {
  totalTransactions: number;
  totalAmount: number;
  suspiciousTransactionsCount: number;
  dailySuspiciousSummary: DailySuspiciousSummaryDto[];
  transactions: TransactionDto[];
}

export interface SuspiciousTransactionDto {
  userId: string;
  amount: number;
  amountWithCurrency: string;
  timestamp: string;
  location: string;
  comment: string;
}

export interface DailySuspiciousSummaryDto {
  userId: string;
  count: number;
  totalAmount: number;
}
