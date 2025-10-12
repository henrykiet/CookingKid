import { Component, Input, OnInit } from '@angular/core';
import { NavbarComponent } from './app-homes/navbar/navbar.component';
import { SidebarComponent } from './app-homes/sidebar/sidebar.component';
import { SidebarToggle } from './models/side-data';
import { NavigationStart, Route, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DynamicService } from './services/dynamic.service';
import { IMetadataForm } from './models/dynamic.model';
import { DynamicGridComponent } from './dynamics/dynamic-grid/dynamic-grid.component';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    NavbarComponent,
    SidebarComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  isSidebarCollapsed = false;
  screenWidth = 0;
  @Input({ required: true }) metaform: IMetadataForm | null = null;
  @Input() controllerName: string = '';
  constructor(private router: Router, private dynamicServer: DynamicService) {}
  ngOnInit() {}
  // hàm nhận sự kiện từ sidebar
  onControllerChange(routeLink: string) {
    console.log('Selected menu routeLink:', routeLink);
    this.controllerName = routeLink; // update để truyền xuống DynamicGridComponent
    console.log('controllerName in AppComponent:', this.controllerName);
  }

  //#region Sidebar
  onToggleSidebar(data: SidebarToggle): void {
    this.screenWidth = data.screenWidth;
    this.isSidebarCollapsed = data.collapsed;
  }

  getBodyClass(): string {
    let styleClass = '';
    if (this.isSidebarCollapsed && this.screenWidth > 768) {
      styleClass = 'body-trimmed';
    } else if (
      this.isSidebarCollapsed &&
      this.screenWidth <= 768 &&
      this.screenWidth > 0
    ) {
      styleClass = 'body-md-screen';
    }
    return styleClass;
  }
  //#endregion
}
