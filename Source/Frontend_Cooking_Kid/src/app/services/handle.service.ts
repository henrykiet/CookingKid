import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HandleService {
  private controllerSource = new BehaviorSubject<string | null>(null);
  currentController$: Observable<string | null> =
    this.controllerSource.asObservable();
  constructor() {}

  setController(controllerName: string): void {
    console.log('Setting controller name:', controllerName);
    this.controllerSource.next(controllerName);
  }
}
