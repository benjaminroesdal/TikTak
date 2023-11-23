import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage-angular';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private _storage: Storage | null = null;

  constructor(private storage: Storage) {
    this.init();
  }

  async init() {
    // If using, define drivers here: await this.storage.defineDriver(/*...*/);
    const storage = await this.storage.create();
    this._storage = storage;
  }

  public async set(key: string, value: any) : Promise<void> {
    await this._storage?.set(key, value);
  }

  public async get(key: string) : Promise<string> {
    const name = await this._storage?.get(key);
    return name;
  }

  public async remove(key: string) : Promise<void> {
    await this._storage?.remove(key);
  }

  public async clear() : Promise<void> {
    await this._storage?.clear();
  }
}
