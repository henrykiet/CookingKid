import { IForm } from './../models/dynamic.model';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class ViewHelperService {
  //hàm lấy fieldControls dựa theo action
  public FilterFieldControl(form: IForm, action: string): void {
    if (form) {
      const fields = form.fieldControls.filter((field) => {
        const isViewArray: string[] = field.isView ?? [];
        return isViewArray.includes(action) || isViewArray.length === 0;
      });
      form.fieldControls = fields;
    }
    if (form.detailForms)
      form.detailForms.forEach((df) => {
        this.FilterFieldControl(df, action);
      });
  }
}
