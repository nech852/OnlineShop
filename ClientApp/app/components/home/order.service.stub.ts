import { EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Order, OrderLine, Product} from './entities';

export class OrderServiceStub
{
    private errorOccured: EventEmitter<Error>;

    constructor()
    {
        this.errorOccured = new EventEmitter<Error>();
    }

    getProducts(): Observable<Product[]>
    {
        return Observable.of([]);
    }

    getOrders(searchMask: string): Observable<Order[]>
    {
        return Observable.of([]);
    }

    getOrderLines(orderId: number): Observable<OrderLine[]>
    {
        return Observable.of([]);
    }

    deleteOrder(orderId: number, searchMask: string): Observable<Order[]>
    {
        return Observable.of([]);
    }

    deleteOrderLine(orderId: number, orderLineId: number): Observable<OrderLine[]>
    {
        return Observable.of([]);
    }

    addOrder(customerName: string, searchMask: string): Observable<Order[]>
    {
        return Observable.of([]);
    }

    addOrderLine(orderId: number, productId: number, quantity: number): Observable<OrderLine[]>
    {
        return Observable.of([]);
    }

    getErrorOccuredEmitter() : EventEmitter<Error>
    {
        return this.errorOccured;
    }
}