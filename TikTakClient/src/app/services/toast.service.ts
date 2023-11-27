import { Injectable } from '@angular/core';
import { Toast } from '@capacitor/toast';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor() { }

  async showToast(textShow:string) {
    await Toast.show({
      text: textShow,
      duration: "long",
      position: 'top'
    });
  };
}
