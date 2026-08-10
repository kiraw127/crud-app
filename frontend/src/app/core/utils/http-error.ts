import { HttpErrorResponse } from '@angular/common/http';

export function getErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    return error.error?.detail ?? error.error ?? fallback;
  }

  return fallback;
}
