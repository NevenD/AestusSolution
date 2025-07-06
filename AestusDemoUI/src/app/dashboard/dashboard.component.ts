import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { fadeIn } from '../helpers/animations';
import { finalize, Subject, takeUntil } from 'rxjs';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardDto, TransactionDto } from '../../entities/models';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule, MatTableModule, MatProgressSpinnerModule],
  animations: [fadeIn],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit, OnDestroy {
  componentDestroyed$: Subject<void> = new Subject<void>();
  isLoading: boolean = true;
  totalTransactions: number = 0;
  totalAmount: number = 0;
  suspiciousTransactionsCount: number = 0;
  transactions: TransactionDto[] = [];

  constructor(private _dashboardService: DashboardService) {}

  chartType = 'line';
  chartLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'];
  chartData = [{ data: [12, 19, 14, 22, 30], label: 'Visits' }];
  chartOptions = {
    responsive: true,
    scales: {
      y: { beginAtZero: true },
    },
  };

  displayedColumns: string[] = ['name', 'status', 'lastActive'];
  tableData = [
    { name: 'Alice', status: 'Online', lastActive: '2 min ago' },
    { name: 'Bob', status: 'Offline', lastActive: '1 hour ago' },
    { name: 'Charlie', status: 'Busy', lastActive: '5 min ago' },
  ];

  ngOnInit(): void {
    this.getAdminData();
  }

  private getAdminData(): void {
    this.isLoading = true;
    this._dashboardService
      .getDashboardData()
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
        takeUntil(this.componentDestroyed$)
      )
      .subscribe((data: DashboardDto) => {
        this.totalTransactions = data.totalTransactions;
        this.totalAmount = data.totalAmount;
        this.transactions = data.transactions;
      });
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
    this.componentDestroyed$.complete();
  }
}
