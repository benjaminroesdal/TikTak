import { Component, OnInit } from '@angular/core';
import { FilePicker } from '@capawesome/capacitor-file-picker';
import { HttpClient } from '@angular/common/http';
import { Filesystem } from '@capacitor/filesystem';
import { ToastService } from '../services/toast.service';
import { environment } from '../../environments/environment';
import { LoadingController } from '@ionic/angular';


@Component({
  selector: 'app-tab4',
  templateUrl: './tab4.page.html',
  styleUrls: ['./tab4.page.scss'],
})
export class Tab4Page implements OnInit {
  currentTag: string = '';
  videoFile: File | null = null;
  videoName: string = 'No file selected!';
  tagsArray: string[] = [];
  formData = new FormData();
  loadingElement!: HTMLIonLoadingElement;
  maxTags: number = 5;
  private apiBaseUrl = environment.firebase.apiBaseUrl;

  constructor(private http: HttpClient, private toastService: ToastService, private loadingCtrl: LoadingController) { }



  uploadFile() {
    this.showLoading();
    this.tagsArray.forEach((tag, index) => {
      this.formData.append(`Tags[${index}].Name`, tag);
    });
    this.http.post(`${this.apiBaseUrl}/BlobStorage/PostBlob`, this.formData).subscribe(e => {
      this.videoName = 'No file selected';
      this.formData = new FormData();
      this.tagsArray = [];
      this.loadingElement.dismiss();
      this.toastService.showToast("Video Uploaded!")
    }, error => {
      this.loadingElement.dismiss();
    });
  }

  appendFileToFormData = async () => {
    await FilePicker.pickMedia().then(e => {
      const file = e.files[0];
      this.videoName = e.files[0].name;
      if (file.blob) {
        const rawFile = new File([file.blob], 'file', {
          type: file.mimeType,
        });
        this.formData.append('file', rawFile, 'file');
      }
      if (!file.blob) {
        this.readFilePath(file.path).then(e => {
          this.convertDataToBlob(e);
        })
      }
    });
  };

  async showLoading() {
    const loading = await this.loadingCtrl.create({
      message: 'Uploading ' + this.videoName + '... Please wait',
    });
    loading.present();
    this.loadingElement = loading;
  }

  onFileSelected(event: Event) {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList) {
      this.videoFile = fileList[0];
    }
  }

  addTag() {
    if (this.currentTag.trim() && !this.tagsArray.includes(this.currentTag.trim())) {
      if (this.tagsArray.length < this.maxTags) {
        this.tagsArray.push(this.currentTag.trim());
        this.currentTag = '';
      } else {
        this.toastService.showToast("maximum number of tags reached.")
      }
    }
  }

  removeTag(tagToRemove: string) {
    this.tagsArray = this.tagsArray.filter(tag => tag !== tagToRemove);
  }

  async readFilePath(path: any): Promise<string> {
    const contents = await Filesystem.readFile({
      path: path,
    });
    return contents.data as string;
  };

  convertDataToBlob(data: any) {
    const base64String = data;
    // Convert Base64 string to a Blob
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const fileBlob = new Blob([byteArray], { type: 'video/mp4' });
    // Append the Blob to FormData
    this.formData.append('file', fileBlob, 'file');
  }

  ngOnInit() {
  }

}

export interface VideoModel {
  Tags: string[];
  File: FormData;
}
