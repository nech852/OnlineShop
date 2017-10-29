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
    beforeEach(() => {
        TestBed.configureTestingModule({ 
            declarations: [HomeComponent],
        }).overrideComponent(HomeComponent, {
            set: {
              providers: [{ provide: OrderService, useClass: OrderServiceStub }]
            }
          }).compileComponents()
        });

        beforeEach(()=> {
            fixture = TestBed.createComponent(HomeComponent);
            comp = fixture.componentInstance;
            orderServiceStub = fixture.debugElement.injector.get(OrderService);
        });

    it('Initialization', (() => {

        const orderSpy = spyOn(orderServiceStub, 'getOrders').and.returnValue(
            Observable.of([{id: 1, customerName: "Jack", totalPrice: 5},
                {id: 2, customerName: "Mike", totalPrice: 8}])
          );
          
        const productSpy = spyOn(orderServiceStub, 'getProducts').and.returnValue(
            Observable.of([{id: 1, name: "Milk", price: 5},
                {id: 2, name: "Bread", price: 8}]),
          );

        fixture.detectChanges();
        const orderRows = fixture.nativeElement.querySelectorAll('.orderTable .orderRow');
        expect(orderRows.length).toEqual(2);
        expect(orderSpy.calls.any()).toEqual(true);
        const firstOrderRow = orderRows[0];
        expect(firstOrderRow.querySelector(".idCell").innerText).toEqual("1");
        expect(firstOrderRow.querySelector(".nameCell").innerText).toEqual("Jack");
        expect(firstOrderRow.querySelector(".totalPriceCell").innerText).toEqual("5.00");
        const secondOrderRow = orderRows[1];
        expect(secondOrderRow.querySelector(".idCell").innerText).toEqual("2");
        expect(secondOrderRow.querySelector(".nameCell").innerText).toEqual("Mike");
        expect(secondOrderRow.querySelector(".totalPriceCell").innerText).toEqual("8.00");
    }));


    it('Edit Order', (() => {
        
        const orderSpy = spyOn(orderServiceStub, 'getOrders').and.returnValue(
            Observable.of([{id: 1, customerName: "Jack", totalPrice: 0},
            {id: 2, customerName: "Mike", totalPrice: 0}]));
                  
        const productSpy = spyOn(orderServiceStub, 'getProducts').and.returnValue(
            Observable.of([{id: 1, name: "Milk", price: 5},
            {id: 2, name: "Bread", price: 8}]),);
        
        fixture.detectChanges();
        const orderRows = fixture.nativeElement.querySelectorAll('.orderTable .orderRow');
        const firstOrderRow = orderRows[0];
        const editOrderButton = firstOrderRow.querySelector(".editOrderButton");
        editOrderButton.click();
        fixture.detectChanges();
        const productList = fixture.nativeElement.querySelector('.productList');
        const productOptions = productList.querySelectorAll('.productOption');
        expect(productOptions[0].innerText.trim()).toEqual("Milk - 5");
        expect(productOptions[1].innerText.trim()).toEqual("Bread - 8");
        expect(productOptions.length).toEqual(2);

        const quantityInput = fixture.nativeElement.querySelector(".quantityInput");
        quantityInput.value = "6";

        const addOrderLineButton = fixture.nativeElement.querySelector(".addOrderLineButton");
        
        const orderLinesSpy = spyOn(orderServiceStub, 'addOrderLine').and.callFake(
            (orderId: number, productId: number, quantity: number) => 
            {
                let result: OrderLine[] = [];
                if(orderId === 1 && productId === 1 && quantity === 6)
                {
                    result = [{id: 8, orderId: 1, productId: 1, 
                        productName: "Milk", productPrice: 5, quantity: 6}];
                } 
                else if(orderId === 1 && productId === 2 && quantity === 8)
                {
                    result = [{id: 8, orderId: 1, productId: 1, 
                                productName: "Milk", productPrice: 5, quantity: 6},
                             {id: 9, orderId: 1, productId: 2, 
                                    productName: "Milk", productPrice: 8, quantity: 8}
                            ];
                }
                return Observable.of(result);
            });

        addOrderLineButton.click();
        fixture.detectChanges();
        let orderLineRows = fixture.nativeElement.querySelectorAll('.orderLineTable .orderLineRow');
        expect(orderLineRows.length).toEqual(1);
        productList.selectedIndex = 1;
        quantityInput.value = "8";
        addOrderLineButton.click();
        fixture.detectChanges();
        orderLineRows = fixture.nativeElement.querySelectorAll('.orderLineTable .orderLineRow');
        expect(orderLineRows.length).toEqual(2);
        expect(firstOrderRow.querySelector('.totalPriceCell').innerText).toEqual("94.00");
    }));
        

});
