import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BlobStorageService {
  private baseUrl = 'https://localhost:7001/BlobStorage';

  constructor(private http: HttpClient) { }

  uploadBlob(file: File) {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(`${this.baseUrl}/PostBlob`, formData);
  }

  removeBlob(blobName: string) {
    return this.http.post(`${this.baseUrl}/RemoveBlob`, { blobName });
  }

  getBlobManifest(id: string) {
    return this.http.get(`${this.baseUrl}/GetBlobManifest?id=${id}`, { responseType: 'blob' });
  }

  getFyp() {
    return this.http.get(`${this.baseUrl}/GetFyp`);
  }
}
