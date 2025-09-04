export interface IMetadataForm {
  controller?: string;
  action?: string;
  vCId?: string;
  vCDate?: string;
  form?: IForm;
}

export interface IForm {
  titleForm?: string;
  typeForm?: string;
  classForm?: string;
  style?: string;
  tableName?: string;
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
interface IOption {
  id?: number | string;
  value?: string;
}
