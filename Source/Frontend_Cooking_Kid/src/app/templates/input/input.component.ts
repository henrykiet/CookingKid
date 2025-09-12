import { Component, forwardRef, Input } from '@angular/core';
import {
  FormsModule,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  template: `
    <label [class]="classLabel">{{ label }}</label>
    <input
      [value]="value"
      (input)="onChange($any($event.target).value)"
      (blur)="onTouched()"
      [type]="type"
      [placeholder]="placeHolder"
      [class]="classInput"
    />
  `,
  styles: [
    `
      input {
        background: rgba(255, 255, 255, 0.5);
        border-radius: 1rem;
        padding: 0.5rem;
      }
    `,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
})
export class InputComponent {
  @Input() formControlName: string = '';
  @Input() label?: string = '';
  @Input() classLabel?: string = '';
  @Input() classInput?: string = '';
  @Input() placeHolder?: string = '';
  @Input() type?: string = 'text';
  value?: any = '';
  onChange = (_: any) => {};
  onTouched = () => {};

  writeValue(val: any): void {
    this.value = val;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
