import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { MaterialModule } from '../material-module';
import { UserList } from './user-list/user-list';
import { UserForm } from './user-form/user-form';

const routes: Routes = [
    { path: '', component: UserList },
    { path: 'new', component: UserForm },
    { path: ':id/edit', component: UserForm },
];

@NgModule({
    declarations: [UserList, UserForm],
    imports: [CommonModule, ReactiveFormsModule, MaterialModule, RouterModule.forChild(routes)],
})
export class UsersModule { }