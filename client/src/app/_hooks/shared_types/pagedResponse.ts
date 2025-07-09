export interface PagedResponse<T> {
  Items: T[];
  TotalCount: number;
  PageNumber: number;
  PageSize: number;
  TotalPages: number;
  HasPreviousPage: boolean;
  HasNextPage: boolean;
}
