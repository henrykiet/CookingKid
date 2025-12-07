import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';
import { ApplicationRef } from '@angular/core';

export default function bootstrap(
  platformContext: any
): Promise<ApplicationRef> {
  // Pass the server config AND the platformContext
  return bootstrapApplication(AppComponent, config, platformContext);
}
