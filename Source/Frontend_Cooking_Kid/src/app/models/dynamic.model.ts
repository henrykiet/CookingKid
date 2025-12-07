export interface IMetadataForm {
  controller?: string;
  action?: string;
  partition?: string;
  isPartition?: boolean;
  form?: IForm;
  pkValue?: { [key: string]: string | null };
  initialDatas?: any;
}

export interface IForm {
  titleForm?: string;
  typeForm?: string;
  classForm?: string;
  style?: string;
  tableName?: string;
  primaryKey?: string[];
  primarykeyValue?: { [key: string]: string | null };
  buttonControls: IButtonControl[];
  fieldControls: IFieldControl[];
  detailForms: IForm[];
  initialDatas: any;
}

export interface IButtonControl {
  name: string;
  label: string;
  type: string;
  class?: string;
  style?: string;
  click?: string;
  isReadOnly?: boolean;
  isDisabled?: boolean;
  isHidden?: boolean;
}

export interface IFieldControl {
  name: string;
  label: string;
  value: string;
  placeHolder: string;
  isView?: string[];
  class?: string;
  style?: string;
  type: string;
  radioOptions?: string[];
  options?: IOption[];
  validators: IValidator[];
  isReadOnly?: boolean;
  isDisabled?: boolean;
  isHidden?: boolean;
}
export interface IValidator {
  validatorName?: string;
  required?: boolean;
  minlength?: number;
  maxlength?: number;
  email?: string;
  pattern?: string;
  message?: string;
}
export interface IOption {
  id?: number | string;
  value?: string;
}
