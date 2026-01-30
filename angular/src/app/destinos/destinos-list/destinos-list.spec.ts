import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DestinosList } from './destinos-list';

describe('DestinosList', () => {
  let component: DestinosList;
  let fixture: ComponentFixture<DestinosList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DestinosList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DestinosList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
