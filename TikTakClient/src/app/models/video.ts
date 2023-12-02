export interface VideoInfoModel {
    UserId: string;
    ProfileImage: string;
    Email: string; // HLS manifest URL
    Video: Video
  }

  export interface Video {
    BlobStorageId: string;
    UploadDate: string;
    Tags: TagModel[],
    Likes: number
  }

  export interface TagModel {
    Name: string;
  }