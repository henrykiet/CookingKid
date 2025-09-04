import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit {
  isShowUserMenu: boolean = false;

  toggleUser() {
    this.isShowUserMenu = !this.isShowUserMenu;
  }

  constructor() {}
  ngOnInit(): void {
    // if (localStorage.getItem('formConfig')) {
    //   this.form = JSON.parse(localStorage.getItem('formConfig') || '{}');
    // }
  }
}
