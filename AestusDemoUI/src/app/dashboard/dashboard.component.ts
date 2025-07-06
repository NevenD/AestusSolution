import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { fadeIn } from '../helpers/animations';
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule, MatTableModule],
  animations: [fadeIn],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {
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
}
