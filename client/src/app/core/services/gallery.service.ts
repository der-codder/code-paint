import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ExtensionInfo, GalleryQuery, QueryResult, Extension } from '..';

interface IQueryResultResource {
  totalCount: number;
  items: ExtensionInfo[];
}

const GALERY_URL = 'https://marketplace.visualstudio.com/_apis/public/gallery/extensionquery';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    accept: 'application/json;api-version=3.0-preview.1'
  })
};

@Injectable({
  providedIn: 'root'
})
export class GalleryService {
  private readonly apiUrl = 'http://localhost:5021/api/gallery';

  constructor(private http: HttpClient) { }

  getExtensions(query: GalleryQuery): Observable<QueryResult> {
    return this.http.get<QueryResult>(this.apiUrl + '?' + this.toQueryString(query));
  }

  getExtension(id: string): Observable<Extension> {
    return this.http.get<Extension>(this.apiUrl + '/' + id)
      .pipe(
        map(ext => this.setThemeNames(ext))
      );
  }

  private toQueryString(queryObj: GalleryQuery): string {
    const parts = [];
    for (const property of Object.keys(queryObj)) {
      const value = queryObj[property];
      if (value !== null && value !== undefined && value !== '') {
        parts.push(encodeURIComponent(property) + '=' + encodeURIComponent(value));
      }
    }

    return parts.join('&');
  }

  private setThemeNames(extension: Extension): Extension {
    for (let i = 0; i < extension.themes.length; i++) {
      extension.themes[i].name = extension.name + i;
    }

    return extension;
  }
}
