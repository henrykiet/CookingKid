import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-leaf-wrapper',
  standalone: true,
  imports: [],
  templateUrl: './leaf-wrapper.component.html',
  styleUrl: './leaf-wrapper.component.scss',
})
export class LeafWrapperComponent {
  @Input() fill: string = 'lightgreen'; // màu lá
  @Input() stroke: string = 'green'; // viền lá
}
