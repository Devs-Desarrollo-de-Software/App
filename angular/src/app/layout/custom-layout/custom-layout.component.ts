import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { CustomSidebarComponent } from '../custom-sidebar/custom-sidebar.component';
import { CustomNavbarComponent } from '../custom-navbar/custom-navbar.component';

@Component({
  selector: 'app-custom-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, CustomSidebarComponent, CustomNavbarComponent],
  templateUrl: './custom-layout.component.html',
  styleUrls: ['./custom-layout.component.scss']
})
export class CustomLayoutComponent {
  sidebarCollapsed = false;

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}
