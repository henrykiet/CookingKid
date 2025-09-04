import { Component, Input, OnInit } from '@angular/core';
import { NavbarComponent } from './app-homes/navbar/navbar.component';
import { SidebarComponent } from './app-homes/sidebar/sidebar.component';
import { SidebarToggle } from './app-homes/sidebar/side-data';
import { NavigationStart, Route, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  isSidebarCollapsed = false;
  screenWidth = 0;

  constructor(private router: Router) {}
  ngOnInit() {
    // this.router.events.subscribe((event) => {
    //   if (event instanceof NavigationStart) {
    //     localStorage.removeItem('formConfig');
    //     localStorage.removeItem('metadataConfig');
    //   }
    // });
  }

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
}
