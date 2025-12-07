// import { Injectable } from '@angular/core';
// import {
//   NgbDateParserFormatter,
//   NgbDateStruct,
// } from '@ng-bootstrap/ng-bootstrap';
// @Injectable()
// export class CustomNgbDateFormat extends NgbDateParserFormatter {
//   readonly DELIMITER = '/';
//   parse(value: string): NgbDateStruct | null {
//     if (value) {
//       const parts = value.split(this.DELIMITER);
//       if (parts.length === 3) {
//         // Giả định nhập DD/MM/YYYY
//         return {
//           day: parseInt(parts[0], 10),
//           month: parseInt(parts[1], 10),
//           year: parseInt(parts[2], 10),
//         };
//       }
//     }
//     return null;
//   }

//   format(date: NgbDateStruct | null): string {
//     if (!date) {
//       return '';
//     }
//     const day = date.day.toString().padStart(2, '0');
//     const month = date.month.toString().padStart(2, '0');
//     const year = date.year;
//     return `${day}${this.DELIMITER}${month}${this.DELIMITER}${year}`; // Định dạng hiển thị
//   }
// }
