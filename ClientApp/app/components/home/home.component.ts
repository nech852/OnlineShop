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
    products: Product[];

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string){
        this.baseUrl =  baseUrl;
        this.performSearch('');
        this.orderLines = [
            {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Bread"},
            {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Ham"},
            {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Milk"},
            {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Butter"},
            {id: 1, orderId: 1, productId:1, productPrice: 4.5, quantity: 3, productName: "Potato"}
       ];

       this.products = [
            {id: 1,  price: 4.5, name: "Bread"},
            {id: 1,  price: 4.5, name: "Ham"},
            {id: 1,  price: 4.5, name: "Milk"},
            {id: 1,  price: 4.5, name: "Butter"},
            {id: 1,  price: 4.5, name: "Potato"},
       ];
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
    }

    deleteOrder(orderId: number){
        let index = this.orders.findIndex(order => order.id === orderId);
        this.orders.splice(index, 1);
    }

    deleteOrderLine(orderLineId: number){
        let index = this.orderLines.findIndex(orderLine => orderLine.id === orderLineId);
        this.orderLines.splice(index, 1);
    }

    addOrder(customerName: string){
        this. orders.push({id:1, customerName:customerName, totalPrice:24});
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

class Product {
    id: number;
    name: string;
    price: number;
}