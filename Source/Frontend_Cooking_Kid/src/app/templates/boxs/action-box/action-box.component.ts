import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-action-box',
  standalone: true,
  imports: [CommonModule],
  template: `<ng-content></ng-content>`,
  styles: [
    `
      :host {
        display: flex;
        justify-content: start;
        margin: 0.2rem;
        padding: 0 0.3rem;
        overflow-x: auto;
        white-space: nowrap;
        text-align: center;
        align-items: center;
      }
    `,
  ],
  host: {
    '[class]': 'class',
    '[ngStyle]': 'style',
  },
})
export class ActionBoxComponent {
  @Input() class?: string;
  @Input() style?: { [klass: string]: any } | null; // hỗ trợ object cho ngStyle
}
