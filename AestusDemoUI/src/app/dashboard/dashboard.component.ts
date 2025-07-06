import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
} from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { fadeIn } from '../helpers/animations';
import { finalize, Subject, takeUntil } from 'rxjs';
import { DashboardService } from '../../services/dashboard.service';
import {
  DailySuspiciousSummaryDto,
  DashboardDto,
  TransactionDto,
} from '../../entities/models';
import { CurrencyPipe } from '@angular/common';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatCardModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatChipsModule,
    CurrencyPipe,
    MatPaginatorModule,
  ],
  animations: [fadeIn],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  providers: [{ provide: MatPaginatorIntl }],
})
export class DashboardComponent implements OnInit, OnDestroy, AfterViewInit {
  componentDestroyed$: Subject<void> = new Subject<void>();
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  selectedpageNumber: number = 1;
  selectedPageSize: number = 5;
  selectedPageIndex: number = 0;
  resultsLength: number = 0;

  isLoading: boolean = true;
  totalTransactions: number = 0;
  totalAmount: number = 0;
  suspiciousTransactionsCount: number = 0;
  dailySuspiciousSummary: DailySuspiciousSummaryDto[] = [];
  transactions: TransactionDto[] = [];
  dataSource!: MatTableDataSource<TransactionDto>;

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

  displayedColumns: string[] = [
    'userId',
    'amountWithCurrency',
    'formatedDate',
    'location',
    'comment',
    'isSuspicious',
  ];

  ngOnInit(): void {
    this.getAdminData();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
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
        this.dailySuspiciousSummary = data.dailySuspiciousSummary;
        this.totalTransactions = data.totalTransactions;
        this.totalAmount = data.totalAmount;
        this.suspiciousTransactionsCount = data.suspiciousTransactionsCount;
        this.transactions = data.transactions;
        this.dataSource = new MatTableDataSource(this.transactions);
        this.resultsLength = this.transactions.length;
        setTimeout(() => {
          if (this.paginator) {
            this.dataSource.paginator = this.paginator;
          }
        });
      });
  }

  changePage(pageEvent: PageEvent): void {
    const page = pageEvent.pageIndex + 1;
    this.selectedPageIndex = pageEvent.pageIndex;
    this.selectedpageNumber = page;
    this.selectedPageSize = pageEvent.pageSize;
  }

  ngOnDestroy(): void {
    this.componentDestroyed$.next();
    this.componentDestroyed$.complete();
  }
}
