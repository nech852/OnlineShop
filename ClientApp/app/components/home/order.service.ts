import { Injectable, Inject, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Http, Response } from '@angular/http';
import { Order, OrderLine, Product} from './entities';
//TODO: Should I insert only part of RxJs??
import 'rxjs/Rx';

@Injectable()
export class OrderService 
{
    private baseUrl: string;

    private errorOccured: EventEmitter<Error>;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string)
    {
        this.baseUrl =  baseUrl;
        this.errorOccured = new EventEmitter<Error>();
    }

    getProducts(): Observable<Product[]>
    {
        let url = `${this.baseUrl}api/Order/Products`;
        return this.http.get(url).map(res => this.convertToJSON<Product[]>(res, url));
    }

    getOrders(searchMask: string): Observable<Order[]>
    {
        let url = `${this.baseUrl}api/Order/Orders?mask=${searchMask}`;
        return this.http.get(url).map(res => this.convertToJSON<Order[]>(res, url));
    }

    getOrderLines(orderId: number): Observable<OrderLine[]>
    {
         let url = `${this.baseUrl}api/Order/OrderLines?orderId=${orderId}`;
         return this.http.get(url).map(res => this.convertToJSON<OrderLine[]>(res, url));
    }

    deleteOrder(orderId: number, searchMask: string): Observable<Order[]>
    {
        let url = `${this.baseUrl}api/Order/DeleteOrder`;
        return this.http.delete(url,
            {body: {OrderId: orderId, Mask: searchMask}}).map(res => this.convertToJSON<Order[]>(res, url));
    }

    deleteOrderLine(orderId: number, orderLineId: number): Observable<OrderLine[]>
    {
        let url = `${this.baseUrl}api/Order/DeleteOrderLine`;
        return this.http.delete(url,
            {body: {OrderId: orderId, OrderLineId: orderLineId }}).map(res => this.convertToJSON<OrderLine[]>(res, url));
    }

    addOrder(customerName: string, searchMask: string): Observable<Order[]>
    {
        let url = `${this.baseUrl}api/Order/AddOrder`;
        return this.http.post(url, {CustomerName: customerName, Mask: searchMask})
            .map(res => this.convertToJSON<Order[]>(res, url));
    }

    addOrderLine(orderId: number, productId: number, quantity: number): Observable<OrderLine[]>
    {
        let url = `${this.baseUrl}api/Order/AddOrderLine`;
        return this.http.post(url, 
            {OrderId: orderId, ProductId: productId, Quantity: quantity})
            .map(res => this.convertToJSON(res, url));            
    }

    getErrorOccuredEmitter() : EventEmitter<Error>
    {
        return this.errorOccured;
    }

    private convertToJSON<T>(res: Response, url: string): T {
        try {
            return res.json() as T
         }
         catch(err) {
            console.error(`Error while extracting json from url: ${url}`);
            console.error(res);
            this.errorOccured.emit(err);
            throw new Error(err);
         }
    }
}