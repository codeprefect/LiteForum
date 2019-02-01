import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { NavMenuComponent } from '../navmenu/navmenu.component';
import { AlertComponent } from '../alert/alert.component';
import { AuthService } from '../../_services/auth.service';
import { AlertService } from '../../_services/alert.service';

describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent,
        NavMenuComponent,
        AlertComponent
      ],
      providers: [
        { provide: AuthService, useValue: {
          isLoggedIn: jest.fn().mockReturnValue(true)
        }},
        AlertService
      ]
    }).compileComponents();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have a navmenu element'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('lfc-nav-menu a').textContent).toContain('LiteForum');
  });

  it('should have a alert element', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('lfc-alert')).toBeDefined();
  });
});
