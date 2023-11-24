import { Component, ViewChild, ElementRef, AfterViewInit, ViewChildren, QueryList, ChangeDetectorRef } from '@angular/core';
import Hls from 'hls.js';
import { Swiper } from 'swiper/types';
import { ActivatedRoute, Router } from '@angular/router';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { BlobStorageService } from '../services/blob-storage.service';
import { Observable, forkJoin } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import {AuthService} from 'src/app/services/auth.service';


@Component({
  selector: 'app-tab2',
  templateUrl: 'tab2.page.html',
  styleUrls: ['tab2.page.scss']
})
export class Tab2Page implements AfterViewInit {
  public videoSources: string[] = []; // Initialize as empty
  @ViewChildren('video') videoElements!: HTMLCollectionOf<Element>;
  @ViewChild('swiper') 
  swiperRef: ElementRef | undefined;
  swiper?: Swiper;
  user: any;

  slideOpts = {
    direction: 'vertical'
  };

  private baseUrl = 'https://localhost:7001/BlobStorage';
  
  constructor(private route: ActivatedRoute, private router: Router, private blobStorageService:BlobStorageService,
     private authService:AuthService,private cdr: ChangeDetectorRef) {
    this.route.queryParams.subscribe(params => {
      let data = this.router.getCurrentNavigation()!.extras.state;
      if (data!['user']) {
          this.user = data!['user'];
      }
    });
  }

  async signOut() {
    GoogleAuth.signOut().then(() => {
      this.router.navigate(['tabs/tab1']);
    });
    await this.authService.Logout();
  }

  async ngOnInit() {
    await this.loadInitialVideos();
    this.cdr.detectChanges(); // Manually trigger change detection
    this.videoSources.forEach(video => this.setupHlsPlayer(video));
    console.log(this.videoElements);
}

ngAfterViewInit() {
   // Adding a slight delay to ensure HLS setup is complete
   setTimeout(() => {
    this.playFirstVideo();
  }, 700); // Adjust the delay as necessary
}
  

  private playFirstVideo() {
    if (this.videoElements && this.videoElements.length > 0) {
      const firstVideoElement = this.videoElements[0] as HTMLVideoElement;
      if (firstVideoElement) {
        firstVideoElement.play().catch(err => console.error('Error playing first video:', err));
      }
    }
  }


  setupHlsPlayer(video: string) {
    console.log('we in here ' +video)
    const videoElement = document.getElementById(video) as HTMLVideoElement;
    let som = document.getElementsByClassName('videoName') as HTMLCollection;
    this.videoElements = som;
    console.log(videoElement)
    this.initHlsPlayer(videoElement, video);
    
  }

  private async loadInitialVideos(): Promise<void> {
    console.log('Fetching initial videos');
    return new Promise((resolve, reject) => {
        this.blobStorageService.getFyp().pipe(
            mergeMap((ids: string[]) => {
                return ids.map(id => `${this.baseUrl}/GetBlobManifest?Id=${id}`);
            })
        ).subscribe((urls: string) => {
            this.videoSources.push(urls)
            resolve();
        }, error => {
            console.error('Error occurred:', error);
            reject(error);
        });
    });
}

  

  onSlideChange(swiperEvent: any) {
    console.log(swiperEvent)
    let count = swiperEvent.detail[0].activeIndex;
    let videoEl = this.videoElements[count] as HTMLVideoElement;
    let videoLast = this.videoElements[count - 1] as HTMLVideoElement;
    videoEl.play();
    videoLast.currentTime = 0;
    videoLast.pause();
    console.log('changed', count);
  }
  
  
  private initHlsPlayer(video: HTMLVideoElement, source: string): void {
    if (Hls.isSupported()) {
      const hls = new Hls();
      hls.loadSource(source);
      hls.attachMedia(video);
      hls.on(Hls.Events.MANIFEST_PARSED, function () {
        video.pause();
      });
      hls.on(Hls.Events.ERROR, function (event, data) {
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              // handle network error
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              // handle media error
              break;
            default:
              // handle other error types
              hls.destroy();
              break;
          }
        }
      });
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
      video.src = source;
      video.addEventListener('loadedmetadata', function () {
        video.pause();
      });
    }
  }
}

export interface Video {
  id: string;
  title: string;
  manifestUrl: string; // HLS manifest URL
  thumbnail: string;
}