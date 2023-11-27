import { Component, ViewChild, ElementRef, AfterViewInit, ViewChildren, QueryList, ChangeDetectorRef } from '@angular/core';
import Hls from 'hls.js';
import { ActivatedRoute, Router } from '@angular/router';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { BlobStorageService } from '../services/blob-storage.service';
import { VideoService } from '../services/video.service';
import { Observable, forkJoin } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth.service';
import { Swiper } from 'swiper';
import { register } from 'swiper/element/bundle';
import { SwiperOptions } from 'swiper/types/swiper-options';


@Component({
  selector: 'app-tab2',
  templateUrl: 'tab2.page.html',
  styleUrls: ['tab2.page.scss']
})
export class Tab2Page implements AfterViewInit {
  @ViewChild('swiperRef', { static: false }) // Change to "false"
  protected _swiperRef: ElementRef | undefined
  public videoSources: Video[] = [];
 // Initialize as empty
  @ViewChildren('video') videoElements!: HTMLCollectionOf<Element>;
  user: any;

  slideOpts = {
    direction: 'vertical'
  };

  private baseUrl = ' https://reliably-generous-grub.ngrok-free.app/BlobStorage';
  // Add a state variable to track if new videos are ready to be played
  newVideosReadyToPlay = false;
  
  constructor(private route: ActivatedRoute, private router: Router, private blobStorageService:BlobStorageService,
     private authService:AuthService, private videoService: VideoService, private cdr: ChangeDetectorRef) {
  }

  async ngOnInit() {
    await this.loadVideos(); // Load videos first
    // Removed this.cdr.detectChanges() from here
  }

  ngAfterViewInit() {
    // Delay the initialization of Swiper to ensure everything is loaded
    setTimeout(() => {
      const swiperEl = this._swiperRef?.nativeElement;
      swiperEl.initialize();
      this.playFirstVideo(); // Play the first video
    }, 500); // You may adjust this delay as necessary
  }
  
  async signOut() {
    GoogleAuth.signOut().then(() => {
      this.router.navigate(['tabs/tab1']);
    });
    await this.authService.Logout();
  }


  likeVideo(i:any){
    console.log(i)
  }

  private playFirstVideo() {
    if (this.videoSources.length > 0) {
      const firstVideoElement = document.getElementById('video_0') as HTMLVideoElement;
      if (firstVideoElement) {
        console.log('First video element found:', firstVideoElement);
        console.log('Video source:', firstVideoElement.src);
        console.log('Video readyState:', firstVideoElement.readyState);
  
        firstVideoElement.play().then(() => {
          console.log('First video is playing.');
        }).catch(err => {
          console.error('Error playing first video:', err);
        });
      } else {
        console.error('First video element not found.');
      }
    } else {
      console.log('No video sources available.');
    }
  }

  setupHlsPlayer(videoSrc: string, index: number) {
    const videoElementId = 'video_' + index;
    const videoElement = document.getElementById(videoElementId) as HTMLVideoElement;
    this.initHlsPlayer(videoElement, videoSrc);
  }

  async loadVideos(): Promise<void> {
    console.log('Fetching initial videos');
    this.newVideosReadyToPlay = false;
    return new Promise((resolve, reject) => {
      this.blobStorageService.getFyp().subscribe(
        (response: any[]) => {
          if (!Array.isArray(response)) {
            throw new Error('Response is not an array');
          }
          const videos = response.map(video => ({
            ...video,
            manifestUrl: `https://tiktakstorage.blob.core.windows.net/tiktaks/${video.blobVideoStorageId}.M3U8`
          }));
  
          this.videoSources = [...this.videoSources, ...videos];
          this.videoSources.forEach((video, index) => this.setupHlsPlayer(video.manifestUrl, index));
          this.newVideosReadyToPlay = true;
          this.cdr.detectChanges();
          resolve();
        },
        error => {
          console.error('Error occurred:', error);
          reject(error);
        }
      );
    });
  }
  
  onSlideChange(swiperEvent: any) {
    const currentIndex = swiperEvent.detail[0].activeIndex;
    const currentVideoElement = document.getElementById('video_' + currentIndex) as HTMLVideoElement;
    const previousIndex = currentIndex > 0 ? currentIndex - 1 : this.videoSources.length - 1;
    const nextIndex = currentIndex < this.videoSources.length - 1 ? currentIndex + 1 : 0;
    const previousVideoElement = document.getElementById('video_' + previousIndex) as HTMLVideoElement;
    const nextVideoElement = document.getElementById('video_' + nextIndex) as HTMLVideoElement;
    const isLastVideo = currentIndex === this.videoSources.length - 1;

    // Pause the previous and next videos to handle both forward and backward navigation
    if (previousVideoElement && previousIndex !== currentIndex) {
        previousVideoElement.currentTime = 0;
        previousVideoElement.pause();
    }
    if (nextVideoElement && nextIndex !== currentIndex) {
        nextVideoElement.currentTime = 0;
        nextVideoElement.pause();
    }

    // Load more videos if the last video is reached
    if (isLastVideo) {
        this.loadVideos().then(() => {
            console.log("Loaded more videos and updated Swiper.");
            const swiperEl = this._swiperRef?.nativeElement;
            swiperEl.swiper.update();
            setTimeout(() => {
            currentVideoElement.play();
            }, 500);
        }).catch(error => console.error("Error loading more videos:", error));
    }

    // Only play the video if new videos are ready
    if (this.newVideosReadyToPlay && currentVideoElement && currentVideoElement.readyState >= 4) {
        currentVideoElement.play().catch(err => console.error('Error playing video:', err));
    }
  }

  private initHlsPlayer(video: HTMLVideoElement, source: string): void {
    console.log("init" + source)
    console.log("init" + video)

    if (Hls.isSupported()) {
      const hls = new Hls();
      hls.loadSource(source);
      hls.attachMedia(video);
      
      hls.on(Hls.Events.ERROR, function (event, data) {
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              // handle network error
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              console.log("error")
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
        video.play();
      });
    }
  }
  
  
}
export interface Video {
  userId: number;
  profileImage: string;
  email: string;
  blobVideoStorageId: string;
  manifestUrl: string;
}