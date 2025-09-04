// import { IForm } from '../../models/dynamic.model';

// export const popupFormConfig: IForm = {
//   titleForm: 'Dynamic Form 1',
//   buttonControls: [
//     {
//       name: 'submit',
//       label: 'Submit',
//       type: 'submit',
//       class: 'btn btn-primary',
//     },
//     {
//       name: 'reset',
//       label: 'Reset',
//       type: 'reset',
//       class: 'btn btn-secondary',
//     },
//   ],
//   fieldControls: [
//     {
//       name: 'firstName',
//       label: 'First Name',
//       value: '',
//       placeHolder: 'Input First Name',
//       class: 'col-md-6',
//       type: 'text',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*First Name is required',
//         },
//       ],
//     },
//     {
//       name: 'lastName',
//       label: 'Last Name',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-6',
//       type: 'text',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Last Name is required',
//         },
//         {
//           validatorName: 'minlength',
//           minlength: 5,
//           message: '*Minimum length should be 5',
//         },
//       ],
//     },
//     {
//       name: 'email',
//       label: 'email',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-4',
//       type: 'email',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Email is required',
//         },
//         {
//           validatorName: 'email',
//           email: 'email',
//           message: '*Email is not valid',
//         },
//       ],
//     },
//     {
//       name: 'phone',
//       label: 'phone',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-4',
//       type: 'number',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Phone is required',
//         },
//         {
//           validatorName: 'maxlength',
//           maxlength: 10,
//           message: '*Maximum length should be 10',
//         },
//       ],
//     },
//     {
//       name: 'gender',
//       label: 'gender',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-4',
//       radioOptions: ['Male', 'Female'],
//       type: 'radio',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Gender is required',
//         },
//       ],
//     },
//     {
//       name: 'option',
//       label: 'package',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-4',
//       options: [
//         {
//           id: '1',
//           value: 'Option 1',
//         },
//         {
//           id: '2',
//           value: 'Option 2',
//         },
//         {
//           id: '2',
//           value: 'Option 3',
//         },
//       ],
//       type: 'select',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Option is required',
//         },
//       ],
//     },
//     {
//       name: 'birthDate',
//       label: 'Birth of Date',
//       value: '',
//       placeHolder: '',
//       class: 'col-md-4',
//       type: 'date',
//       validators: [
//         {
//           validatorName: 'required',
//           required: true,
//           message: '*Phone is required',
//         },
//       ],
//     },
//   ],
// };
