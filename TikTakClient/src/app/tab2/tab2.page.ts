import { Component, ViewChild, ElementRef, AfterViewInit, ViewChildren, QueryList } from '@angular/core';
import Hls from 'hls.js';
import { Swiper } from 'swiper/types';
import { ActivatedRoute, Router } from '@angular/router';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
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

  constructor(private route: ActivatedRoute, private router: Router, private authService: AuthService) {
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

  ngOnInit(): void {
    this.loadInitialVideos();
  }

  ngAfterViewInit() {
    this.videoSources.forEach(video => this.setupHlsPlayer(video));
  
    // Adding a slight delay to ensure HLS setup is complete
    setTimeout(() => {
      this.playFirstVideo();
    }, 500); // Adjust the delay as necessary
  }
  

  private playFirstVideo() {
    if (this.videoElements && this.videoElements.length > 0) {
      const firstVideoElement = this.videoElements[0] as HTMLVideoElement;
      if (firstVideoElement) {
        firstVideoElement.play().catch(err => console.error('Error playing first video:', err));
      }
    }
  }


  setupHlsPlayer(video: string): void {
    const videoElement = document.getElementById(video) as HTMLVideoElement;
    let som = document.getElementsByClassName('videoName') as HTMLCollection;
    this.videoElements = som;
    this.initHlsPlayer(videoElement, video);
  }

  private loadInitialVideos() {
    // Fetch initial video sources (blobIds) from the backend
    // Add them to the videoSources array
    // For example:
    console.log('we in here');
    this.videoSources = this.videoSources.concat([
      'https://tiktakstorage.blob.core.windows.net/tiktaks/0ef6f12e-8f30-4980-97c2-b7c9ac391a63.M3U8',
      'https://tiktakstorage.blob.core.windows.net/tiktaks/831b5d59-cb28-4c03-82e4-3782edb110ed.M3U8',
      'https://tiktakstorage.blob.core.windows.net/tiktaks/8fe579fb-c191-453b-a61d-73b472e3e124.M3U8',
      'https://tiktakstorage.blob.core.windows.net/tiktaks/933c546f-8401-44f4-8be9-ac56d8c20df4.M3U8',
      // ... more initial sources
    ]);
  }

  onSlideChange(swiperEvent: any) {
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