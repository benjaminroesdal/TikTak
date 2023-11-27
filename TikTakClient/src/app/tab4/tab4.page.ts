import { Component, OnInit } from '@angular/core';
import { FilePicker } from '@capawesome/capacitor-file-picker';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Filesystem } from '@capacitor/filesystem';


@Component({
  selector: 'app-tab4',
  templateUrl: './tab4.page.html',
  styleUrls: ['./tab4.page.scss'],
})
export class Tab4Page implements OnInit {
  formData = new FormData();
  mediaRecorder: any;
  videoPlayer: any;

  constructor(private http: HttpClient) { }


  appendFileToFormData = async () => {
    console.log("YOYOYOY");
    await FilePicker.pickMedia().then(e =>{
      const file = e.files[0];
      console.log("Hej med dig");
      console.log(e);
      if (file.blob) {
        const rawFile = new File([file.blob], 'file', {
          type: file.mimeType,
        });
        this.formData.append('file', rawFile, 'file');
        console.log(this.formData.get('file'));
        this.http.post('https://c56d-93-176-82-58.ngrok-free.app/BlobStorage/PostBlob', this.formData).subscribe(e => {
          console.log(e);
        });
      }
      if(!file.blob){
        this.readFilePath(file.path).then(e => {
          this.convertDataToBlob(e)
          console.log("DET HER ER FOR ANDROID");
          console.log(this.formData);
          this.http.post('https://c56d-93-176-82-58.ngrok-free.app/BlobStorage/PostBlob', this.formData).subscribe(e => {
            console.log(e);
          });
        })
      }
    });
  };

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
