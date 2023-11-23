import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface UserTagInteraction {
  userId: number;
  videoId: number;
}

interface Like {
  userId: number;
  videoId: number;
  likeDate: Date;
}

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private baseUrl = 'http://yourapi.com/video'; 

  constructor(private http: HttpClient) { }

  countUserVideoInteraction(tagInteraction: UserTagInteraction): Observable<any> {
    return this.http.post(`${this.baseUrl}/CountUserVideoInteraction`, tagInteraction);
  }

  registerVideoLike(like: Like): Observable<any> {
    return this.http.post(`${this.baseUrl}/RegisterVideoLike`, like);
  }
}
