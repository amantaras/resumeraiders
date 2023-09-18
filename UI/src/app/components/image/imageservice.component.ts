import { Component, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

class ImageSnippet {
  constructor(public src: string, public file: File) {}
}

@Component({
  selector: 'image-component',
  templateUrl: './imageservice.component.html',
})
export class ImageService {
  selectedFile!: ImageSnippet;

  @Output() fileUploadedEvent = new EventEmitter<String>();

  constructor(private http: HttpClient) {}

  processFile(imageInput: any) {
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {
      console.log('loaded image');
      console.log(event.target.result);
      var imageContent = event.target.result.replace(
        /^data:image\/png;base64,/,
        ''
      );
      this.fileUploadedEvent.emit(imageContent);
      console.log('Writing base64 value');
      console.log(imageContent);
    });

    reader.readAsDataURL(file);
  }
}
