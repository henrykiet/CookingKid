import { switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Component, inject, Input, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DynamicService } from '../../services/dynamic.service';
import { IMetadataForm } from '../../models/dynamic.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonComponent } from '../../templates/button/button.component';
import { ActionBoxComponent } from '../../templates/boxs/action-box/action-box.component';
import { TableComponent } from '../../templates/table/table.component';
import { ViewHelperService } from '../../helpers/view-helper';
import { DataTransferService } from '../../services/data-transfer.service';

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
})
export class DynamicGridComponent implements OnInit {
  @Input() row: any = null;
  action: string = 'list';
  metaform: IMetadataForm | null = null;
  currentController: string | null = null;
  route = inject(ActivatedRoute);
  loading: boolean = false;
  isPopup: boolean = false;
  constructor(private dynamicService: DynamicService, private router: Router) {}
  ngOnInit(): void {
    // Lắng nghe thay đổi tham số route để lấy controllerName -> grid
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          const controllerName = params.get('controllerName');
          if (controllerName) {
            this.currentController = controllerName;
            this.metaform = {
              controller: controllerName,
              action: this.action,
            };
            this.getGridForm();
          } else {
            this.metaform = null;
          }
          return [];
        })
      )
      .subscribe();
  }

  getGridForm(): void {
    this.loading = true;
    this.dynamicService.getMetadataForm(this.metaform).subscribe({
      next: (res) => {
        if (res) {
          this.metaform = res;
          // if (this.metaform.form != null) {
          //   this.viewHelper.FilterFieldControl(this.metaform.form, this.action);
          // }
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
  handlePopupForm(row: any, action: string): void {
    if (this.metaform != null && this.metaform.form != null) {
      // this.metaform!.action = action;
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
        // SỬ DỤNG KEY  ĐỂ TRA CỨU TRONG normalizedRow
        const normalizedKey = key.toLowerCase();
        pkValue[key] =
          normalizedRow[normalizedKey] !== undefined &&
          normalizedRow[normalizedKey] !== null
            ? String(normalizedRow[normalizedKey])
            : null;
      });
      // this.metaform.pkValue = pkValue;
      const metadataConfig: IMetadataForm = {
        controller: this.metaform.controller,
        action: action,
        partition: this.metaform.partition,
        isPartition: this.metaform.isPartition,
        pkValue: pkValue,
      };
      localStorage.setItem('metadataConfig', JSON.stringify(metadataConfig));
      //chuyển hướng đến popup
      // this.router.navigate(['popup'], { relativeTo: this.route });
      this.router.navigate(['/popup', this.currentController]);
    } else {
      console.error('Metadata form or form configuration is null');
    }
  }
}
