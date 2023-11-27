import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BlobStorageService {
  private baseUrl = 'https://c56d-93-176-82-58.ngrok-free.app/BlobStorage';

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'ngrok-skip-browser-warning': '69420'
    });
  }

  uploadBlob(file: File) {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(`${this.baseUrl}/PostBlob`, formData);
  }

  removeBlob(blobName: string) {
    return this.http.post(`${this.baseUrl}/RemoveBlob`, { blobName });
  }

  getBlobManifest(id: string) {
    const headers = this.getHeaders();
    console.log(headers)
    return this.http.get(`${this.baseUrl}/GetBlobManifest?Id=${id}`, { headers });
  }

  getFyp(): Observable<string[]> {
    //this recieves 3 videos to the fyp
    return this.http.get<string[]>(`${this.baseUrl}/GetFyp`);
  }
}
