import { Component, inject, Input } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DynamicService } from '../../services/dynamic.service';
import {
  IForm,
  IMetadataForm,
  IOption,
  IValidator,
} from '../../models/dynamic.model';
import { CommonModule } from '@angular/common';
import { ActionBoxComponent } from '../../templates/boxs/action-box/action-box.component';
import { ButtonComponent } from '../../templates/button/button.component';
import { InputComponent } from '../../templates/input/input.component';
import { SelectComponent } from '../../templates/select/select.component';
import { RadioComponent } from '../../templates/radio/radio.component';

@Component({
  selector: 'app-dynamic-popup',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ActionBoxComponent,
    ButtonComponent,
    InputComponent,
    SelectComponent,
    RadioComponent,
  ],
  templateUrl: './dynamic-popup.component.html',
  styleUrl: './dynamic-popup.component.scss',
})
export class DynamicPopupComponent {
  @Input() form: IForm | null = null;
  metadataForm: IMetadataForm | null = null;
  loading: boolean = true;
  fb = inject(FormBuilder);
  dynamicFormGroup: FormGroup = this.fb.group({}, { updateOn: 'submit' });
  isMaster: boolean = false;
  activeDetailIndex: number = 0;
  setActiveDetail(index: number) {
    this.activeDetailIndex = index;
  }
  constructor(private dynamicService: DynamicService) {}
  private isBrowser(): boolean {
    return typeof window !== 'undefined';
  }

  ngOnInit(): void {
    if (localStorage.getItem('metadataConfig')) {
      this.metadataForm = JSON.parse(localStorage.getItem('metadataConfig')!);
      this.form = this.metadataForm?.form || null;
      if (this.form) {
        this.validations();
        this.loading = false;
      } else {
        console.error('No Form config found in localStorage');
      }
    } else {
      console.error('No Metadata Config found in localStorage');
    }
  }

  // getPopupForm(): void {
  //   this.loading = true;
  //   this.dynamicService.handleMetadataForm(this.metadataForm).subscribe({
  //     next: (res) => {
  //       if (res && res.form) {
  //         this.form = res.form;
  //         localStorage.setItem('metadataConfig', JSON.stringify(this.form));
  //         this.validations();
  //       } else {
  //         console.error('No form configuration found');
  //       }
  //       this.loading = false;
  //     },
  //     error: (err) => {
  //       console.error('Error loading form', err);
  //       this.loading = false;
  //     },
  //   });
  // }

