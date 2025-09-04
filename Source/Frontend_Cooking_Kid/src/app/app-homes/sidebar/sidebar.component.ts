import {
  Component,
  EventEmitter,
  HostListener,
  OnInit,
  Output,
} from '@angular/core';
import { menuData, sidebarData, SidebarToggle } from './side-data';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent implements OnInit {
  @Output() onToggleSidebar: EventEmitter<SidebarToggle> = new EventEmitter();
  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    if (typeof window !== 'undefined') {
      this.screenWidth = window.innerWidth;
      if (this.screenWidth <= 768) {
        this.collapsed = false;
        this.onToggleSidebar.emit({
          collapsed: this.collapsed,
          screenWidth: this.screenWidth,
        });
      }
    }
  }

  collapsed = false;
  screenWidth = 0;
  sideData: menuData[] = [];
  previousExpandedItems: menuData[] = [];
  constructor(private router: Router) {}

  ngOnInit(): void {
    if (typeof window !== 'undefined') {
      this.screenWidth = window.innerWidth;
    }

    //gọi hàm load menu cho sau này sử dụng
    this.sideData = this.loadMenu(sidebarData);
  }

  //hàm gọi api menu
  loadMenu(item: any[]): menuData[] {
    return item.map((i) => ({
      label: i.label,
      routeLink: i.routeLink,
      icon: i.icon,
      expanded: i.expanded ?? false,
      children: i.children ? this.loadMenu(i.children) : undefined,
    }));
  }

  //handle sidebar
  toggleCollapse(): void {
    this.collapsed = !this.collapsed;

    //khi thu gọn
    if (!this.collapsed) {
      //lưu lại các item đã mở trước đó
      this.previousExpandedItems = this.sideData.filter(
        (item) => item.expanded
      );
      //đóng hết submenu
      this.closeAllSubMenus();
    } else {
      // Khi mở rộng => mở lại previous menu nếu có
      this.previousExpandedItems.forEach((prevItem) => {
        const match = this.sideData.find(
          (item) => item.label === prevItem.label
        );
        if (match) {
          match.expanded = true;
        }
      });
    }

    this.onToggleSidebar.emit({
      collapsed: this.collapsed,
      screenWidth: this.screenWidth,
    });
  }

  //nút chọn item
  onItemClick(item: menuData, event: MouseEvent) {
    if (item.children) {
      event.preventDefault();

      item.expanded = !item.expanded;
    }

    if (this.collapsed == false) item.expanded = false;
    this.onToggleSidebar.emit({
      collapsed: this.collapsed,
      screenWidth: this.screenWidth,
    });
  }

  closeAllSubMenus(): void {
    this.sideData.forEach((item) => {
      if (item.children) {
        item.expanded = false;
      }
    });
  }
}
