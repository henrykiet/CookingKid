import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { IFieldControl } from '../../models/dynamic.model';
import { PaginationComponent } from '../pagination/pagination.component';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [CommonModule, PaginationComponent, ButtonComponent],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss',
})
export class TableComponent {
  @Input() thead: IFieldControl[] | undefined = [];
  @Input() tbody: any[] = [];
  @Output() row = new EventEmitter<any>();
  currentPage: number = 1;
  totalPages: number = 10;
  @ViewChild('table') table!: ElementRef<HTMLTableElement>;

  // ngAfterViewInit() {
  //   this.syncColumnWidths();
  //   window.addEventListener('resize', () => this.syncColumnWidths());
  // }

  // ngOnChanges(changes: SimpleChanges) {
  //   if (changes['tbody']) {
  //     setTimeout(() => this.syncColumnWidths(), 0); // Đảm bảo DOM update xong
  //   }
  // }

  // private syncColumnWidths() {
  //   if (!this.table) return;

  //   const tableEl = this.table.nativeElement;
  //   const headerCells = tableEl.querySelectorAll('thead th');
  //   const bodyRows = tableEl.querySelectorAll('tbody tr');

  //   if (!bodyRows.length) return;

  //   bodyRows.forEach((row) => {
  //     const cells = row.querySelectorAll('td');
  //     cells.forEach((cell, i) => {
  //       const thWidth = (headerCells[i] as HTMLElement).getBoundingClientRect()
  //         .width;
  //       (cell as HTMLElement).style.width = thWidth + 'px';
  //     });
  //   });
  // }

  //update
  onchangeUpdate(row: any) {
    this.row.emit(row);
  }

  //delete
  onchangeDelete(row: any) {
    this.row.emit(row);
  }

  //phân trang
  onPageChange($event: number) {
    throw new Error('Method not implemented.');
  }
}