  validations() {
    if (this.form?.fieldControls && this.form.fieldControls.length > 0) {
      let formGroup: any = {};

      //master
      if (this.form?.fieldControls?.length) {
        const initialRecord = this.form.initialDatas[0];
        this.form.fieldControls.forEach((control) => {
          let value = initialRecord?.[control.name];
          let fieldName = control.name;
          if (control.type == 'date' && value) {
            const dateString = String(value).split(' ')[0]; // Lấy phần ngày: "08/05/1991"
            const parts = dateString.split('/');

            if (parts.length === 3) {
              // Giả sử: parts[0] là Ngày (08), parts[1] là Tháng (05), parts[2] là Năm (1991)
              // Chuyển sang định dạng chuẩn YYYY-MM-DD
              const day = parts[0].padStart(2, '0'); // 08
              const month = parts[1].padStart(2, '0'); // 05
              const year = parts[2]; // 1991

              value = `${year}-${month}-${day}`; // Kết quả: "1991-05-08"
            }
          } else if (control.type == 'radio' && value) {
            control.radioOptions?.forEach((radio) => {
              const firstCharName = String(radio).charAt(0).toLowerCase();
              const lowerValue = value.toLowerCase();
              if (lowerValue == firstCharName) {
                value = radio;
              }
            });
          } else if (control.type == 'select') {
            if (control.options && typeof value === 'string') {
              // 2. Ép kiểu rõ ràng biến lặp `select` thành IOption
              // Sử dụng for...of hoặc forEach với khai báo kiểu rõ ràng
              for (const select of control.options as IOption[]) {
                // 3. Đảm bảo select.id không phải undefined trước khi gọi String()
                const idValue =
                  select.id !== undefined ? String(select.id) : '';
                const selectLower = idValue.toLowerCase();

                const lowerValue = value.toLowerCase();

                console.log('select', select.id, select.value);

                // 4. Nếu tìm thấy, gán giá trị và THOÁT KHỎI VÒNG LẶP
                if (selectLower == lowerValue) {
                  value = select.value;
                  // Nếu đây là hàm xử lý gán giá trị, bạn nên dùng return hoặc flag để dừng lại
                  // break; // Bạn không thể dùng break trong forEach, nên dùng for...of hoặc .find()
                }
              }
            }
          }
          formGroup[fieldName] = [
            value || '',
            this.mapValidators(control.validators),
          ];
        });
      }

      //detail
      if (this.form?.detailForms?.length) {
        this.form.detailForms.forEach((detailForm) => {
          const formArray = this.fb.array<FormGroup>([]);

          detailForm.initialDatas?.forEach((item: { [key: string]: any }) => {
            const group: any = {};
            detailForm.fieldControls?.forEach((field) => {
              group[field.name] = [
                item[field.name] ?? '', // chắc chắn sẽ có giá trị mặc định
                this.mapValidators(field.validators),
              ];
            });

            formArray.push(this.fb.group(group));
          });
          formGroup[detailForm.tableName!] = formArray;
        });
      }
      this.dynamicFormGroup = this.fb.group(formGroup);
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
    group?: AbstractControl
  ): string {
    const control = group
      ? (group as FormGroup).get(fieldName)
      : this.dynamicFormGroup.get(fieldName);

    if (!control) return '';

    let errorMessage = '';
    validators.forEach((validator) => {
      if (control.hasError(validator.validatorName as string)) {
        errorMessage = validator.message as string;
      }
    });
    return errorMessage;
  }

  getInitialData(tableName: string | undefined): FormArray {
    if (!tableName) {
      return this.fb.array([]);
    }
    return this.dynamicFormGroup.get(tableName) as FormArray;
  }

  handelMasterForm(is: boolean): void {
    this.isMaster = is;
    console.log('isMaster:', this.isMaster);
  }

  invokeAction(methodName: string | undefined, ...args: any[]) {
    if (methodName && typeof (this as any)[methodName] === 'function') {
      (this as any)[methodName](...args);
      console.log(`Invoked method: ${methodName} with args:`, args);
    } else {
      console.error(`Method ${methodName} not found`);
    }
  }

  onSubmit() {
    this.validations();
  }

  onReset(detailForm: any | null, rowIndex: number | null) {
    console.log('Resetting form...');
    console.log('Detail Form:', detailForm);
    console.log('Row Index:', rowIndex);
    console.log('dynamicFormGroup:', this.dynamicFormGroup);
    //reset master
    if (this.isMaster && this.form) {
      console.log('Resetting master form...');
      this.form.fieldControls.forEach((field) => {
        const ctrl = this.dynamicFormGroup.get(field.name);
        // chỉ reset field của master, không reset FormArray (detail)
        if (ctrl && !(ctrl instanceof FormArray)) {
          ctrl.setValue(field.value);
        }
      });
      return;
    } else if (!this.isMaster && detailForm != null && rowIndex != null) {
      // Reset 1 row trong detail
      if (detailForm && rowIndex !== null) {
        const formArray = this.getInitialData(detailForm.tableName);
        const row = formArray.at(rowIndex) as FormGroup;

        const initialRowData = detailForm.initialDatas?.[rowIndex];
        if (initialRowData) {
          detailForm.fieldControls.forEach((field: any) => {
            row.get(field.name)?.setValue(initialRowData[field.name] ?? '');
          });
        }
      }
    }
  }
  // Remove detail row
  removeDetailRow(detailForm: IForm, rowIndex: number): void {
    if (!detailForm.tableName) return;

    const formArray = this.dynamicFormGroup.get(
      detailForm.tableName
    ) as FormArray;
    if (formArray && formArray.length > rowIndex) {
      formArray.removeAt(rowIndex); // Xóa dòng trong FormArray
    }

    // Nếu bạn muốn đồng bộ lại luôn cả initialDatas trong metadata
    if (detailForm.initialDatas) {
      detailForm.initialDatas.splice(rowIndex, 1);
    }
  }
}
