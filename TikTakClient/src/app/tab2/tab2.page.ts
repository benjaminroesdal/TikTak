import { Component, ViewChild, ElementRef, AfterViewInit, ViewChildren, QueryList, ChangeDetectorRef } from '@angular/core';
import Hls from 'hls.js';
import { ActivatedRoute, Router, NavigationStart } from '@angular/router';
import { GoogleAuth } from '@codetrix-studio/capacitor-google-auth';
import { BlobStorageService } from '../services/blob-storage.service';
import { VideoService } from '../services/video.service';
import { mergeMap } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth.service';
import { ToastService } from '../services/toast.service';
import { Video } from '../models/video';
import { UserTagInteraction } from '../models/userTagInteraction';
import { IonDatetime } from '@ionic/angular';
import { Like } from '../models/like';
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-tab2',
  templateUrl: 'tab2.page.html',
  styleUrls: ['tab2.page.scss']
})
export class Tab2Page implements AfterViewInit {
  private apiBaseUrl = environment.firebase.apiBaseUrl;
  @ViewChild('swiperRef', { static: false }) // Change to "false"
  protected _swiperRef: ElementRef | undefined
  public videoSources: string[] = []; // Initialize as empty
  @ViewChildren('video') videoElements!: HTMLCollectionOf<Element>;
  public userName!: string;
  user: any;

  slideOpts = {
    direction: 'vertical'
  };

  // Add a state variable to track if new videos are ready to be played
  newVideosReadyToPlay = false;
  
  constructor(private route: ActivatedRoute, private router: Router, private blobStorageService:BlobStorageService,
     private authService:AuthService, private videoService: VideoService, private cdr: ChangeDetectorRef, private toastService: ToastService) {
      
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
    }, 1000); // You may adjust this delay as necessary
  }

  ngOnDestroy() {
    if (this.videoElements) {
      for (let video of Array.from(this.videoElements)) {
        let videoElement = video as HTMLVideoElement;
        videoElement.pause();
        videoElement.removeAttribute('src'); // remove source
        videoElement.load();
      }
    }
  }
  
  async signOut() {
    GoogleAuth.signOut().then(() => {
      this.router.navigate(['tabs/tab1']);
    });
    await this.authService.Logout();
  }


  likeVideo(blobVideoStorageId: string){
    console.log("Like video called with ID:", blobVideoStorageId);
    const like: Like = {
      blobStorageId:blobVideoStorageId,
      likeDate: new Date()
    }
    this.videoService.registerVideoLike(like).subscribe();
    if (!blobVideoStorageId) {
        console.error("blobVideoStorageId is undefined");
    }
  }

  startFiveSecondTimer(videoElement: HTMLVideoElement, blobVideoStorageId: string) {
    setTimeout(() => {
      if (!videoElement.paused) { // Check if the video is still playing
        const userTagInteraction: UserTagInteraction = {
          blobStorageId:blobVideoStorageId
        }
        this.videoService.countUserVideoInteraction(userTagInteraction).subscribe();    
        console.log(`BlobStorageID ${blobVideoStorageId} has been running for 5 seconds`);
      }
    }, 5000); // Set timeout for 5 seconds
  }

  private playFirstVideo() {
    if (this.videoSources.length > 0) {
        const firstVideoElement = document.getElementById('video_0') as HTMLVideoElement;
        if (firstVideoElement) {
            firstVideoElement.play().catch(err => console.error('Error playing first video:', err));
        }
    }
  }

  setupHlsPlayer(videoSrc: string, index: number, blobVideoStorageId: string) {
    const videoElementId = 'video_' + index;
    const videoElement = document.getElementById(videoElementId) as HTMLVideoElement;
    this.initHlsPlayer(videoElement, videoSrc, blobVideoStorageId);
  }
  

  public videosArray: any[] = []; // To store the video objects


  async loadVideos(): Promise<void> {
    this.newVideosReadyToPlay = false;
    return new Promise((resolve, reject) => {
      this.blobStorageService.getFyp().subscribe((videoInfoList: any[]) => {
        console.log(videoInfoList);
        this.videosArray.push(...videoInfoList);
        setTimeout(() => {
          this.videosArray.forEach(item1 => {
            const matchingItem = videoInfoList.find(item2 => item2.id === item1.id);
            if (matchingItem) {
              item1.manifestUrl = `${this.apiBaseUrl}/BlobStorage/GetBlobManifest?id=` + matchingItem.video.blobStorageId;
            }
          });
          console.log(this.videosArray)
          this.videosArray.forEach((video, index) => {
            // Ensure that video.blobVideoStorageId is the correct property from your video object
            this.setupHlsPlayer(`${this.apiBaseUrl}/BlobStorage/GetBlobManifest?id=` + video.video.blobStorageId, index, video.video.blobStorageId);
          });
          this.newVideosReadyToPlay = true;
          resolve();
          this.cdr.detectChanges();
        }, 700);
      }, error => {
        this.toastService.showToast("Loading videos failed, please try again.");
      });
    });
  }

  onSlideChange(swiperEvent: any) {
    const currentIndex = swiperEvent.detail[0].activeIndex;
    const currentVideoElement = document.getElementById('video_' + currentIndex) as HTMLVideoElement;
    const previousIndex = currentIndex > 0 ? currentIndex - 1 : this.videosArray.length - 1;
    const nextIndex = currentIndex < this.videosArray.length - 1 ? currentIndex + 1 : 0;
    const previousVideoElement = document.getElementById('video_' + previousIndex) as HTMLVideoElement;
    const nextVideoElement = document.getElementById('video_' + nextIndex) as HTMLVideoElement;
    const isLastVideo = currentIndex === this.videosArray.length - 1;

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
            const swiperEl = this._swiperRef?.nativeElement;
            swiperEl.swiper.update();
            setTimeout(() => {
            currentVideoElement.play();
            }, 700);
        }).catch(() => {
          this.toastService.showToast("Loading videos failed, please try again.");
        });
    }

    // Only play the video if new videos are ready
    if (this.newVideosReadyToPlay && currentVideoElement && currentVideoElement.readyState >= 4) {
        currentVideoElement.play().catch(() => {
          this.toastService.showToast("Loading video failed, please try again.");
        });
    }
  }

  private initHlsPlayer(video: HTMLVideoElement, source: string, blobVideoStorageId: string): void {
    if (Hls.isSupported()) {
        this.setupHls(video, source);
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        this.setupNativeVideoPlayer(video, source);
    }

    this.addVideoEventListeners(video, blobVideoStorageId);
}

private setupHls(video: HTMLVideoElement, source: string): void {
    const hls = new Hls();
    hls.loadSource(source);
    hls.attachMedia(video);
    hls.on(Hls.Events.MANIFEST_PARSED, () => video.pause());
    hls.on(Hls.Events.ERROR, (event, data) => this.handleHlsError(hls, data));
}

private setupNativeVideoPlayer(video: HTMLVideoElement, source: string): void {
    video.src = source;
    video.addEventListener('loadedmetadata', () => video.pause());
}

private handleHlsError(hls: Hls, data: any): void {
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
}

private addVideoEventListeners(video: HTMLVideoElement, blobVideoStorageId: string): void {
    video.addEventListener('play', () => this.startFiveSecondTimer(video, blobVideoStorageId));
    video.addEventListener('ended', () => console.log(`Video with blobVideoStorageId ${blobVideoStorageId} has finished playing.`));
}


}

