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

@Component({
  selector: 'app-dynamic-grid',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './dynamic-grid.component.html',
  styleUrl: './dynamic-grid.component.scss',
})
export class DynamicGridComponent implements OnInit {
  loading: boolean = false;
  metaform: IMetadataForm | null = null;
  fb = inject(FormBuilder);
  dynamicFormGroup: FormGroup = this.fb.group({}, { updateOn: 'submit' });
  constructor(private dynamicService: DynamicService) {}
  ngOnInit(): void {
    this.getGridForm();
  }
  getGridForm(): void {
    this.loading = true;
    this.dynamicService.getMetadataForm().subscribe({
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
}
