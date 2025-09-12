import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { IForm, IMetadataForm } from '../models/dynamic.model';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { lauching } from '../../../lauching.conf';
import { APIResponse } from '../models/response-api.model';

@Injectable({
  providedIn: 'root',
})
export class DynamicService {
  formConfig: IForm | null = null;
  metadataConfig: IMetadataForm | null = null;
  private apiUrl = `${lauching.apiUrl}/api/data`;
  constructor(private http: HttpClient) {}

  getMetadataForm(): Observable<IMetadataForm | null> {
    return this.http
      .get<APIResponse<IMetadataForm>>(`${this.apiUrl}/form`)
      .pipe(
        map((response) => {
          if (response.success) {
            localStorage.setItem(
              'metadataConfig',
              JSON.stringify(response.data)
            );
            localStorage.setItem(
              'formConfig',
              JSON.stringify(response.data.form)
            );
            this.metadataConfig = response.data;
            this.formConfig = response.data.form ?? null;
            return response.data;
          } else {
            return null;
          }
        })
      );
  }

  handleMetadataForm(
    metadataForm: IMetadataForm | null
  ): Observable<IMetadataForm | null> {
    return this.http
      .post<APIResponse<IMetadataForm>>(`${this.apiUrl}/form`, metadataForm)
      .pipe(
        map((response) => {
          if (response.success) {
            localStorage.setItem(
              'metadataConfig',
              JSON.stringify(response.data)
            );
            localStorage.setItem(
              'formConfig',
              JSON.stringify(response.data.form)
            );
            this.metadataConfig = response.data;
            this.formConfig = response.data.form ?? null;
            return response.data;
          } else {
            return null;
          }
        })
      );
  }
  //#region old
  // getDynamicForm(config: IForm): IForm | null {
  //   if (isPlatformBrowser(this.platformId)) {
  //     this.formConfig = config;
  //     localStorage.setItem('formConfig', JSON.stringify(config));
  //   } else {
  //     // chạy trên server, không dùng localStorage
  //     this.formConfig = config;
  //   }
  //   return this.formConfig;
  // }
  //#endregion
}
