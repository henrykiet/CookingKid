import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  // changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent implements OnInit {
  ngOnInit(): void {
    console.log('Login component initialized');
  }
  onSubmit() {
    throw new Error('Method not implemented.');
  }
}
