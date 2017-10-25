/// <reference path="../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, fakeAsync, ComponentFixture } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { OrderService } from './order.service';
import { OrderServiceStub } from './order.service.stub';
import { Order, OrderLine, Product} from './entities';
import { Observable } from 'rxjs/Observable';

let fixture: ComponentFixture<HomeComponent>;
let orderServiceStub : OrderService
let comp: HomeComponent;

describe('Home component', () => {
    beforeEach(async() => {
        TestBed.configureTestingModule({ 
            declarations: [HomeComponent],
        }).overrideComponent(HomeComponent, {
            set: {
              providers: [{ provide: OrderService, useClass: OrderServiceStub }]
            }
          }).compileComponents()
        });

        beforeEach(()=>{
            fixture = TestBed.createComponent(HomeComponent);
            comp = fixture.componentInstance;
            orderServiceStub = fixture.debugElement.injector.get(OrderService);
        });

    it('fist test', (async() => {

        const spy = spyOn(orderServiceStub, 'getOrders').and.returnValue(
            Observable.of([{id: 1, customerName: "Pavel", totalPrice: 5}])
          );
          comp.ngOnInit();
          fixture.detectChanges();
          expect(comp.orders.length).toEqual(1);
          expect(spy.calls.any()).toEqual(true);
    }));

});
