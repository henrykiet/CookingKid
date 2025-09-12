import { Component } from '@angular/core';
import { IMetadataForm } from '../../models/dynamic.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DynamicGridComponent } from '../../dynamics/dynamic-grid/dynamic-grid.component';

@Component({
  selector: 'app-grid-master',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, DynamicGridComponent],
  template: `<app-dynamic-grid [metaform]="metadataForm"></app-dynamic-grid>`,
})
export class UserGridComponent {
  metadataForm: IMetadataForm = {
    controller: 'user',
    action: 'list',
  };
}
