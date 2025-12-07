import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DataTransferService {
  private data: any = {};

  setData(key: string, value: any): void {
    this.data[key] = value;
  }

  getData(key: string): any {
    const value = this.data[key];
    // Xóa dữ liệu sau khi lấy để tránh sử dụng lại (tùy chọn)
    delete this.data[key];
    return value;
  }
}
