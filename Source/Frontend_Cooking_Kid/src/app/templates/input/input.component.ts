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
      [disabled]="isDisabled"
      [readonly]="isReadOnly"
      [hidden]="isHidden"
      [style]="style"
    />
  `,
  styles: [
    `
      input {
        background: rgba(255, 255, 255, 0.5);
        border-radius: 1rem;
        padding: 0.5rem;
        width: 100%;
      }
      input:hover {
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.25);
      }
      input:focus {
        outline: none; /* Tắt viền mặc định của trình duyệt */
        box-shadow: 0 0 10px rgba(59, 92, 60, 0.5);
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
  @Input() style: string | undefined = '';
  @Input() classLabel?: string = '';
  @Input() classInput?: string = '';
  @Input() placeHolder?: string = '';
  @Input() type?: string = 'text';
  @Input() isDisabled: boolean | undefined = false;
  @Input() isReadOnly: boolean | undefined = false;
  @Input() isHidden: boolean | undefined = false;
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
