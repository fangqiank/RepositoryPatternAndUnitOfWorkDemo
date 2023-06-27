import { Component } from '@angular/core'
import { Driver } from './models/driver'
import { DriverService } from './services/driver.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Frontend';

  drivers: Driver[] = []

  constructor(private driverService: DriverService){}

  ngOnInit(): void{
    this.driverService.getDrivers().subscribe((result: Driver[]) => this.drivers = result)
  } 
}
