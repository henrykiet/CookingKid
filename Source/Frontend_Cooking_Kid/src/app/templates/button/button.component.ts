import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      class="{{ variant }} {{ class }}"
      [ngClass]="customClass"
      [attr.style]="style"
      [type]="type"
      (click)="onClick.emit($event)"
    >
      <i *ngIf="icon" [class]="icon + ' ' + classIcon" [style]="styleIcon"></i>
      <span [class]="classSpan" [style]="styleSpan"> {{ label }}</span>
    </button>
  `,
  styleUrl: './button.component.scss',
})
export class ButtonComponent {
  @Input() customClass?: any;
  @Input() variant: string = '';
  @Input() class?: string;
  @Input() classIcon?: string;
  @Input() classSpan?: string;
  @Input() label?: string;
  @Input() icon?: string;
  @Input() type: string = 'button';
  @Input() style?: string;
  @Input() styleIcon?: string;
  @Input() styleSpan?: string;
  @Output() onClick = new EventEmitter<Event>();
}
