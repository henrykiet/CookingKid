import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { IOption } from '../../models/dynamic.model';

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="select-wrapper">
      <label [class]="classLabel">{{ label }}</label>

      <!-- Box hiển thị giá trị -->
      <div class="select-box" (click)="toggleDropdown()">
        <span>{{ selectedLabel || placeHolder }}</span>
        <span class="material-icons icon">expand_more</span>
      </div>

      <!-- Dropdown list -->
      <ul *ngIf="isOpen" class="dropdown">
        <li
          *ngFor="let option of options"
          class="dropdown-item"
          (click)="selectOption(option)"
        >
          {{ option.value }}
        </li>
      </ul>
    </div>
  `,
  styles: [
    `
      .select-wrapper {
      }
      .select-box {
        border: 1px solid #ccc;
        border-radius: 1rem;
        padding: 8px 12px;
        background: rgba(255, 255, 255, 0.5);
        cursor: pointer;

        display: flex;
        justify-content: space-between;
        align-items: center;

        &:hover {
          border-color: #3b82f6;
        }
      }

      .icon {
        font-size: 18px;
        color: #555;
      }

      .dropdown {
        position: relative;
        left: 0;
        right: 0;
        background: rgba(255, 255, 255, 0.5);
        border: 1px solid #ddd;
        border-radius: 0.8rem;
        margin-top: 4px;
        max-height: 240px;
        overflow-y: auto;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        list-style: none;
        padding: 0;
      }

      .dropdown-item {
        padding: 8px 12px;
        cursor: pointer;
        border-radius: 0.8rem;
        display: block;
        white-space: nowrap; /* không xuống dòng */
        overflow: hidden; /* ẩn phần thừa */
        text-overflow: ellipsis; /* hiện ... */
        max-width: 100%; /* giới hạn theo khung */
        align-items: center;
        text-align: center;
        &:hover {
          background-color: #e0f2fe;
        }
      }
    `,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    },
  ],
})
export class SelectComponent {
  @Input() label? = '';
  @Input() classLabel? = '';
  @Input() classSelect? = 'form-select';
  @Input() placeHolder? = 'Choose...';
  @Input() options?: IOption[] = [];

  isOpen = false;
  value: any;
  selectedLabel?: string;

  onChange = (_: any) => {};
  onTouched = () => {};

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  selectOption(option: IOption) {
    this.value = option.id;
    this.selectedLabel = option.value;
    this.onChange(this.value);
    this.onTouched();
    this.isOpen = false;
  }

  writeValue(val: any): void {
    this.value = val;
    const found = this.options!.find((o) => o.id === val);
    this.selectedLabel = found?.value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
