import { Injectable } from '@angular/core';
import { Geolocation } from '@capacitor/geolocation';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  constructor() { }

  public async printCurrentPosition() : Promise<any>{
    return await Geolocation.getCurrentPosition().then(e => {
      console.log(e);
      return e;
    });
    };
}
