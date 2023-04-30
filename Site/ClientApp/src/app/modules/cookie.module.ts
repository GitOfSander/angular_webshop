import { ModuleWithProviders, NgModule, Injectable, Injector, APP_INITIALIZER } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';
import { Http, Headers } from '@angular/http';
import { Observable } from "rxjs";

export module CookieModule {
  export function read(name: string) {
    if (typeof document !== 'undefined') {
      var result = new RegExp('(?:^|; )' + encodeURIComponent(name) + '=([^;]*)').exec(document.cookie);
      return result ? result[1] : null;
    }
  }

  export function write(name: string, value: string, days?: number) {
    if (typeof document !== 'undefined') {
      if (!days) {
        days = 365 * 20;
      }

      var date = new Date();
      date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));

      var expires = "; expires=" + date.toUTCString();

      document.cookie = name + "=" + value + expires + "; path=/";
    }
  }

  export function remove(name: string) {
    write(name, "", -1);
  }
}
