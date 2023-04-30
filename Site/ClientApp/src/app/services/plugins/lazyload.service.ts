import { Injectable } from '@angular/core';
import LazyLoad from 'vanilla-lazyload/dist/lazyload';

@Injectable({
  providedIn: 'root',
})
export class LazyLoadService {
  public lazyLoad: any = "";

  constructor() {
    this.lazyLoad = null;
  }

  public setLazyLoad() {
    this.lazyLoad = new LazyLoad({
      elements_selector: '.lazy'
    });
  }

  public getLazyLoad() {
    return this.lazyLoad;
  }

  public update() {
    this.lazyLoad.update();
  }

  public load(element: any, force: boolean = false) {
    this.lazyLoad.load(element, force);
  }

  public loadAll() {
    this.lazyLoad.loadAll();
  }
}
