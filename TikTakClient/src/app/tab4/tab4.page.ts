import { Component, OnInit } from '@angular/core';
import { FilePicker } from '@capawesome/capacitor-file-picker';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Filesystem } from '@capacitor/filesystem';
import { Toast } from '@capacitor/toast';
import { Video } from '../tab2/tab2.page';


@Component({
  selector: 'app-tab4',
  templateUrl: './tab4.page.html',
  styleUrls: ['./tab4.page.scss'],
})
export class Tab4Page implements OnInit {
  currentTag: string = ''; // This will hold the value of the tag input
  videoFile: File | null = null;
  tagsArray: string[] = [];
  formData = new FormData();
  mediaRecorder: any;
  videoPlayer: any;
  maxTags: number = 5; // Set your desired maximum number of tags

  constructor(private http: HttpClient) { }



  uploadFile() {
    // Append each tag separately
    this.tagsArray.forEach((tag, index) => {
      this.formData.append(`Tags[${index}].Name`, tag);
    });
    this.http.post('https://ee5d-93-176-82-57.ngrok-free.app/BlobStorage/PostBlob', this.formData).subscribe(e => {
      console.log(e);
    });
  }

  appendFileToFormData = async () => {
    await FilePicker.pickMedia().then(e =>{
      const file = e.files[0];
      if (file.blob) {
        const rawFile = new File([file.blob], 'file', {
          type: file.mimeType,
        });
        this.formData.append('file', rawFile, 'file');
      }
      if(!file.blob){
        this.readFilePath(file.path).then(e => {
          this.convertDataToBlob(e);
        })
      }
    });
  };

  onFileSelected(event: Event) {
    const element = event.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList) {
      this.videoFile = fileList[0];
      // You can also implement a preview of the video here
    }
  }

  addTag() {
    if (this.currentTag.trim() && !this.tagsArray.includes(this.currentTag.trim())) {
      if (this.tagsArray.length < this.maxTags) {
        this.tagsArray.push(this.currentTag.trim());
        this.currentTag = ''; // Clear the input
      } else {
        // Optionally, provide user feedback that the max number of tags has been reached
        this.showHelloToast("maximum number of tags reached.")
        console.error('Maximum number of tags reached.');
        // You can use an Ionic toast for better user experience
      }
    }
  }

  async showHelloToast(textShow:string) {
    await Toast.show({
      text: textShow,
      duration: "long",
      position: "center"
    });
  };


  removeTag(tagToRemove: string) {
    this.tagsArray = this.tagsArray.filter(tag => tag !== tagToRemove);
  }

  async readFilePath(path:any) : Promise<string> {
    // Here's an example of reading a file with a full file path. Use this to
    // read binary data (base64 encoded) from plugins that return File URIs, such as
    // the Camera.
    const contents = await Filesystem.readFile({
      path: path,
    });
    return contents.data as string;
  };

  convertDataToBlob(data:any) {
          // Assume 'base64String' is the long base64-encoded data string you retrieved
      const base64String = data; // Your actual base64 string will be much longer
      console.log(base64String);
      // Convert Base64 string to a Blob
      const byteCharacters = atob(base64String);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const fileBlob = new Blob([byteArray], { type: 'video/mp4' }); // Specify the correct MIME type
      console.log(fileBlob);
      // Append the Blob to FormData
      this.formData.append('file', fileBlob, 'file'); // Replace 'filename.mp4' with the actual file name if available
  }

  ngOnInit() {
  }

}

export interface VideoModel {
  Tags: string[];
  File: FormData;
}
