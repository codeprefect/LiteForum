import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthService } from './_services/auth.service';
import { AlertService } from './_services/alert.service';
import { AuthGuard } from './_guards/auth.guard';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { StorageService } from './_services/storage.service';
import { AlertComponent } from './components/alert/alert.component';
import { PostService } from './_services/post.service';
import { CommentService } from './_services/comment.service';
import { ReplyService } from './_services/reply.service';
import { CategoryService } from './_services/category.service';
import { PostCardComponent } from './components/postcard/postcard.component';
import { MomentModule } from 'angular2-moment';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        AlertComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        PostCardComponent
    ],
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        MomentModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'post', component: HomeComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        AuthGuard,
        AlertService,
        AuthService,
        PostService,
        CommentService,
        ReplyService,
        StorageService,
        CategoryService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: JwtInterceptor,
            multi: true
        }
    ]
})
export class AppModuleShared {
}
