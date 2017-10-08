import { Component, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {
    
    private baseUrl: string;
    orders: Order[];
    orderLines: OrderLine[];

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string){
        this.baseUrl =  baseUrl;
        this.performSearch('');
    }

    performSearch(searchTerm: string){
        console.log(`User entered: ${searchTerm}`);
        this.http.get(`${this.baseUrl}api/Order/Search?mask=${searchTerm}`).subscribe(result => {
            console.log(`Response is received`);
            this.orders = result.json() as Order[];
            console.log(this.orders);
        }, error => console.error(error));
    }
    
    editOrder(orderId: number){
        console.log(`Order edited: ${orderId}`);
        
        this.orderLines = [
             {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Bread"},
             {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Ham"},
             {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Milk"},
             {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Butter"},
             {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Potato"}

        ];
    }
}

interface Order {
    id: number;
    customerName: string;
    totalPrice: number;
}

class OrderLine {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    productPrice: number;
    quantity: number;
}