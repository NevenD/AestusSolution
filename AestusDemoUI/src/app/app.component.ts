import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { animate, style, transition, trigger } from '@angular/animations';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatCardModule, MatButtonModule],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-20px)' }),
        animate(
          '600ms ease-out',
          style({ opacity: 1, transform: 'translateY(0)' })
        ),
      ]),
    ]),
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  constructor(private router: Router) {}
  title = 'AestusDemoUI';

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}
