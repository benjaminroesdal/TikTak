import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Like } from '../models/like';
import { UserTagInteraction } from '../models/userTagInteraction';


@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private apiBaseUrl = environment.firebase.apiBaseUrl;

  constructor(private http: HttpClient) { }

  countUserVideoInteraction(tagInteraction: UserTagInteraction): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/Video/CountUserVideoInteraction`, tagInteraction);
  }

  registerVideoLike(like: Like): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/Video/RegisterVideoLike`, like);
  }
}
