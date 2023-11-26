import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage-angular';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  public _storage: Storage | null = null;

  constructor(private storage: Storage) {
    this.init();
  }

  public async init() : Promise<void> {
    // If using, define drivers here: await this.storage.defineDriver(/*...*/);
    const storage = await this.storage.create();
    this._storage = storage;
  }

  public async set(key: string, value: any) : Promise<void> {
    await this._storage?.set(key, value);
  }

  public async get(key: string) : Promise<string> {
    return await this._storage?.get(key).then(e => {
      return e;
    });
  }

  public async remove(key: string) : Promise<void> {
    await this._storage?.remove(key);
  }

  public async clear() : Promise<void> {
    await this._storage?.clear();
  }
}
