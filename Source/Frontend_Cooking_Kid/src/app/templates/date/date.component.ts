// import { CommonModule, DatePipe } from '@angular/common';
// import { Component, forwardRef, Input } from '@angular/core';
// import {
//   ControlValueAccessor,
//   NG_VALUE_ACCESSOR,
//   ReactiveFormsModule,
//   FormsModule,
// } from '@angular/forms';
// import {
//   NgbDateAdapter,
//   NgbDateParserFormatter,
//   NgbDatepickerModule,
//   NgbDateStruct,
//   NgbInputDatepicker,
// } from '@ng-bootstrap/ng-bootstrap';
// import { CustomNgbDateAdapter } from './ngb-date-adapter';
// import { CustomNgbDateFormat } from './ngb-date-format';

// const CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR: any = {
//   provide: NG_VALUE_ACCESSOR,
//   useExisting: forwardRef(() => DateComponent),
//   multi: true,
// };

// @Component({
//   selector: 'app-date',
//   standalone: true,
//   imports: [
//     CommonModule,
//     ReactiveFormsModule,
//     NgbInputDatepicker,
//     NgbDatepickerModule,
//     FormsModule,
//   ],
//   template: `<div class="input-group" [style]="style">
//     <input
//       class="form-control"
//       placeholder="dd/MM/yyyy"
//       #dp="ngbDatepicker"
//       ngbDatepicker
//       name="{{ name }}"
//       [id]="id"
//       [(ngModel)]="ngbDateModel"
//       (ngModelChange)="onNgbDateChange($event)"
//       (click)="dp.toggle()"
//       (blur)="onTouched()"
//     />
//     <button
//       class="btn btn-outline-secondary calendar"
//       (click)="dp.toggle()"
//       type="button"
//     >
//       📅
//     </button>
//   </div>`,
//   providers: [
//     //     CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR,
//     { provide: NgbDateAdapter, useClass: CustomNgbDateAdapter },
//     { provide: NgbDateParserFormatter, useClass: CustomNgbDateFormat },
//   ],
// })
// export class DateComponent implements ControlValueAccessor {
//   @Input() formControlName: string | null = null;
//   @Input() name: string | undefined;
//   @Input() id: string | undefined;
//   @Input() style: string | undefined;
//   ngbDateModel: NgbDateStruct | null = null;
//   onChange: (value: any) => void = () => {};
//   onTouched: () => void = () => {};

//   constructor(private adapter: NgbDateAdapter<string>) {}
//   // CVA implementation (Ng-Bootstrap sẽ tự động sử dụng chúng)
//   writeValue(value: string): void {
//     // Chuyển string YYYY-MM-DD thành NgbDateStruct để gán vào ngbDateModel
//     this.ngbDateModel = this.adapter.fromModel(value);
//   }
//   registerOnChange(fn: any): void {
//     this.onChange = fn;
//   }
//   registerOnTouched(fn: any): void {
//     this.onTouched = fn;
//   }
//   // Xử lý sự kiện thay đổi từ NgbDatepicker
//   onNgbDateChange(ngbDate: NgbDateStruct | null): void {
//     // Chuyển NgbDateStruct thành string YYYY-MM-DD để gửi ra Form Group
//     const valueToSendToForm = this.adapter.toModel(ngbDate);
//     this.onChange(valueToSendToForm);
//   }
// }
