import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { VideoInfoModel } from '../models/video';

@Injectable({
  providedIn: 'root'
})
export class BlobStorageService {
  private apiBaseUrl = environment.firebase.apiBaseUrl;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'ngrok-skip-browser-warning': '69420'
    });
  }

  uploadBlob(file: File) {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(`${this.apiBaseUrl}/BlobStorage/PostBlob`, formData);
  }

  removeBlob(blobName: string) {
    return this.http.post(`${this.apiBaseUrl}/BlobStorage/RemoveBlob`, { blobName });
  }

  getBlobManifest(id: string) {
    const headers = this.getHeaders();
    return this.http.get(`${this.apiBaseUrl}/BlobStorage/GetBlobManifest?Id=${id}`, { headers });
  }

  getFyp(): Observable<VideoInfoModel[]> {
    //this recieves x videos to the fyp
    return this.http.get<VideoInfoModel[]>(`${this.apiBaseUrl}/BlobStorage/GetFyp`);
  }
}
