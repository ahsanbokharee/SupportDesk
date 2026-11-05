import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { MaterialModule } from '../material-module';
import { CategoryList } from './category-list/category-list';
import { CategoryForm } from './category-form/category-form';

const routes: Routes = [
    { path: '', component: CategoryList },
    { path: 'new', component: CategoryForm },
    { path: ':id/edit', component: CategoryForm },
];

@NgModule({
    declarations: [CategoryList, CategoryForm],
    imports: [CommonModule, ReactiveFormsModule, MaterialModule, RouterModule.forChild(routes)],
})
export class CategoriesModule { }