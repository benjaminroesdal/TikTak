import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BlobStorageService {
  private baseUrl = 'http://yourapi.com/blobstorage';

  constructor(private http: HttpClient) { }

  uploadBlob(file: File): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    return this.http.post(`${this.baseUrl}/PostBlob`, formData);
  }

  removeBlob(blobName: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/RemoveBlob`, { blobName });
  }

  getBlobManifest(id: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/GetBlobManifest?id=${id}`, { responseType: 'blob' });
  }

  getFyp(): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetFyp`);
  }
}
