import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit {
  isShowUserMenu: boolean = false;
  @Input()
  toggleUser() {
    this.isShowUserMenu = !this.isShowUserMenu;
  }

  constructor() {}
  ngOnInit(): void {}
}
