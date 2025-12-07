import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  EventEmitter,
  HostBinding,
  HostListener,
  inject,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { IFieldControl, IValidator } from '../../models/dynamic.model';
import { PaginationComponent } from '../pagination/pagination.component';
import { ButtonComponent } from '../button/button.component';
import { InputComponent } from '../input/input.component';
import {
  ControlContainer,
  FormGroupDirective,
  FormArray,
  ReactiveFormsModule,
  FormGroup,
  Validators,
  AbstractControl,
  FormControl,
} from '@angular/forms';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    CommonModule,
    PaginationComponent,
    ButtonComponent,
    InputComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss',
  viewProviders: [
    { provide: ControlContainer, useExisting: FormGroupDirective },
  ],
})
export class TableComponent implements OnInit, OnChanges {
  private parentFormGroupDirective = inject(FormGroupDirective, {
    optional: true,
  });
  @HostBinding('attr.formArrayName') get hostFormArrayName() {
    return this.isDetail && this.formArrayName ? this.formArrayName : null;
  }
  @Input() field: IFieldControl[] | undefined = [];
  @Input() data: any[] = [];
  @Output() row = new EventEmitter<any>();
  @Input() isDetail: boolean = false;
  @Input() formArray: FormArray | null = null;
  @Input() formArrayName: string | undefined = undefined;
  @Input() isFormSubmitted: boolean = false;
  @ViewChild('table') table!: ElementRef<HTMLTableElement>;

  //trạng thái kéo bảng
  // Lắng nghe sự kiện di chuột trên toàn bộ cửa sổ
  @HostListener('document:mousemove', ['$event'])
  onTableMouseMove(event: MouseEvent) {
    this.onMouseMove(event);
  }

  // Lắng nghe sự kiện thả chuột trên toàn bộ cửa sổ
  @HostListener('document:mouseup')
  onTableMouseUp() {
    this.onMouseUp();
  }
  private isResizing: boolean = false;
  private currentTh!: HTMLTableHeaderCellElement | null;
  private startX!: number;
  private startWidth!: number;

  currentPage: number = 1;
  totalPages: number = 10;

