import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { fadeIn } from '../helpers/animations';

@Component({
  selector: 'app-home',
  imports: [MatButtonModule, MatCardModule],
  standalone: true,
  animations: [fadeIn],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  constructor(private router: Router) {}

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}
