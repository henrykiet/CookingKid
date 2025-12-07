// // src/app/shared/format/ngb-date-adapter.ts
// import { Injectable } from '@angular/core';
// import { NgbDateAdapter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

// @Injectable()
// export class CustomNgbDateAdapter extends NgbDateAdapter<string> {
//   fromModel(value: string | null): NgbDateStruct | null {
//     if (!value) return null;
//     // Input từ Form Control là YYYY-MM-DD
//     const parts = value.split('-');
//     if (parts.length !== 3) return null;
//     return {
//       year: parseInt(parts[0], 10),
//       month: parseInt(parts[1], 10),
//       day: parseInt(parts[2], 10),
//     };
//   }

//   toModel(date: NgbDateStruct | null): string | null {
//     if (!date) return null;
//     // Xuất ra YYYY-MM-DD cho Form Control
//     const month = date.month.toString().padStart(2, '0');
//     const day = date.day.toString().padStart(2, '0');
//     return `${date.year}-${month}-${day}`;
//   }
// }
