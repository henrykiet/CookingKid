import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { IOption } from '../../models/dynamic.model';

@Component({
  selector: 'app-radio',
  imports: [CommonModule],
  template: `
    <div>
      <label [class]="classLabel">{{ label }}</label>
      <div *ngFor="let option of options" class="form-check">
        <input
          class="form-check-input"
          type="radio"
          [name]="nameRadio"
          [value]="option"
          [checked]="value === option"
          [id]="for"
          (change)="onChange(option)"
          (blur)="onTouched()"
        />
        <label class="form-check-label" [for]="for">{{ option }}</label>
      </div>
    </div>
  `,
  styles: [``],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RadioComponent),
      multi: true,
    },
  ],
})
export class RadioComponent {
  @Input() label = '';
  @Input() for? = '';
  @Input() classLabel? = '';
  @Input() nameRadio = 'radio';
  @Input() options?: string[] = [];

  value: any;
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
