import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';

export interface User {
    id:number;
    firstName:string;
    lastName:string;
    email:string;
    role:string;
    isActive:boolean;
    createdAt:string;
}
export interface CreateUser {
    firstName:string;
    lastName:string;
    email:string;
    password:string;
    role:string;
}
export interface UpdateUser {
    firstName:string;
    lastName:string;
    email:string;
    role:string;
    isActive:boolean;
}

@Service()
export class UserService {
    private http = inject(HttpClient);
    private baseUrl = '/api/users';

    getUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.baseUrl);
    }
    getUser(id:number): Observable<User> {
        return this.http.get<User>(`${this.baseUrl}/${id}`);
    }
    createUser(user: CreateUser): Observable<User> {
        return this.http.post<User>(this.baseUrl, user);
    }
    updateUser(id:number, user:UpdateUser): Observable<User> {
        return this.http.put<User>(`${this.baseUrl}/${id}`, user);
    }
}
