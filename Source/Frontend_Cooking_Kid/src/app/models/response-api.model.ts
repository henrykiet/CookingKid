export interface APIResponse<T = any> {
  successCode: number;
  success: boolean;
  data: T;
  message: string;
}
