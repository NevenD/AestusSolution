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
  timestamp: string;
  amountWithCurrency: string;
  location: string;
  comment: string;
}
