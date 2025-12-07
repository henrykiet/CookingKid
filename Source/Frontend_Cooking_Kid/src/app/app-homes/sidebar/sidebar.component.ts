import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { menuData, SidebarToggle } from '../../models/side-data';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { IMetadataForm } from '../../models/dynamic.model';
import { DynamicService } from '../../services/dynamic.service';
import { HandleService } from '../../services/handle.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent implements OnInit {
  @Output() onToggleSidebar: EventEmitter<SidebarToggle> = new EventEmitter();
  @Output() controllerName: EventEmitter<string> = new EventEmitter<string>();
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
  @Input() metaform: IMetadataForm | null = null;
  isHover = false;
  collapsed = false;
  screenWidth = 0;
  sideData: menuData[] = [];
  previousExpandedItems: menuData[] = [];
  constructor(
    private dynamicService: DynamicService,
    private elementRef: ElementRef,
    private handleService: HandleService
  ) {}

  ngOnInit(): void {
    this.getMenus();
  }

  //call api get menus
  getMenus() {
    this.metaform = new Object() as IMetadataForm;
    this.metaform.controller = 'menus';
    this.metaform.action = 'list';
    this.dynamicService.getMetadataForm(this.metaform).subscribe({
      next: (res) => {
        if (res) {
          this.metaform = res;
          console.log(this.metaform);
          //map to sideBar data
          const newSideData =
            this.metaform.form?.initialDatas.map((item: any) => {
              // Tìm các subItem thuộc về parent hiện tại
              const children: menuData[] = [];
              this.metaform?.form?.detailForms?.forEach((childForm: any) => {
                childForm.initialDatas.forEach((subItem: any) => {
                  if (subItem.menu_id == item.menu_id) {
                    children.push({
                      label: subItem.label ?? '',
                      routeLink: subItem.routeLink ?? '',
                      icon: subItem.icon ?? '',
                      expanded: subItem.expanded ?? false,
                      controller: subItem.controller ?? '',
                    });
                  }
                });
              });

              return {
                label: item.label,
                routeLink: item.routeLink,
                controller: item.controller,
                icon: item.icon,
                expanded: item.expanded ?? false,
                children: children.length > 0 ? children : undefined,
              } as menuData;
            }) ?? [];
          this.sideData = newSideData;
          localStorage.setItem('sideData', JSON.stringify(this.sideData));
        } else {
          console.error('No form configuration found');
        }
      },
      error: (err) => {
        console.error('Error loading form', err);
      },
    });
  }

  loadMenu(item: any[]): menuData[] {
    return item.map((i) => ({
      label: i.label,
      routeLink: i.routeLink,
      icon: i.icon,
      expanded: i.expanded ?? false,
      children: i.children ? this.loadMenu(i.children) : undefined,
    }));
  }

  //handle router link click
  onRouterLinkClick(controller: string | undefined): void {
    console.log('Router link clicked. Controller:', controller);
    if (controller) {
      this.handleService.setController(controller);
    }
  }

  //click logo
  openSidebar() {
    this.collapsed = !this.collapsed;
    this.toggleCollapse(this.collapsed);
  }

  //handle open and close sidebar
  toggleCollapse(isCollapse: boolean): void {
    // Logic lưu/phục hồi menu dựa trên tham số isCollapse
    if (isCollapse) {
      // Logic LƯU và ĐÓNG menu con (khi thu gọn)
      this.previousExpandedItems = this.sideData.filter(
        (item) => item.expanded
      );
      this.closeAllSubMenus();
    } else {
      // Logic PHỤC HỒI menu (khi mở rộng)
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

  //dropdown child
  onItemClick(item: menuData, event: MouseEvent) {
    if (item.children) {
      event.preventDefault();
      item.expanded = !item.expanded;
    }
    this.onToggleSidebar.emit({
      collapsed: this.collapsed,
      screenWidth: this.screenWidth,
    });
  }

  //close submenu
  closeAllSubMenus(): void {
    this.sideData.forEach((item) => {
      if (item.children) {
        item.expanded = false;
      }
    });
  }

  @HostListener('document:click', ['$event'])
  handleClickOutside(event: Event) {
    // 1. Kiểm tra xem click có nằm NGOÀI sidebar không
    if (!this.elementRef.nativeElement.contains(event.target)) {
      // 2. Nếu sidebar đang MỞ RỘNG (collapsed = false) HOẶC đang HOVER mở rộng
      if (this.collapsed === false || this.isHover === true) {
        // 3. SET trạng thái thu gọn và gọi hàm xử lý
        this.collapsed = true; // Set trạng thái THU GỌN
        this.isHover = false; // Tắt trạng thái hover
        this.toggleCollapse(true); // Gọi hàm xử lý menu (true = thu gọn)
      }
    }
  }

  // Di chuột VÀO sidebar
  @HostListener('mouseenter')
  onMouseEnter() {
    if (this.collapsed === true || this.isHover === false) {
      this.collapsed = false;
      this.isHover = true;
      this.toggleCollapse(false);
    }
  }

  // Di chuột RA khỏi sidebar
  @HostListener('mouseleave')
  onMouseLeave() {
    if (this.collapsed === false || this.isHover === true) {
      this.collapsed = true;
      this.isHover = false;
      this.toggleCollapse(true);
    }
  }
}
