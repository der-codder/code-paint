import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ThemeInfo, GalleryQuery, QueryResult } from '..';

interface IQueryResultResource {
  totalCount: number;
  items: ThemeInfo[];
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
  private readonly originUrl = 'http://localhost:5021';
  private readonly apiEndpoint = '/api/gallery';

  constructor(private http: HttpClient) { }

  getThemes(query: GalleryQuery): Observable<QueryResult> {
    return this.http
      .get(this.originUrl + this.apiEndpoint + '?' + this.toQueryString(query))
      .pipe(
        map((data: IQueryResultResource) => this.toGalleryQuery(data))
      );
  }

  private toGalleryQuery(obj: IQueryResultResource): QueryResult {
    return { totalCount: obj.totalCount, themes: obj.items };
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
}
