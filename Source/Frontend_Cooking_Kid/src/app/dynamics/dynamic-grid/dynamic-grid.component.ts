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

@Component({
  selector: 'app-dynamic-grid',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ActionBoxComponent,
    ButtonComponent,
    PaginationComponent,
  ],
  templateUrl: './dynamic-grid.component.html',
  styleUrl: './dynamic-grid.component.scss',
})
export class DynamicGridComponent implements OnInit {
  currentPage: number = 1;
  @Input({ required: true }) metaform: IMetadataForm | null = null;
  loading: boolean = false;
  fb = inject(FormBuilder);
  dynamicFormGroup: FormGroup = this.fb.group({}, { updateOn: 'submit' });
  totalPages: number = 10;
  constructor(private dynamicService: DynamicService, private router: Router) {}
  ngOnInit(): void {
    this.getGridForm();
  }
  onPageChange($event: number) {
    throw new Error('Method not implemented.');
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
  handleUpdate(row: any): void {
    if (this.metaform != null && this.metaform.form != null) {
      this.metaform!.action = 'update';
      // Build giá trị PK từ danh sách primarykey
      const pkValue: { [key: string]: string | null } = {};
      this.metaform.form.primarykey?.forEach((key) => {
        pkValue[key] =
          row[key] !== undefined && row[key] !== null ? String(row[key]) : null;
      });
      // Gán vào form
      this.metaform.form.primarykeyvalue = pkValue;
      localStorage.setItem('metadataConfig', JSON.stringify(this.metaform));
      this.router.navigate([`${this.router.url}/popup`]);
    } else {
      //hiển thị không chuyển trang đucojw do thiếu dữ liệu
    }
  }
}
