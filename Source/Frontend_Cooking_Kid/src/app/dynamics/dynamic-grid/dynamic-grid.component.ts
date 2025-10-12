import { map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DynamicService } from '../../services/dynamic.service';
import { IMetadataForm } from '../../models/dynamic.model';
import { Route, Router } from '@angular/router';
import { ButtonComponent } from '../../templates/button/button.component';
import { ActionBoxComponent } from '../../templates/boxs/action-box/action-box.component';
import { PaginationComponent } from '../../templates/pagination/pagination.component';
import { TableComponent } from '../../templates/table/table.component';

@Component({
  selector: 'app-dynamic-grid',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ActionBoxComponent,
    ButtonComponent,
    TableComponent,
  ],
  templateUrl: './dynamic-grid.component.html',
  styleUrl: './dynamic-grid.component.scss',
})
export class DynamicGridComponent implements OnInit {
  @Input({ required: true }) metaform: IMetadataForm | null = null;
  @Input() row: any = null;
  loading: boolean = false;
  fb = inject(FormBuilder);
  dynamicFormGroup: FormGroup = this.fb.group({}, { updateOn: 'submit' });
  constructor(private dynamicService: DynamicService, private router: Router) {}
  ngOnInit(): void {
    this.getGridForm();
  }

  getGridForm(): void {
    this.loading = true;
    this.dynamicService.handleMetadataForm(this.metaform).subscribe({
      next: (res) => {
        if (res) {
          this.metaform = res;
          localStorage.setItem('metadataConfig', JSON.stringify(this.metaform));
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

  handleDelete() {}
  handleUpdate(row: any, action: string): void {
    if (this.metaform != null && this.metaform.form != null) {
      this.metaform!.action = action;
      // Build giá trị PK từ danh sách primarykey
      const pkValue: { [key: string]: string | null } = {};
      const normalizedRow: { [key: string]: any } = {};
      for (const key in row) {
        if (Object.prototype.hasOwnProperty.call(row, key)) {
          normalizedRow[key.toLowerCase()] = row[key];
        }
      }
      // Khóa chính trong metadata là 'primaryKey'
      this.metaform.form.primaryKey?.forEach((key) => {
        // 2. SỬ DỤNG KEY (Vốn đã là chữ thường) ĐỂ TRA CỨU TRONG normalizedRow
        const normalizedKey = key.toLowerCase();

        pkValue[key] =
          normalizedRow[normalizedKey] !== undefined &&
          normalizedRow[normalizedKey] !== null
            ? String(normalizedRow[normalizedKey])
            : null;
      });
      this.metaform.pkValue = pkValue;
      // call api get update form
      this.dynamicService.handleMetadataForm(this.metaform).subscribe({
        next: (res) => {
          if (res && res.form) {
            // Gán vào form
            this.metaform!.form = res.form;
            console.log('metaform form for update:', this.metaform);
            localStorage.setItem(
              'metadataConfig',
              JSON.stringify(this.metaform)
            );
            this.router.navigate([`${this.router.url}/popup`]);
          }
        },
      });
    } else {
      //hiển thị không chuyển trang đucojw do thiếu dữ liệu
    }
  }
}