  parentFormGroup!: FormGroup;
  ngOnInit(): void {
    if (this.parentFormGroupDirective) {
      this.parentFormGroup = this.parentFormGroupDirective.form;
      this.data = this.formArray ? this.formArray.value : this.data;
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Kiểm tra nếu field đã được khởi tạo/thay đổi

    if (changes['field'] && this.field) {
      // Chỉ chạy validation khi field có giá trị

      this.validateDetail();

      // Nếu bạn dùng convertDateFields, cũng gọi ở đây

      this.convertDateFields();
    }
  }

  private convertDateFields() {
    if (!this.field || !this.data) return;
    const dateFields = this.field
      .filter((f) => f.type === 'date') // Giả định bạn có thuộc tính 'type: "date"' trong IFieldControl
      .map((f) => f.name);
    if (dateFields.length === 0) return;
    this.data.forEach((rowData, index) => {
      dateFields.forEach((fieldName) => {
        const dateValue = rowData[fieldName];
        if (dateValue && typeof dateValue === 'string') {
          const dateSplit = dateValue.split('T')[0];
          const part = dateSplit.split('-');
          let dateObject = '';
          if (part.length === 3) {
            dateObject = `${part[2]}-${part[1]}-${part[0]}`;
          }
          rowData[fieldName] = dateObject;
          // Cập nhật lại FormArray
          if (this.formArray && this.formArray.at(index)) {
            (this.formArray.at(index) as FormGroup)
              .get(fieldName)
              ?.setValue(dateObject, { emitEvent: false });
          }
        }
      });
    });
  }
  // 1. Khi người dùng nhấn chuột vào Resizer
  onMouseDown(event: MouseEvent, columnIndex: number) {
    // Nếu sự kiện không phải từ class resizer, bỏ qua (tùy chọn)
    if (!(event.target as HTMLElement).classList.contains('resizer')) {
      return;
    }

    this.isResizing = true;
    this.startX = event.clientX;

    // Lấy thẻ <th> hiện tại
    this.currentTh = (event.target as HTMLElement)
      .parentElement as HTMLTableHeaderCellElement;

    // Lấy độ rộng ban đầu của cột
    this.startWidth = this.currentTh.offsetWidth;

    // Ngăn chặn việc chọn văn bản trong khi kéo
    event.preventDefault();
  }

  // 2. Khi người dùng di chuyển chuột (và vẫn đang giữ nút chuột)
  onMouseMove(event: MouseEvent) {
    if (!this.isResizing || !this.currentTh) return;

    // Tính toán độ lệch
    const diffX = event.clientX - this.startX;

    // Tính toán độ rộng mới
    const minColWidth = 70; //giới hạn tối thiểu
    const newWidth = Math.max(minColWidth, this.startWidth + diffX);

    // Áp dụng độ rộng mới
    this.currentTh.style.width = `${newWidth}px`;
    this.currentTh.style.minWidth = `${newWidth}px`; // Quan trọng cho các cột dính
  }

  // 3. Khi người dùng thả nút chuột
  onMouseUp() {
    this.isResizing = false;
    this.currentTh = null;
  }

  validateDetail() {
    if (this.isDetail && this.field) {
      this.field.forEach((field) => {
        this.mapValidators(field.validators);
      });
    }
  }

  private mapValidators(validators: IValidator[]) {
    let controlValidators: any = [];
    if (validators) {
      validators.forEach((validator: IValidator) => {
        if (validator.validatorName === 'required')
          controlValidators.push(Validators.required);
        if (validator.validatorName === 'minlength')
          controlValidators.push(
            Validators.minLength(validator.minlength as number)
          );
        if (validator.validatorName === 'maxlength')
          controlValidators.push(
            Validators.maxLength(validator.maxlength as number)
          );
        if (validator.validatorName === 'email')
          controlValidators.push(Validators.email);
        if (validator.validatorName === 'pattern')
          controlValidators.push(
            Validators.pattern(validator.pattern as string)
          );
      });
    }
    return controlValidators;
  }
  getValidationErrors(
    fieldName: string,
    validators: IValidator[],
    group: AbstractControl // Bắt buộc phải là AbstractControl của hàng hiện tại
  ): string {
    const control = (group as FormGroup).get(fieldName);

    if (!control || control.valid) return '';

    let errorMessage = '';
    validators.forEach((validator) => {
      if (
        !errorMessage &&
        control!.hasError(validator.validatorName as string)
      ) {
        errorMessage = validator.message as string;
      }
    });
    return errorMessage;
  }
  //add
  onAdd() {
    if (this.isDetail && this.formArray) {
      // Logic thêm FormArray
      const newRowFormGroup = this.createEmptyRowFormGroup();
      this.formArray.push(newRowFormGroup);
      this.data.push(newRowFormGroup.value);
    } else {
      // Nếu không phải chi tiết, vẫn có thể thêm hàng trống vào data array
      this.data.push({});
      // Hoặc emit sự kiện cho component cha xử lý việc thêm hàng
      this.row.emit({ action: 'add', data: {} });
    }
  }
  private createEmptyRowFormGroup(): FormGroup {
    const rowFormGroup = new FormGroup({});

    // Lặp qua cấu hình các trường (cột) của bảng
    if (this.field) {
      this.field.forEach((fieldConfig) => {
        // Lấy các validator tương ứng (nếu có)
        // Cần hàm mapValidators() để chuyển IValidator[] thành ValidatorFn[]
        const validators = this.mapValidators
          ? this.mapValidators(fieldConfig.validators)
          : [];

        // Thêm FormControl với giá trị mặc định là chuỗi rỗng và các validator
        rowFormGroup.addControl(
          fieldConfig.name,
          new FormControl('', validators)
        );
      });
    }
    // Cần thêm FormControl cho cột "Action" nếu bạn sử dụng nó trong form (thường là không cần)
    return rowFormGroup;
  }
  //update
  onUpdate(row: any) {
    if (!this.isDetail) this.row.emit(row);
  }

  //delete
  onDelete(row: any) {
    if (this.isDetail && this.formArray) {
      this.formArray.removeAt(row);
      this.data.splice(row, 1);
      console.log(this.formArray.value);
      console.log(this.data);
    } else {
      //không phải detail thì emit data
      this.row.emit(row);
    }
  }

  //phân trang
  onPageChange($event: number) {
    throw new Error('Method not implemented.');
  }
}
