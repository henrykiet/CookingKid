import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input, OnInit } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-radio',
  imports: [CommonModule],
  template: `
    <div>
      <label [class]="classLabel">{{ label }}</label>
      <div *ngFor="let option of options; let i = index" class="form-check">
        <input
          class="form-check-input"
          type="radio"
          [name]="nameRadio"
          [value]="getRadioKey(option)"
          [checked]="value === getRadioKey(option)"
          [id]="nameRadio + i"
          (change)="onChange(getRadioKey(option))"
          (blur)="onTouched()"
        />
        <label class="form-check-label" [for]="nameRadio + i">{{
          option
        }}</label>
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
  writeValue(val: string): void {
    this.value = val.charAt(0).toUpperCase();
  }
  onChange = (key: string) => {};
  getRadioKey(label: string): string {
    return label.charAt(0).toLocaleUpperCase();
  }

  onTouched = () => {};

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
