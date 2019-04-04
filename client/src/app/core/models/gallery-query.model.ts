import { ThemeInfo } from './theme-info.model';

export interface GalleryQuery {
  searchTerm: string;
  sortBy: string;
  pageNumber: number;
  pageSize: number;
}
