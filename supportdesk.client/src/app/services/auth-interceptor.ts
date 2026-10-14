import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).getToken();
  
  if (!token)
  {
    return next(req);
  }

  const authRequest = req.clone(
    {setHeaders: { Authorization: `Bearer ${token}` }},
  )

  return next(req);
};
