export interface ListApiResponse {
  controller: string;
  tableName: string;
  primaryKey: string[];
  language: string;
  unit: string;
  idVC: string;
  type: string;
  action: string;
  sort: string;
  userId: string;
  data: Record<string, string>[];
  total: number;
  page: number;
  pageSize: number;
}
