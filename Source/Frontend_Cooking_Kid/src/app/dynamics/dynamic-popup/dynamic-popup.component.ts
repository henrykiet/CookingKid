import { Component, inject, Input, Output } from '@angular/core';
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
import { TableComponent } from '../../templates/table/table.component';
import { ViewHelperService } from '../../helpers/view-helper';
import { Router } from '@angular/router';
// import { DateComponent } from '../../templates/date/date.component';

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
    TableComponent,
    // DateComponent,
  ],
  templateUrl: './dynamic-popup.component.html',
})
export class DynamicPopupComponent {
  formArray: FormArray | null = null;
  action: string = 'update';
  @Input() form: IForm | null = null;
  metadataForm: IMetadataForm | null = null;
  loading: boolean = true;
  fb = inject(FormBuilder);
  dynamicFormGroup: FormGroup = this.fb.group({}, { updateOn: 'submit' });
  isMaster: boolean = false;
  //khai báo nút chuyển tab detail
  activeDetailIndex: number = 0;
  setActiveDetail(index: number) {
    this.activeDetailIndex = index;
  }

  constructor(
    private dynamicService: DynamicService,
    private viewHelper: ViewHelperService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const metadataString = localStorage.getItem('metadataConfig');
    if (metadataString) {
      // Kiểm tra cả 2 điều kiện
      this.metadataForm = JSON.parse(metadataString);
      if (this.metadataForm) {
        this.getPopupForm(this.metadataForm);
        console.log(this.metadataForm.action);
      }
    } else {
      console.error(
        'Missing metadataConfig or pkValue in localStorage. Cannot load popup form.'
      );
      // Có thể thêm logic điều hướng về trang Grid nếu thiếu dữ liệu
      // this.router.navigate(['../'], { relativeTo: this.route });
    }
  }

  getPopupForm(metadataForm: IMetadataForm): void {
    this.loading = true;
    this.dynamicService.getMetadataForm(metadataForm).subscribe({
      next: (res) => {
        if (res && res.form) {
          this.form = res.form;
          // if (this.form)
          //   this.viewHelper.FilterFieldControl(this.form, this.action);
          this.validations();
        } else {
          console.error('No form configuration found');
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading form', err);
        this.loading = false;
      },
    });
  }

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
            const dateString = String(value).split('T')[0]; // Lấy phần ngày: "08/05/1991"
            const parts = dateString.split('-');

            if (parts.length === 3) {
              // Giả sử: parts[0] là Ngày (08), parts[1] là Tháng (05), parts[2] là Năm (1991)
              // Chuyển sang định dạng chuẩn YYYY-MM-DD
              const day = parts[2].padStart(2, '0'); // 08
              const month = parts[1].padStart(2, '0'); // 05
              const year = parts[0]; // 1991
              value = `${year}-${month}-${day}`;
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
            if (control.options && value) {
              let found = false;
              for (const select of control.options as IOption[]) {
                const idValue =
                  select.id !== undefined ? String(select.id) : '';
                const selectLower = idValue.toLowerCase();
                const lowerValue = value.toLowerCase();
                // Nếu giá trị data (A) khớp với ID của option (A)
                if (selectLower == lowerValue) {
                  value = select.id;
                  found = true;
                  break;
                }
              }
              // Nếu không tìm thấy, giá trị nên là null/empty string để kích hoạt required
              if (!found) {
                value = '';
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
    let control: AbstractControl | null;

    // Trường hợp 1: Nếu group được truyền vào (detail table), tìm control trong group đó
    if (group) {
      control = (group as FormGroup).get(fieldName);
    }
    // Trường hợp 2: Master form, tìm control trong dynamicFormGroup chính
    else {
      control = this.dynamicFormGroup.get(fieldName);
    }
    if (!control || control.valid) return '';
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
    this.dynamicFormGroup.markAllAsTouched();
    if (this.dynamicFormGroup.invalid) {
      console.warn('Form is invalid. Cannot submit.');
      return;
    } else {
      if (!this.metadataForm) {
        console.warn('Metadata form is not exist');
        return;
      }
      const requestMetadata: IMetadataForm = {
        ...this.metadataForm,
        initialDatas: this.dynamicFormGroup.value,
      };
      console.log('Dynamic when update:', this.dynamicFormGroup.value);
      this.dynamicService.updateMetadataForm(requestMetadata).subscribe({
        next: (res) => {
          if (res?.success) {
            console.log('success');
            localStorage.removeItem('metadataConfig');
            const controllerName = this.metadataForm?.controller;
            if (controllerName) {
              this.router.navigate(['/grid', controllerName]);
            } else {
              console.error(
                'Controller name is missing, cannot navigate back to grid.'
              );
              // Có thể điều hướng về trang chủ hoặc trang mặc định
              // this.router.navigate(['/']);
            }
          } else {
            console.log('unsuccess');
          }
        },
      });
    }
  }

  onReset(detailForm: any | null, rowIndex: number | null) {
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

  // HÀM GETTER ĐỂ LẤY FORM ARRAY TỪ FORM GROUP
  getDetailFormArray(tableName: string | undefined): FormArray | null {
    if (!tableName) return null;
    // Tránh lỗi kiểu dữ liệu và đảm bảo nó là FormArray
    const control = this.dynamicFormGroup.get(tableName);
    if (control instanceof FormArray) {
      this.formArray = control;
      return control;
    }
    return null;
  }
}
